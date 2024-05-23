namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.IO;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    class LevelManager
    {
        private static LevelManager instance;
        public static LevelManager Instance {
            get {
                if (instance == null)
                    instance = new LevelManager();
                return instance;
            }
        }

        private ContentManager content;

        public bool IsUsingLevel = true;
        public Level CurrentLevel { get; private set; }
        public Intermission CurrentIntermission { get; private set; }
        public int LevelIndex = 1;
        private bool seenIntermission = false;
        public int IntermissionIndex = 1;
        public int Lives = 5;
        public int Score = 0;
        public int next1Up = 10000;
        public static bool Debug = false;
        public static int HighScore;
        public bool SkipDraw = false;

        public static void ResetInstance() {
            instance = new LevelManager();
        }

        public void LoadContent(ContentManager content) {
            this.content = content;
            if (IsUsingLevel)
                CurrentLevel.LoadContent(content);
            else
                CurrentIntermission.LoadContent(content);
        }

        public void UnloadContent() {
            if (IsUsingLevel)
                CurrentLevel.UnloadContent();
            else
                CurrentIntermission.UnloadContent();
        }

        public void Update(GameTime gameTime) {
            if (IsUsingLevel) {
                CurrentLevel.Update(gameTime);
                if (CurrentLevel.GotoNextLevel) {
                    UnloadContent();
                    LoadNextLevel();
                }
            } else {
                CurrentIntermission.Update(gameTime);
                if (CurrentIntermission.GotoNextLevel) {
                    UnloadContent();
                    LoadNextLevel();
                }
            }
        } 

        public void Draw() {
            if (!SkipDraw) {
                if (IsUsingLevel)
                    CurrentLevel.Draw();
                else
                    CurrentIntermission.Draw();
            } else
                SkipDraw = false;
        }

        public void AddToScore(int score) {
            Score += score;
            if (Score >= next1Up) {
                next1Up += 10000;
                if (Lives != 6) {
                    AudioManager.Instance.PlaySoundEffect("extend");
                    Lives++;
                    CurrentLevel.LivesCounter.Initialize();
                }
            }
            if (Score > HighScore)
                HighScore = Score;
        }

        public void LoadNextLevel() {
            System.Diagnostics.Debug.WriteLine("Loading Next Level...");
            if (IsUsingLevel) {
                if (LevelIndex != 1)
                    CurrentLevel.UnloadContent();
                if (!seenIntermission && (LevelIndex == 3 || LevelIndex == 6)) {
                    IsUsingLevel = false;
                    CurrentIntermission = new Intermission(IntermissionIndex);
                    IntermissionIndex++;
                } else if (!seenIntermission && (LevelIndex == 10 || LevelIndex == 14 || LevelIndex == 18)) {
                    IsUsingLevel = false;
                    CurrentIntermission = new Intermission(IntermissionIndex);
                } else {
                    IsUsingLevel = true;
                    CurrentLevel = new Level(LevelIndex);
                    LevelIndex++;
                    seenIntermission = false;
                }
            } else {
                IsUsingLevel = true;
                CurrentIntermission.UnloadContent();
                CurrentLevel = new Level(LevelIndex);
                LevelIndex++;
                seenIntermission = true;
            }

            LoadContent(content);
            SkipDraw = true;
        }
    }
}
