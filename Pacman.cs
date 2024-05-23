namespace Pacman
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class Pacman : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Pacman() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 
                ScreenManager.Instance.WindowDimensions.X * ScreenManager.Instance.Scale;
            graphics.PreferredBackBufferHeight =
                ScreenManager.Instance.WindowDimensions.Y * ScreenManager.Instance.Scale;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            StateManager.Instance.Content = Content;
            ScreenManager.Instance.SpriteBatch = spriteBatch;
            ScreenManager.Instance.GraphicsDevice = graphics.GraphicsDevice;

            ScreenManager.Instance.LoadContent(Content);
            AudioManager.Instance.LoadContent(Content);
            StateManager.Instance.SwitchGameState(new MenuState());


            base.LoadContent();
        }

        protected override void UnloadContent() {
            StateManager.Instance.UnloadContent();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            InputManager.Instance.Update();
            StateManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            StateManager.Instance.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
