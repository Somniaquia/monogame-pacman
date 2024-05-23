namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class Player : Entity
    {
        private Animation Default = new Animation(16);
        private Animation FaceLeft = new Animation(16);
        private Animation FaceRight = new Animation(16);
        private Animation FaceUp = new Animation(16);
        private Animation FaceDown = new Animation(16);
        private Animation Death = new Animation(16, 100);
        private Animation Giant = new Animation(32);

        protected float normalSpeedModifier;
        protected float poweredSpeedModifier;

        private bool turnable;
        private bool isTurning;
        private bool preturn;
        private bool oddNumberEat;
        private int pausedFrame;

        public bool IsShown;
        public bool IsMovable;

        public void LoadContent(ContentManager content) {
            Default.AddFrame(new Point(2, 0));
            FaceLeft.AddFrame(new Point(2, 0), new Point(1, 1), new Point(0, 1), new Point(1, 1));
            FaceRight.AddFrame(new Point(2, 0), new Point(1, 0), new Point(0, 0), new Point(1, 0));
            FaceUp.AddFrame(new Point(2, 0), new Point(1, 2), new Point(0, 2), new Point(1, 2));
            FaceDown.AddFrame(new Point(2, 0), new Point(1, 3), new Point(0, 3), new Point(1, 3));
            for (int i = 3; i <= 13; i++)
                Death.AddFrame(new Point(i, 0));
            Death.AddFrame(new Point(13, 0), new Point(13, 1));
            Giant.AddFrame(new Point(1, 1), new Point(2, 1), new Point(3, 1), new Point(2, 1));
        }

        public override void Initialize() {
            if (StateManager.Instance.CurrentState.IsMenuState) {
                Position = new Vector2(14 * 8, 29 * 8);
                IsShown = true;
                currentAnimation = Default;
            } else {
                if (LevelManager.Instance.IsUsingLevel) {
                    base.Initialize();
                    currentAnimation = Default;
                    pendingDirection = Direction.Left;
                    MovingDirection = Direction.Null;

                    Position = LevelManager.Instance.CurrentLevel.Map.GetPlayerStartTile() * 8;
                    Position.X += 4;
                    Position.Y += 4;

                    normalSpeedModifier = LevelManager.Instance.CurrentLevel._PlayerNormalSpeedModifier;
                    poweredSpeedModifier = LevelManager.Instance.CurrentLevel._PlayerPoweredSpeedModifier;
                    CurrentSpeed = _fullSpeed * normalSpeedModifier;

                    IsShown = true;
                    IsMovable = false;
                    turnable = false;
                    isTurning = false;
                    preturn = true;
                    oddNumberEat = true;
                    pausedFrame = 0;

                    UpdateCheckingTileInfo();
                } else {
                    if (LevelManager.Instance.CurrentIntermission.IntermissionIndex == 1) {
                        currentAnimation = FaceLeft;
                        Position = new Vector2(ScreenManager.Instance.WindowDimensions.X + 32,
                            ScreenManager.Instance.WindowDimensions.Y / 2);
                        CurrentSpeed = _fullSpeed * 0.9f;
                        MovingDirection = Direction.Left;

                        IsShown = true;
                        IsMovable = false;
                    } else if (LevelManager.Instance.CurrentIntermission.IntermissionIndex == 2) {
                        currentAnimation = FaceLeft;
                        Position = new Vector2(ScreenManager.Instance.WindowDimensions.X + 32,
                            ScreenManager.Instance.WindowDimensions.Y / 2);
                        CurrentSpeed = _fullSpeed * 0.8f;
                        MovingDirection = Direction.Left;

                        IsShown = true;
                        IsMovable = true;
                    } else if (LevelManager.Instance.CurrentIntermission.IntermissionIndex == 3) {
                        currentAnimation = FaceLeft;
                        Position = new Vector2(ScreenManager.Instance.WindowDimensions.X + 32,
                            ScreenManager.Instance.WindowDimensions.Y / 2);
                        CurrentSpeed = _fullSpeed;
                        MovingDirection = Direction.Left;

                        IsShown = true;
                        IsMovable = true;
                    }
                }
            }
        }

        public void UnloadContent() {
            // TODO: Figure out what to do when UnloadContent is called.
        }

        public void Update(GameTime gameTime) {
            if (StateManager.Instance.CurrentState.IsMenuState) {
                if (InputManager.Instance.KeyPressed(Keys.Left))
                    currentAnimation = FaceLeft;
                else if (InputManager.Instance.KeyPressed(Keys.Right))
                    currentAnimation = FaceRight;
                else if (InputManager.Instance.KeyPressed(Keys.Up))
                    currentAnimation = FaceUp;
                else if (InputManager.Instance.KeyPressed(Keys.Down))
                    currentAnimation = FaceDown;

                currentAnimation.Update(gameTime);
            } else {
                if (LevelManager.Instance.IsUsingLevel) {
                    if (!isTurning)
                        TrySetPendingDirection();
                    if (IsShown) {
                        if (IsMovable && !LevelManager.Instance.CurrentLevel.IsLevelPaused) {
                            if (pausedFrame == 0)
                                InteractWithMap();
                            else
                                pausedFrame--;
                        }
                    }

                    if (!LevelManager.Instance.CurrentLevel.IsLevelPaused || LevelManager.Instance.CurrentLevel.LevelEvent == LevelEvent.PlayerDeath)
                        currentAnimation.Update(gameTime);
                } else {
                    if (MovingDirection == Direction.Left)
                        Position.X -= CurrentSpeed;
                    else
                        Position.X += CurrentSpeed;
                    currentAnimation.Update(gameTime);
                }
            }
        }

        private void TrySetPendingDirection() {
            if (InputManager.Instance.KeyPressed(Keys.Left))
                pendingDirection = Direction.Left;
            else if (InputManager.Instance.KeyPressed(Keys.Right))
                pendingDirection = Direction.Right;
            else if (InputManager.Instance.KeyPressed(Keys.Up)
                && LevelManager.Instance.CurrentLevel.LevelEvent != LevelEvent.LevelIntro)
                pendingDirection = Direction.Up;
            else if (InputManager.Instance.KeyPressed(Keys.Down)
                && LevelManager.Instance.CurrentLevel.LevelEvent != LevelEvent.LevelIntro)
                pendingDirection = Direction.Down;
        }

        private void InteractWithMap() {
            if (LevelManager.Instance.CurrentLevel.IsPowered)
                CurrentSpeed = _fullSpeed * poweredSpeedModifier;
            else
                CurrentSpeed = _fullSpeed * normalSpeedModifier;
            desiredMovingDistance = CurrentSpeed;

            while (desiredMovingDistance != 0) {
                UpdateCheckingTileInfo();
                TryEat();
                if (!isTurning) {
                    UpdateTurnableInfo();
                        if ((turnable && pendingDirection != MovingDirection) || MovingDirection == Direction.Null) {
                        if (MovingDirection == Direction.Left && pendingDirection == Direction.Right
                            || MovingDirection == Direction.Right && pendingDirection == Direction.Left
                            || MovingDirection == Direction.Up && pendingDirection == Direction.Down
                            || MovingDirection == Direction.Down && pendingDirection == Direction.Up
                            || MovingDirection == Direction.Null) {

                            MovingDirection = pendingDirection;
                            if (MovingDirection == Direction.Up)
                                currentAnimation = FaceUp;
                            else if (MovingDirection == Direction.Down)
                                currentAnimation = FaceDown;
                            else if (MovingDirection == Direction.Left)
                                currentAnimation = FaceLeft;
                            else if (MovingDirection == Direction.Right)
                                currentAnimation = FaceRight;
                        } else
                            isTurning = true;
                    } else {
                        TryMove(false);
                    }
                }

                if (isTurning) {
                    if (distanceFromMovingTile == 0) {
                        MovingDirection = pendingDirection;
                        isTurning = false;
                        if (MovingDirection == Direction.Up)
                            currentAnimation = FaceUp;
                        else if (MovingDirection == Direction.Down)
                            currentAnimation = FaceDown;
                        else if (MovingDirection == Direction.Left)
                            currentAnimation = FaceLeft;
                        else if (MovingDirection == Direction.Right)
                            currentAnimation = FaceRight;
                    } else {
                        TryMove(true);
                    }
                }
            }
        }

        private void UpdateTurnableInfo() {
            //System.Diagnostics.Debug.WriteLine(positionInCurrentTile);
            turnable = false;
            if (pendingTile.TileType != TileType.Wall && pendingTile.TileType != TileType.GhostEntrance) {
                turnable = true;

                if (MovingDirection == Direction.Left) {
                    if (positionInCurrentTile.X >= 4)
                        preturn = true;
                    else
                        preturn = false;
                } else if (MovingDirection == Direction.Right) {
                    if (positionInCurrentTile.X < 4)
                        preturn = true;
                    else
                        preturn = false;
                } else if (MovingDirection == Direction.Up) {
                    if (positionInCurrentTile.Y >= 4)
                        preturn = true;
                    else
                        preturn = false;
                } else if (MovingDirection == Direction.Down) {
                    if (positionInCurrentTile.Y < 4)
                        preturn = true;
                    else
                        preturn = false;
                }
            }
        }

        private void TryEat() {
            if (currentTile.TileType == TileType.Pellet || currentTile.TileType == TileType.PowerPellet) {
                if (oddNumberEat == true) {
                    AudioManager.Instance.PlaySoundEffect("munch_1");
                    oddNumberEat = false;
                } else {
                    AudioManager.Instance.PlaySoundEffect("munch_2");
                    oddNumberEat = true;
                }

                if (currentTile.TileType == TileType.Pellet) {
                    LevelManager.Instance.AddToScore(10);
                    pausedFrame = 1;
                } else if (currentTile.TileType == TileType.PowerPellet) {
                    LevelManager.Instance.AddToScore(50);
                    LevelManager.Instance.CurrentLevel.IsPowered = true;
                    LevelManager.Instance.CurrentLevel.PoweredStage = 0;
                    LevelManager.Instance.CurrentLevel.ManagePlayerStates(new GameTime());
                    LevelManager.Instance.CurrentLevel.ManageGhostStates(new GameTime());
                    pausedFrame = 3;
                }
                currentTile.SwitchTileType("-");
                LevelManager.Instance.CurrentLevel.EatenDotCount += 1;
                if (LevelManager.Instance.CurrentLevel.EatenDotCount == 70 || LevelManager.Instance.CurrentLevel.EatenDotCount == 170)
                    LevelManager.Instance.CurrentLevel.Fruit.Initialize(LevelManager.Instance.CurrentLevel._FruitNumber);

                if (LevelManager.Instance.CurrentLevel.UsePersonalCounter 
                    && LevelManager.Instance.CurrentLevel.housedGhosts.Count != 0)
                    LevelManager.Instance.CurrentLevel.personalDotCount += 1;
                if (!LevelManager.Instance.CurrentLevel.UsePersonalCounter)
                    LevelManager.Instance.CurrentLevel.globalDotCount += 1;
                LevelManager.Instance.CurrentLevel.personalTime = 0;
                if (LevelManager.Instance.CurrentLevel.EatenDotCount == 244)
                    LevelManager.Instance.CurrentLevel.LevelEvent = LevelEvent.ClearLevel;
            } else if (LevelManager.Instance.CurrentLevel.Fruit.IsAvailable) {
                if (Position.X == LevelManager.Instance.CurrentLevel.Map.GetPlayerStartTile().X * 8 + 4
                    && Position.Y == (LevelManager.Instance.CurrentLevel.Map.GetPlayerStartTile().Y - 6) * 8 + 4) {
                    LevelManager.Instance.CurrentLevel.Fruit.GetEaten();
                    AudioManager.Instance.PlaySoundEffect("eat_fruit");
                }
            }
        }

        protected void TryMove(bool isTurning) {
            if (!isTurning) {
                if (movingTile.TileType != TileType.Wall &&
                movingTile.TileType != TileType.GhostEntrance || distanceFromMovingTile != 0) {
                    if (CurrentSpeed > Math.Abs(distanceFromMovingTile - 4) && distanceFromMovingTile != 4)
                        movingDistance = Math.Abs(distanceFromMovingTile - 4);
                    else if (desiredMovingDistance > distanceFromMovingTile && distanceFromMovingTile != 0)
                        movingDistance = distanceFromMovingTile;
                    else
                        movingDistance = desiredMovingDistance;

                    if (MovingDirection == Direction.Up)
                        Position.Y -= movingDistance;
                    else if (MovingDirection == Direction.Down)
                        Position.Y += movingDistance;
                    else if (MovingDirection == Direction.Left)
                        Position.X -= movingDistance;
                    else if (MovingDirection == Direction.Right)
                        Position.X += movingDistance;
                    desiredMovingDistance -= movingDistance;
                    currentAnimation.AnimationPaused = false;
                } else {
                    desiredMovingDistance = 0;
                    currentAnimation.AnimationPaused = true;
                }
            } else {
                if (distanceFromMovingTile != 0) {
                    if (desiredMovingDistance > distanceFromMovingTile && distanceFromMovingTile != 0)
                        movingDistance = distanceFromMovingTile;
                    else {
                        movingDistance = desiredMovingDistance;
                    }

                    if (pendingDirection == Direction.Up)
                        Position.Y -= movingDistance;
                    else if (pendingDirection == Direction.Down)
                        Position.Y += movingDistance;
                    else if (pendingDirection == Direction.Left)
                        Position.X -= movingDistance;
                    else if (pendingDirection == Direction.Right)
                        Position.X += movingDistance;

                    if (preturn) {
                        if (MovingDirection == Direction.Up)
                            Position.Y -= movingDistance;
                        else if (MovingDirection == Direction.Down)
                            Position.Y += movingDistance;
                        else if (MovingDirection == Direction.Left)
                            Position.X -= movingDistance;
                        else if (MovingDirection == Direction.Right)
                            Position.X += movingDistance;
                    } else {
                        if (MovingDirection == Direction.Up)
                            Position.Y += movingDistance;
                        else if (MovingDirection == Direction.Down)
                            Position.Y -= movingDistance;
                        else if (MovingDirection == Direction.Left)
                            Position.X += movingDistance;
                        else if (MovingDirection == Direction.Right)
                            Position.X -= movingDistance;
                    }

                    desiredMovingDistance -= movingDistance;
                }
            }

            if (Position.X < 0)
                Position.X = LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X * 8 + Position.X;
            else if (Position.X > LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X * 8)
                Position.X -= LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X * 8;
            if (Position.Y < 0)
                Position.Y = LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y * 8 + Position.Y;
            else if (Position.Y > LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y * 8)
                Position.Y -= LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y * 8;
        }

        public void ForceSwitchAnimation(string animationName) {
            if (animationName == "Default")
                currentAnimation = Default;
            else if (animationName == "Death") {
                currentAnimation = Death;
                Death.ResetAnimation();
            } else if (animationName == "Giant") {
                currentAnimation = Giant;
            }
        }

        public void Draw() {
            if (IsShown) {
                if (currentAnimation != Giant) {
                    ScreenManager.Instance.Draw("Pacman", currentAnimation.GetCurrentFrameRectangle(),
                                new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16));
                } else {
                    ScreenManager.Instance.Draw("Pacman", currentAnimation.GetCurrentFrameRectangle(),
                                new Rectangle((int)Position.X - 24, (int)Position.Y - 24, 32, 32));
                }
                
                if (LevelManager.Debug)
                    ScreenManager.Instance.DrawRectangle(new Rectangle(currentTilePosition.X * 8, currentTilePosition.Y * 8, 8, 8), Color.Yellow, 0.5f);
            }
        }
    }
}