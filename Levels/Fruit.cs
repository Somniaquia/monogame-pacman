namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Fruit
    {
        private Animation fruitAnimation = new Animation(16);
        private Animation shortScores = new Animation(16);
        private Animation longScores = new Animation(new Point(32, 16));

        private Animation currentAnimation;
        private int fruitNumber;

        private double presentDuration;
        private double presentAge;
        public bool IsAvailable;
        protected bool isDisplayingAsScore;
        protected double displayingAsScoreAge;

        private Vector2 position;

        public void LoadContent(ContentManager content) {
            for (int i = 0; i < 8; i++) {
                fruitAnimation.AddFrame(new Point(i, 0));
            }
            shortScores.AddFrame(new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1));
            longScores.AddFrame(new Point(0, 2), new Point(1, 2), new Point(0, 3), new Point(1, 3));
        }

        public void UnloadContent() { }

        public void Initialize(int fruitNumber) {
            this.fruitNumber = fruitNumber;
            IsAvailable = true;
            Random random = new Random();
            presentDuration = 9000 + random.NextDouble() * 1000;
            presentAge = 0;
            isDisplayingAsScore = false;
            displayingAsScoreAge = 1000;
            position = LevelManager.Instance.CurrentLevel.Map.GetPlayerStartTile() * 8;
            position.X += 4;
            position.Y += (8 * -6) + 4;

            currentAnimation = fruitAnimation;
            currentAnimation.CurrentFrameIndex = fruitNumber - 1;
        }

        public void Update(GameTime gameTime) {
            if (!LevelManager.Instance.CurrentLevel.IsLevelPaused) {
                if (isDisplayingAsScore) {
                    if (displayingAsScoreAge >= 0) {
                        displayingAsScoreAge -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    } else {
                        isDisplayingAsScore = false;
                    }
                } else {
                    if (IsAvailable && presentAge >= presentDuration) {
                        IsAvailable = false;
                    } else {
                        presentAge += gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }
            }
        }

        public void GetEaten() {
            if (fruitNumber == 1)
                LevelManager.Instance.AddToScore(100);
            else if (fruitNumber == 2)
                LevelManager.Instance.AddToScore(300);
            else if (fruitNumber == 3)
                LevelManager.Instance.AddToScore(500);
            else if (fruitNumber == 4)
                LevelManager.Instance.AddToScore(700);
            else if (fruitNumber == 5)
                LevelManager.Instance.AddToScore(1000);
            else if (fruitNumber == 6)
                LevelManager.Instance.AddToScore(2000);
            else if (fruitNumber == 7)
                LevelManager.Instance.AddToScore(3000);
            else
                LevelManager.Instance.AddToScore(5000);
            IsAvailable = false;
            isDisplayingAsScore = true;
            displayingAsScoreAge = 1000;

            if (fruitNumber <= 4) {
                currentAnimation = shortScores;
                currentAnimation.CurrentFrameIndex = fruitNumber - 1;
            } else {
                currentAnimation = longScores;
                currentAnimation.CurrentFrameIndex = fruitNumber - 5;
            }
        }

        public void Draw() {
            if (isDisplayingAsScore) {
                if (fruitNumber <= 4) {
                    ScreenManager.Instance.Draw("Scores", currentAnimation.GetCurrentFrameRectangle(),
                        new Rectangle((int)position.X - 8, (int)position.Y - 8, 16, 16));
                } else {
                    ScreenManager.Instance.Draw("Scores", currentAnimation.GetCurrentFrameRectangle(),
                        new Rectangle((int)position.X - 16, (int)position.Y - 8, 32, 16));
                }
            }
            if (IsAvailable) {
                ScreenManager.Instance.Draw("Fruits", currentAnimation.GetCurrentFrameRectangle(),
                    new Rectangle((int)position.X - 8, (int)position.Y - 8, 16, 16));
            }
        }
    }
}
