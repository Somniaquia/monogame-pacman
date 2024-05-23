namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    class MenuState : State
    {
        public bool GotoNextState = false;
        public double Timer = 0;
        public Animation TitleScreen = new Animation(new Point(28 * 8, 36 * 8));
        public TextObject InstructionsText = new TextObject();
        public TextObject HighScoreIndicatorText = new TextObject();
        public TextObject HighScoreText = new TextObject();
        public TextObject PressEnterText = new TextObject();
        public TextObject HighScorerName = new TextObject();
        public Player Player = new Player();

        public override void LoadContent(ContentManager content) {
            this.content = content;
            IsMenuState = true;
            TitleScreen.AddFrame(new Point(0, 0));
            Player.LoadContent(content);
            Player.Initialize();
            PressEnterText.LoadContent();
            HighScoreIndicatorText.LoadContent();
            HighScoreText.LoadContent();
            InstructionsText.Initialize(new Vector2(13.5f, 20), "USE ARROW KEYS TO MOVE", Color.White);
            PressEnterText.Initialize(new Vector2(13.5f, 34), "PUSH ENTER BUTTON", Color.Yellow, 500);
            HighScoreIndicatorText.Initialize(new Vector2(13.5f, 0), "HIGH SCORE", Color.White);
            HighScoreText.Initialize(new Vector2(13.5f, 1), LevelManager.HighScore.ToString(), Color.White);
        }

        public override void UnloadContent() {
            
        }

        public override void Update(GameTime gameTime) {
            //AudioManager.Instance.PlaySingletonSoundEffect("intermission");
            if (InputManager.Instance.KeyPressed(Keys.Enter)) {
                AudioManager.Instance.PlaySoundEffect("credit");
                Player.ForceSwitchAnimation("Death");
                GotoNextState = true;
            }
            if (GotoNextState) {
                if (Timer >= 1000)
                    StateManager.Instance.SwitchGameState(new GameplayState());
                else
                    Timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            Player.Update(gameTime);
            PressEnterText.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            ScreenManager.Instance.Draw("TitleScreen", TitleScreen.GetCurrentFrameRectangle(), new Rectangle(0, 0, 28 * 8, 36 * 8));
            InstructionsText.Draw();
            PressEnterText.Draw();
            HighScoreIndicatorText.Draw();
            HighScoreText.Draw();
            Player.Draw();
        }
    }
}