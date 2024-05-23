namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Blinky : Ghost
    {
        private Animation SlowLeft = new Animation(16, 200);
        private Animation LegExposed = new Animation(16, 2000);
        private Animation PatchedLeft = new Animation(16, 100);
        private Animation NakedRight = new Animation(new Point(32, 16), 100);

        public bool ElroyPossible;
        public int ElroyState;

        private float elroy1SpeedModifier;
        private float elroy2SpeedModifier;

        public override void Initialize() {
            base.Initialize();
            ElroyPossible = false;
            ElroyState = 0;
            elroy1SpeedModifier = normalSpeedModifier + 0.05f;
            elroy2SpeedModifier = elroy1SpeedModifier + 0.05f;
        }

        public override void LoadContent(ContentManager content) {
            FaceUp.AddFrame(new Point(4, 0), new Point(5, 0));
            FaceDown.AddFrame(new Point(6, 0), new Point(7, 0));
            FaceLeft.AddFrame(new Point(2, 0), new Point(3, 0));
            FaceRight.AddFrame(new Point(0, 0), new Point(1, 0));
            SlowLeft.AddFrame(new Point(2, 0), new Point(3, 0));
            LegExposed.AddFrame(new Point(8, 3), new Point(9, 3));
            PatchedLeft.AddFrame(new Point(10, 3), new Point(11, 3));
            NakedRight.AddFrame(new Point(4, 4), new Point(5, 4));
            NakedRight.AnimationPaused = false;
            ghostColor = Color.Red;

            homeTilePosition = LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile();
            scatteringTilePosition = new Vector2(LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X - 3, 0);

            base.LoadContent(content);
        }

        protected override void ChangeMoveSpeed(string speedModifierName) {
            float speedModifier = 0;
            if (speedModifierName == "Normal") {
                if (ElroyState == 0)
                    speedModifier = normalSpeedModifier;
                else if (ElroyState == 1)
                    speedModifier = elroy1SpeedModifier;
                else if (ElroyState == 2)
                    speedModifier = elroy2SpeedModifier;
            } else if (speedModifierName == "Frightened") {
                speedModifier = frightenedSpeedModifier;
            } else if (speedModifierName == "Tunnel") {
                speedModifier = tunnelSpeedModifier;
            } else if (speedModifierName == "Retreating") {
                speedModifier = 3f;
            } else
                System.Diagnostics.Debug.WriteLine("Speedmodifier name not available: " + speedModifierName);

            if (CurrentSpeed != _fullSpeed * speedModifier) {
                CurrentSpeed = _fullSpeed * speedModifier;
            }
        }

        protected override void SetTargetTile() {
            Point playerPosition = LevelManager.Instance.CurrentLevel.Player.currentTilePosition;
            if (ghostState == GhostState.Chase || ElroyState != 0)
                targetTilePosition = new Vector2(playerPosition.X, playerPosition.Y);
            else
                targetTilePosition = scatteringTilePosition;
        }

        public override void ForceSwitchAnimation(string animationName) {
            base.ForceSwitchAnimation(animationName);
            if (animationName == "SlowLeft") {
                currentAnimation = SlowLeft;
            } else if (animationName == "LegExposed") {
                currentAnimation = LegExposed;
            } else if (animationName == "PatchedLeft") {
                currentAnimation = PatchedLeft;
            } else if (animationName == "NakedRight") {
                currentAnimation = NakedRight;
            } else if (animationName == "Stop") {
                currentAnimation.AnimationPaused = true;
            }
        }
    }
}
