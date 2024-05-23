namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public enum LevelEvent { None, LevelIntro, PlayerDeath, ClearLevel }
    public enum GhostStateEvent { None, ShowGhosts, HideGhosts, EnterChaseMode, EnterScatterMode, 
        EnterFrightenedMode, WarnLeavingFrightenedMode, LeaveFrightenedMode }
    public enum BonusSymbol { Cherries, Strawberry, Peach, Apple, Grapes, Galaxian, Bell, Key}

    public class Level
    {
        public int levelIndex;
        public Tilemap Map;

        public int _InkyDotLimit;
        public int _ClydeDotLimit;
        public int _Elroy1DotsLeft;
        public List<double> _PlannedGhostCycle = new List<double>();
        public double _PowerPelletDuration;
        public double _FlashDuration;
        public float _PlayerNormalSpeedModifier;
        public float _PlayerPoweredSpeedModifier;
        public float _GhostNormalSpeedModifier; 
        public float _GhostFrightenedSpeedModifier;
        public float _GhostTunnelSpeedModifier;
        public int _FruitNumber;

        public bool IsLevelPaused = false;
        public double LevelPausedAge;
        public int EatenDotCount = 0;
        private string currentSirenName = "siren_1";

        public LevelEvent LevelEvent = LevelEvent.LevelIntro;
        public double LeftEventDuration;
        public int EventStage = 0;

        public bool IsPowered;
        public double LeftPoweredDuration;
        public int PoweredStage;
        public int GhostsEatenInRow;
        public int GhostsCapturedThisLevel = 0;

        public GhostStateEvent GhostStateEvent;
        public GhostState ghostCycle;
        public double ghostCycleAge;
        private int ghostCycleIndex;
        public int retreatingGhosts;
        public bool ElroyPossible;

        public List<Ghost> housedGhosts = new List<Ghost>();
        public bool UsePersonalCounter = true;
        public int personalDotCount;
        public int globalDotCount;
        public double personalTime;

        public bool ShortenIntro = true;
        public bool GotoNextLevel = false;

        public Player Player;
        public Blinky Blinky;
        public Pinky Pinky;
        public Inky Inky;
        public Clyde Clyde;
        public Fruit Fruit;
        public TextObject MazeText;
        public TextObject ScoreIndicatorText;
        public TextObject HighScoreIndicatorText;
        public TextObject ScoreText;
        public TextObject HighScoreText;
        public LivesCounter LivesCounter;
        public FruitCounter FruitCounter;

        public Level(int levelIndex) {
            this.levelIndex = levelIndex;
            Map = new Tilemap();
            Player = new Player();
            Blinky = new Blinky();
            Pinky = new Pinky();
            Inky = new Inky();
            Clyde = new Clyde();
            Fruit = new Fruit();
            MazeText = new TextObject();
            ScoreIndicatorText = new TextObject();
            ScoreText = new TextObject();
            HighScoreIndicatorText = new TextObject();
            HighScoreText = new TextObject();
            LivesCounter = new LivesCounter();
            FruitCounter = new FruitCounter();
        }

        public void LoadContent(ContentManager content) {
            Map.LoadContent(content);
            Player.LoadContent(content);
            Blinky.LoadContent(content);
            Pinky.LoadContent(content);
            Inky.LoadContent(content);
            Clyde.LoadContent(content);
            Fruit.LoadContent(content);
            MazeText.LoadContent();
            ScoreIndicatorText.LoadContent();
            ScoreText.LoadContent();
            HighScoreIndicatorText.LoadContent();
            HighScoreText.LoadContent();
            LivesCounter.LoadContent();
            FruitCounter.LoadContent();
        }

        public void Initialize() {
            InitializeLevelSettings();
            Player.Initialize();
            Blinky.Initialize();
            Pinky.Initialize();
            Inky.Initialize();
            Clyde.Initialize();

            ScoreIndicatorText.Initialize(new Vector2(4, 0), "1UP", Color.White);
            HighScoreIndicatorText.Initialize(new Vector2(13.5f, 0), "HIGH SCORE", Color.White);
            ScoreText.Initialize(new Vector2(4, 1), "00", Color.White);
            HighScoreText.Initialize(new Vector2(13.5f, 1), " ", Color.White);

            LivesCounter.Initialize();
            FruitCounter.Initialize();

            IsPowered = false;
            LeftPoweredDuration = 0;
            PoweredStage = 0;
            GhostsEatenInRow = 0;

            ghostCycleIndex = 0;
            ghostCycleAge = 0;
            ghostCycle = GhostState.Scatter;
            retreatingGhosts = 0;

            housedGhosts.Add(Pinky);
            housedGhosts.Add(Inky);
            housedGhosts.Add(Clyde);
            personalDotCount = 0;
            globalDotCount = 0;
            personalTime = 0;
            ElroyPossible = false;

            AudioManager.Instance.RemoveAllSingletonSoundEffect();
            AudioManager.Instance.RemoveAllSoundEffect();
        }

        private void InitializeLevelSettings() {
            #region _DotLimits
            if (levelIndex == 1) {
                _InkyDotLimit = 30;
                _ClydeDotLimit = 60;
            } else if (levelIndex == 2) {
                _InkyDotLimit = 0;
                _ClydeDotLimit = 50;
            } else {
                _InkyDotLimit = 0;
                _ClydeDotLimit = 0;
            }


            #endregion

            #region _ElroySettings
            if (levelIndex == 1)
                _Elroy1DotsLeft = 20;
            else if (levelIndex == 2)
                _Elroy1DotsLeft = 30;
            else if (3 <= levelIndex && levelIndex <= 5)
                _Elroy1DotsLeft = 40;
            else if (6 <= levelIndex && levelIndex <= 8)
                _Elroy1DotsLeft = 50;
            else if (9 <= levelIndex && levelIndex <= 11)
                _Elroy1DotsLeft = 60;
            else if (12 <= levelIndex && levelIndex <= 14)
                _Elroy1DotsLeft = 80;
            else if (15 <= levelIndex && levelIndex <= 18)
                _Elroy1DotsLeft = 100;
            else
                _Elroy1DotsLeft = 120;
            #endregion

            #region _PlannedGhostCycle
            if (levelIndex == 1) {
                _PlannedGhostCycle.Add(7000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(7000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(5000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(5000);
            } else if (2 <= levelIndex && levelIndex <= 4) {
                _PlannedGhostCycle.Add(7000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(7000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(5000);
                _PlannedGhostCycle.Add(1033000);
                _PlannedGhostCycle.Add(16);
            } else {
                _PlannedGhostCycle.Add(5000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(5000);
                _PlannedGhostCycle.Add(20000);
                _PlannedGhostCycle.Add(5000);
                _PlannedGhostCycle.Add(1037000);
                _PlannedGhostCycle.Add(5000);
            }
            #endregion

            # region _PowerPelletDuration & _FlashDuration
            if (levelIndex == 1)
                _PowerPelletDuration = 6000;
            else if (levelIndex == 2 || levelIndex == 6 || levelIndex == 10)
                _PowerPelletDuration = 5000;
            else if (levelIndex == 3)
                _PowerPelletDuration = 4000;
            else if (levelIndex == 4 || levelIndex == 14)
                _PowerPelletDuration = 3000;
            else if (levelIndex == 5 || levelIndex == 7 || levelIndex == 8 || levelIndex == 11)
                _PowerPelletDuration = 2000;
            else if (levelIndex == 9 || levelIndex == 12 || levelIndex == 13 || levelIndex == 12
                || levelIndex == 15 || levelIndex == 16 || levelIndex == 18)
                _PowerPelletDuration = 1000;
            else
                _PowerPelletDuration = 0;

            if (_PowerPelletDuration == 1000)
                _FlashDuration = 1000;
            else if (_PowerPelletDuration == 0)
                _FlashDuration = 0;
            else
                _FlashDuration = 2000;
            #endregion

            #region _SpeedModifiers
            if (levelIndex == 1) {
                _PlayerNormalSpeedModifier = 0.8f;
                _PlayerPoweredSpeedModifier = 0.9f;
                _GhostNormalSpeedModifier = 0.75f;
                _GhostFrightenedSpeedModifier = 0.5f;
                _GhostTunnelSpeedModifier = 0.4f;
            } else if ((levelIndex >= 2 && levelIndex <= 4)) {
                _PlayerNormalSpeedModifier = 0.9f;
                _PlayerPoweredSpeedModifier = 0.95f;
                _GhostNormalSpeedModifier = 0.85f;
                _GhostFrightenedSpeedModifier = 0.55f;
                _GhostTunnelSpeedModifier = 0.45f;
            } else if ((levelIndex >= 5 && levelIndex <= 20)) {
                _PlayerNormalSpeedModifier = 1f;
                _PlayerPoweredSpeedModifier = 1f;
                _GhostNormalSpeedModifier = 0.95f;
                _GhostFrightenedSpeedModifier = 0.6f;
                _GhostTunnelSpeedModifier = 0.5f;
            } else {
                _PlayerNormalSpeedModifier = 0.9f;
                _PlayerPoweredSpeedModifier = 0.9f;
                _GhostNormalSpeedModifier = 0.95f;
                _GhostFrightenedSpeedModifier = 0.95f;
                _GhostTunnelSpeedModifier = 0.5f;
            }

            #endregion

            #region _FruitNumber
            if (levelIndex == 1)
                _FruitNumber = 1;
            else if (levelIndex == 2)
                _FruitNumber = 2;
            else if (levelIndex == 3 || levelIndex == 4)
                _FruitNumber = 3;
            else if (levelIndex == 5 || levelIndex == 6)
                _FruitNumber = 4;
            else if (levelIndex == 7 || levelIndex == 8)
                _FruitNumber = 5;
            else if (levelIndex == 9 || levelIndex == 10)
                _FruitNumber = 6;
            else if (levelIndex == 11 || levelIndex == 12)
                _FruitNumber = 7;
            else
                _FruitNumber = 8;
            #endregion
        }

        public void UnloadContent() {
            AudioManager.Instance.RemoveAllSoundEffect();
            AudioManager.Instance.RemoveAllSingletonSoundEffect();
            Map.UnloadContent();
            Player.UnloadContent();
            Blinky.UnloadContent();
            Pinky.UnloadContent();
            Inky.UnloadContent();
            Clyde.UnloadContent();
            Fruit.UnloadContent();
        }
        
        public void Update(GameTime gameTime) {
            ScoreText.Text = LevelManager.Instance.Score.ToString();
            HighScoreText.Text = LevelManager.HighScore.ToString();

            if (InputManager.Instance.KeyPressed(Keys.F1)) {
                if (!LevelManager.Debug)
                    LevelManager.Debug = true;
                else
                    LevelManager.Debug = false;
            }

            if (LevelManager.Debug && InputManager.Instance.KeyPressed(Keys.L)) {
                System.Diagnostics.Debug.WriteLine("Debug: skipped this Level!");
                GotoNextLevel = true;
            } else if (LevelManager.Debug && InputManager.Instance.KeyPressed(Keys.K)) {
                System.Diagnostics.Debug.WriteLine("Debug: killed the Player!");
                if (LevelEvent == LevelEvent.None)
                    LevelEvent = LevelEvent.PlayerDeath;
                else if (LevelManager.Instance.Lives != 1) {
                    LevelManager.Instance.Lives--;
                    LivesCounter.Initialize();
                }
            } else if (LevelManager.Debug && InputManager.Instance.KeyPressed(Keys.F) && LevelEvent == LevelEvent.None) {
                Fruit.Initialize(_FruitNumber);
                System.Diagnostics.Debug.WriteLine("Debug: initialized a fruit!");
            }

            if (LevelPausedAge > 0) {
                IsLevelPaused = true;
                LevelPausedAge -= gameTime.ElapsedGameTime.TotalMilliseconds;
            } else
                IsLevelPaused = false;

            ManageStates(gameTime);

            Map.Update(gameTime);
            Player.Update(gameTime);
            Blinky.Update(gameTime);
            Pinky.Update(gameTime);
            Inky.Update(gameTime);
            Clyde.Update(gameTime);
            Fruit.Update(gameTime);
            ScoreIndicatorText.Update(gameTime);
        }

        private void ManageStates(GameTime gameTime) {
            ManageEventStates(gameTime);
            ManagePlayerStates(gameTime);
            ManageGhostStates(gameTime);
            ManageLevelClearage();
        }

        private void ManageEventStates(GameTime gameTime) {
            LeftEventDuration -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (LevelEvent == LevelEvent.LevelIntro) {
                if (!ShortenIntro) {
                    if (EventStage == 0) {
                        LeftEventDuration = 5000;
                        System.Diagnostics.Debug.WriteLine("Level Started! - Level " + levelIndex);
                        Initialize();
                        AudioManager.Instance.PlaySingletonSoundEffect("game_start");
                        MazeText.Initialize(new Vector2(Map.GetPlayerStartTile().X, Map.GetPlayerStartTile().Y - 6), "READY!", Color.Yellow);
                        
                        EventStage = 1;
                    } else if (EventStage == 1 && LeftEventDuration <= 3000) {
                        GhostStateEvent = GhostStateEvent.ShowGhosts;
                        EventStage = 2;
                    } else if (EventStage == 2 && LeftEventDuration <= 0) {
                        MazeText.IsShown = false;
                        Player.IsMovable = true;
                        Blinky.IsMovable = true;
                        Pinky.IsMovable = true;
                        Inky.IsMovable = true;
                        Clyde.IsMovable = true;
                        LevelEvent = LevelEvent.None;
                        EventStage = 0;
                        ShortenIntro = true;
                    }
                } else {
                    if (EventStage == 0) {
                        LeftEventDuration = 3000;
                        System.Diagnostics.Debug.WriteLine("Level Started! - Level " + levelIndex);
                        MazeText.Initialize(new Vector2(Map.GetPlayerStartTile().X, Map.GetPlayerStartTile().Y - 6), "READY!", Color.Yellow);
                        Initialize();
                        GhostStateEvent = GhostStateEvent.ShowGhosts;
                        EventStage = 1;
                    } else if (EventStage == 1 && LeftEventDuration <= 0) {
                        MazeText.IsShown = false;
                        Player.IsMovable = true;
                        Blinky.IsMovable = true;
                        Pinky.IsMovable = true;
                        Inky.IsMovable = true;
                        Clyde.IsMovable = true;
                        LevelEvent = LevelEvent.None;
                        EventStage = 0;
                    }
                }
                
            }

            if (LevelEvent == LevelEvent.PlayerDeath) {
                if (EventStage == 0) {
                    System.Diagnostics.Debug.WriteLine("Player Ded");
                    Player.IsMovable = false;
                    Player.ForceSwitchAnimation("Default");
                    AudioManager.Instance.RemoveAllSingletonSoundEffect();
                    AudioManager.Instance.RemoveAllSoundEffect();
                    LeftEventDuration = 4500;
                    LevelPausedAge = 2000;
                    EventStage = 1;
                } else if (EventStage == 1 && LeftEventDuration <= 3000) {
                    GhostStateEvent = GhostStateEvent.HideGhosts;
                    Fruit.IsAvailable = false;
                    Player.ForceSwitchAnimation("Death");
                    AudioManager.Instance.PlaySingletonSoundEffect("death_1");
                    EventStage = 2;
                } else if (EventStage == 2 && LeftEventDuration <= 1750) {
                    Player.IsShown = false;
                    AudioManager.Instance.RemoveSingletonSoundEffect("death_1");
                    AudioManager.Instance.PlaySoundEffect("death_2");
                    EventStage = 3;
                } else if (EventStage == 3 && LeftEventDuration <= 1500) {
                    AudioManager.Instance.PlaySoundEffect("death_2");
                    EventStage = 4;
                } else if (EventStage == 4 && LeftEventDuration <= 0) {
                    if (LevelManager.Instance.Lives != 1) {
                        EventStage = 0;
                        LevelEvent = LevelEvent.LevelIntro;
                        UsePersonalCounter = false;
                        globalDotCount = 0;
                        LevelManager.Instance.Lives--;
                    } else {
                        MazeText.Initialize(new Vector2(Map.GetPlayerStartTile().X, Map.GetPlayerStartTile().Y - 6), "GAME OVER", Color.Red);
                        EventStage = 5;
                    }
                } else if (EventStage == 5 && LeftEventDuration <= -2000) {
                    StateManager.Instance.SwitchGameState(new MenuState());
                }
            }
            
            if (LevelEvent == LevelEvent.ClearLevel) {
                if (EventStage == 0) {
                    System.Diagnostics.Debug.WriteLine("Cleared a Level!");
                    Player.IsMovable = false;
                    Player.ForceSwitchAnimation("Default");
                    Fruit.IsAvailable = false;
                    AudioManager.Instance.RemoveAllSingletonSoundEffect();
                    AudioManager.Instance.RemoveAllSoundEffect();
                    LeftEventDuration = 4000;
                    LevelPausedAge = 2000;
                    EventStage = 1;
                } else if (EventStage == 1 && LeftEventDuration <= 3000) {
                    GhostStateEvent = GhostStateEvent.HideGhosts;
                    Map.Flash();
                    EventStage = 2;
                } else if (EventStage == 2 && LeftEventDuration <= 1750) {
                    Player.IsShown = false;
                    Map.IsShown = false;
                    GotoNextLevel = true;
                }
            }
        }

        public void ManageGhostStates(GameTime gameTime) {
            if (GhostStateEvent != GhostStateEvent.None) {
                Blinky.ReceiveGhostState(GhostStateEvent);
                Pinky.ReceiveGhostState(GhostStateEvent);
                Inky.ReceiveGhostState(GhostStateEvent);
                Clyde.ReceiveGhostState(GhostStateEvent);
                GhostStateEvent = GhostStateEvent.None;
            }

            if (!IsLevelPaused && LevelEvent == LevelEvent.None) {
                if (ghostCycleIndex != _PlannedGhostCycle.Count) {
                    if (ghostCycleAge >= _PlannedGhostCycle[ghostCycleIndex]) {
                        if (ghostCycle == GhostState.Scatter) {
                            ghostCycle = GhostState.Chase;
                            System.Diagnostics.Debug.WriteLine("Ghosts entered the Chase Mode!");
                            GhostStateEvent = GhostStateEvent.EnterChaseMode;
                        } else {
                            ghostCycle = GhostState.Scatter;
                            System.Diagnostics.Debug.WriteLine("Ghosts entered the Scatter Mode!");
                            GhostStateEvent = GhostStateEvent.EnterScatterMode;
                        }
                        ghostCycleIndex++;
                        ghostCycleAge = 0;
                    } else {
                        ghostCycleAge += gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }

                if (housedGhosts.Count != 0) {
                    personalTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (housedGhosts.Contains(Blinky)) {
                        System.Diagnostics.Debug.WriteLine("Blinky leaves the Ghost House!");
                        housedGhosts.Remove(Blinky);
                        Blinky.PendingToLeave = true;
                        personalDotCount = 0;
                        personalTime = 0;
                    }

                    if (!housedGhosts.Contains(Blinky)) {
                        if (UsePersonalCounter) {
                            if (housedGhosts.Contains(Pinky)) {
                                System.Diagnostics.Debug.WriteLine("Pinky leaves the Ghost House!");
                                housedGhosts.Remove(Pinky);
                                Pinky.PendingToLeave = true;
                                personalDotCount = 0;
                                personalTime = 0;
                            } else if (housedGhosts.Contains(Inky) && (personalDotCount >= _InkyDotLimit || personalTime >= 4000)) {
                                System.Diagnostics.Debug.WriteLine("Inky leaves the Ghost House!");
                                housedGhosts.Remove(Inky);
                                Inky.PendingToLeave = true;
                                personalDotCount = 0;
                                personalTime = 0;
                            } else if (housedGhosts.Contains(Clyde) && (personalDotCount >= _ClydeDotLimit || personalTime >= 4000)) {
                                System.Diagnostics.Debug.WriteLine("Clyde leaves the Ghost House!");
                                housedGhosts.Remove(Clyde);
                                Clyde.PendingToLeave = true;
                                personalDotCount = 0;
                                personalTime = 0;
                            }
                        } else {
                            if (housedGhosts.Contains(Pinky) && (globalDotCount == 7 || personalTime >= 4000)) {
                                System.Diagnostics.Debug.WriteLine("Pinky leaves the Ghost House! (using global counter)");
                                housedGhosts.Remove(Pinky);
                                Pinky.PendingToLeave = true;
                                personalTime = 0;
                            } else if (housedGhosts.Contains(Inky) && (globalDotCount == 17 || personalTime >= 4000)) {
                                System.Diagnostics.Debug.WriteLine("Inky leaves the Ghost House! (using global counter)");
                                housedGhosts.Remove(Inky);
                                Inky.PendingToLeave = true;
                                personalTime = 0;
                            } else if (housedGhosts.Contains(Clyde) && (globalDotCount == 32 || personalTime >= 4000)) {
                                System.Diagnostics.Debug.WriteLine("Clyde leaves the Ghost House! (using global counter)");
                                housedGhosts.Remove(Clyde);
                                Clyde.PendingToLeave = true;
                                personalTime = 0;
                                UsePersonalCounter = true;
                            }
                        }
                    }
                }
            }
        }

        public void ManagePlayerStates(GameTime gameTime) {
            if (IsPowered) {
                if (!IsLevelPaused && LeftPoweredDuration > 0)
                    LeftPoweredDuration -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (PoweredStage == 0) {
                    System.Diagnostics.Debug.WriteLine("Player Powered!");
                    LeftPoweredDuration = _PowerPelletDuration;
                    LevelManager.Instance.CurrentLevel.GhostStateEvent = GhostStateEvent.EnterFrightenedMode;
                    AudioManager.Instance.RemoveSingletonSoundEffect(currentSirenName);
                    PoweredStage = 1;
                } else if (PoweredStage == 1 && LeftPoweredDuration <= 2000) {
                    GhostStateEvent = GhostStateEvent.WarnLeavingFrightenedMode;
                    PoweredStage = 2;
                } else if (PoweredStage == 2 && LeftPoweredDuration <= 0) {
                    GhostStateEvent = GhostStateEvent.LeaveFrightenedMode;
                    GhostsEatenInRow = 0;
                    PoweredStage = 0;
                    IsPowered = false;
                }
            }
        }

        private void ManageLevelClearage() {
            // there are 244 dots.

            if (EatenDotCount > 244 - _Elroy1DotsLeft / 2 && ElroyPossible) {
                Blinky.ElroyState = 2;
            } else if (EatenDotCount > 244 - _Elroy1DotsLeft && ElroyPossible) {
                Blinky.ElroyState = 1;
            }


            if (EatenDotCount < 122)
                currentSirenName = "siren_1";
            else if (EatenDotCount < 244 - 61) {
                currentSirenName = "siren_2";
                AudioManager.Instance.RemoveSingletonSoundEffect("siren_1");
            } else if (EatenDotCount < 244 - 30) {
                AudioManager.Instance.RemoveSingletonSoundEffect("siren_2");
                currentSirenName = "siren_3";
            } else if (EatenDotCount < 244 - 15) {
                AudioManager.Instance.RemoveSingletonSoundEffect("siren_3");
                currentSirenName = "siren_4";
            } else if (EatenDotCount < 244 - 7) {
                AudioManager.Instance.RemoveSingletonSoundEffect("siren_4");
                currentSirenName = "siren_5";
            } else if (EatenDotCount == 244) {
                LevelEvent = LevelEvent.ClearLevel;
            }
            if (LevelEvent == LevelEvent.None && EatenDotCount != 244) {
                if (IsPowered) {
                    AudioManager.Instance.RemoveSingletonSoundEffect(currentSirenName);
                    if (retreatingGhosts != 0) {
                        AudioManager.Instance.RemoveSingletonSoundEffect("power_pellet");
                        AudioManager.Instance.PlaySingletonSoundEffect("retreating");
                    } else {
                        AudioManager.Instance.RemoveSingletonSoundEffect("retreating");
                        AudioManager.Instance.PlaySingletonSoundEffect("power_pellet");
                    }
                } else {
                    AudioManager.Instance.RemoveSingletonSoundEffect("power_pellet");
                    AudioManager.Instance.RemoveSingletonSoundEffect("retreating");
                    AudioManager.Instance.PlaySingletonSoundEffect(currentSirenName);
                }
            }
        }

        public void Draw() {
            Map.Draw();
            Fruit.Draw();
            Player.Draw();
            Clyde.Draw();
            Inky.Draw(); 
            Pinky.Draw();
            Blinky.Draw();
            MazeText.Draw();
            ScoreIndicatorText.Draw();
            ScoreText.Draw();
            HighScoreIndicatorText.Draw();
            HighScoreText.Draw();
            LivesCounter.Draw();
            FruitCounter.Draw();
        }
    }
}