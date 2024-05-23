namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    class Intermission
    {
        private Animation Knot = new Animation(16, 500);

        public int IntermissionIndex;
        public int StateIndex = 1;
        public double StateAge = 0;

        public Player Player;
        public Blinky Blinky;
        public FruitCounter FruitCounter;

        public bool GotoNextLevel = false;

        public Intermission(int intermissionIndex) {
            IntermissionIndex = intermissionIndex;
            Player = new Player();
            Blinky = new Blinky();
            FruitCounter = new FruitCounter();
        }

        public void LoadContent(ContentManager content) {
            Player.LoadContent(content);
            Blinky.LoadContent(content);
            FruitCounter.LoadContent();
            FruitCounter.Initialize();
            Knot.AddFrame(new Point(8, 2), new Point(9, 2), new Point(10, 2), new Point(11, 2), new Point(12, 2));
        }

        public void UnloadContent() {
            AudioManager.Instance.RemoveAllSingletonSoundEffect();
            Player.UnloadContent();
            Blinky.UnloadContent();
        }

        public void Update(GameTime gameTime) {
            AudioManager.Instance.PlaySingletonSoundEffect("intermission");

            if (InputManager.Instance.KeyPressed(Keys.L))
                GotoNextLevel = true;
            ManageIntermissionStates(gameTime);

            Player.Update(gameTime);
            Blinky.Update(gameTime);
            Knot.Update(gameTime);
        }

        private void ManageIntermissionStates(GameTime gameTime) {
            StateAge += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (IntermissionIndex == 1) {
                if (StateIndex== 1) {
                    System.Diagnostics.Debug.WriteLine("Intermission Started! - Intermission 1");
                    Player.Initialize();
                    Blinky.Initialize();

                    StateIndex = 2;
                } else if (StateIndex == 2 && StateAge >= 4000) {
                    StateIndex = 3;
                    Player.Position.X = -64 - 256;
                    Player.ForceSwitchAnimation("Giant");
                    Player.MovingDirection = Direction.Right;
                    Player.CurrentSpeed = Player._fullSpeed * 1.2f;
                    Blinky.Position.X = -64;
                    Blinky.MovingDirection = Direction.Right;
                    Blinky.CurrentSpeed = Blinky._fullSpeed * 0.7f;
                    Blinky.ForceSwitchAnimation("Frightened");
                } else if (StateIndex == 3 && StateAge >= 10000) {
                    GotoNextLevel = true;
                }
            } else if (IntermissionIndex == 2) {
                if (StateIndex == 1) {
                    System.Diagnostics.Debug.WriteLine("Intermission Started! - Intermission 2");
                    Player.Initialize();
                    Blinky.Initialize();
                    Knot.AnimationPaused = true;
                    StateIndex = 2;
                } else if (StateIndex == 2 && StateAge >= 2800) {
                    Blinky.CurrentSpeed = Blinky._fullSpeed * 0.05f;
                    Blinky.ForceSwitchAnimation("SlowLeft");
                    Knot.CurrentFrameIndex = 1;
                    Knot.AnimationPaused = false;
                    StateIndex = 3;
                } else if (StateIndex == 3 && StateAge >= 4000) {
                    Blinky.IsMovable = false;
                    Blinky.ForceSwitchAnimation("Stop");
                    Knot.AnimationPaused = true;
                    
                    StateIndex = 4;
                } else if (StateIndex == 4 && StateAge >= 5000) {
                    Blinky.ForceSwitchAnimation("LegExposed");
                    Knot.CurrentFrameIndex = 4;

                    StateIndex = 5;
                } else if (StateIndex == 5 && StateAge >= 10000) {
                    GotoNextLevel = true;
                }
            } else if (IntermissionIndex == 3) {
                if (StateIndex == 1) {
                    System.Diagnostics.Debug.WriteLine("Intermission Started! - Intermission 3");
                    Player.Initialize();
                    Blinky.Initialize();

                    StateIndex = 2;
                } else if (StateIndex == 2 && StateAge >= 6000) {
                    StateIndex = 3;
                    Blinky.Position.X = -16;
                    Blinky.MovingDirection = Direction.Right;
                    Blinky.CurrentSpeed = Blinky._fullSpeed * 0.8f;
                    Blinky.ForceSwitchAnimation("NakedRight");
                } else if (StateIndex == 3 && StateAge >= 10000) {
                    GotoNextLevel = true;
                }
            }
        }

        public void Draw() {
            if (IntermissionIndex == 2)
                ScreenManager.Instance.Draw("Ghosts", Knot.GetCurrentFrameRectangle(),
                    new Rectangle(ScreenManager.Instance.WindowDimensions.X / 2 - 8,
                    ScreenManager.Instance.WindowDimensions.Y / 2 - 9, 16, 16));
            Player.Draw();
            Blinky.Draw();
            FruitCounter.Draw();
        }
    }
}
