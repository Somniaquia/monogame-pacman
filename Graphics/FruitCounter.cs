namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class FruitCounter
    {
        private List<Animation> counterAnimations = new List<Animation>();
        private int levelIndex;
        private Animation currentAnimation;

        private Vector2 position;

        public void LoadContent() {
            for (int i = 0; i < 20; i++) {
                Animation counterAnimation = new Animation(new Point(16 * 7, 16));
                counterAnimation.AddFrame(new Point(0, i));
                counterAnimations.Add(counterAnimation);
            }
        }

        public void Initialize() {
            position = new Vector2((LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X - 14) * 8,
                (LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y - 2) * 8);
            levelIndex = LevelManager.Instance.LevelIndex;
            if (levelIndex >= 21)
                levelIndex = 20;
            currentAnimation = counterAnimations[levelIndex - 2];
        }

        public void Draw() {
            ScreenManager.Instance.Draw("FruitCounter", currentAnimation.GetCurrentFrameRectangle(),
                new Rectangle((int)position.X, (int)position.Y, 16 * 7, 16));
        }

    }
}
