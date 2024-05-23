namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    class StateManager {
        private static StateManager instance;
        public static StateManager Instance {
            get {
                if (instance == null)
                    instance = new StateManager();
                return instance;
            }
        }

        public ContentManager Content { get; set; }

        public State CurrentState { get; private set; }

        public void UnloadContent() {
            CurrentState.UnloadContent();
        }

        public void Update(GameTime gameTime) {
            CurrentState.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) {
            CurrentState.Draw(spriteBatch);
        }

        public void SwitchGameState(State state) {
            if (CurrentState != null)
                CurrentState.UnloadContent();
            CurrentState = state;
            System.Diagnostics.Debug.WriteLine("Switching Game State!");
            CurrentState.LoadContent(Content);
            CurrentState.Update(new GameTime());
        }
    }
}
