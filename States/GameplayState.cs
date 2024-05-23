namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    class GameplayState : State
    {
        private bool isGameForcePaused = false;

        public override void LoadContent(ContentManager content) {
            this.content = content;
            LevelManager.ResetInstance();
            LevelManager.Instance.LoadNextLevel();
            LevelManager.Instance.CurrentLevel.ShortenIntro = false;
        }

        public override void UnloadContent() {
            LevelManager.Instance.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            if (InputManager.Instance.KeyPressed(Keys.Space)) {
                if (isGameForcePaused)
                    isGameForcePaused = false;
                else {
                    isGameForcePaused = true;
                    AudioManager.Instance.RemoveAllSingletonSoundEffect();
                }
            }

            if (!isGameForcePaused)
                LevelManager.Instance.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            LevelManager.Instance.Draw();
        }
    }
}
