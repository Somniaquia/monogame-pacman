namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class LivesCounter
    {
        private List<Animation> counterAnimations = new List<Animation>();
        private int lives;
        private Animation currentAnimation;

        private Vector2 position;

        public void LoadContent() {
            for (int i = 0; i < 5; i++) {
                Animation counterAnimation = new Animation(new Point(16 * 5, 16));
                counterAnimation.AddFrame(new Point(0, i));
                counterAnimations.Add(counterAnimation);
            }
        }

        public void Initialize() {
            position = new Vector2(0, (LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y - 2) * 8);
            lives = LevelManager.Instance.Lives;
            if (lives != 1)
                currentAnimation = counterAnimations[lives - 2];
        }

        public void Draw() {
            if (lives != 1) {
                ScreenManager.Instance.Draw("LifeCounter", currentAnimation.GetCurrentFrameRectangle(),
                    new Rectangle((int)position.X, (int)position.Y, 16 * 5, 16));
            }
        }
    }
}
