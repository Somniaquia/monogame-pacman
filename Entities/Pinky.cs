namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Pinky : Ghost
    {
        public override void LoadContent(ContentManager content) {
            FaceUp.AddFrame(new Point(4, 1), new Point(5, 1));
            FaceDown.AddFrame(new Point(6, 1), new Point(7, 1));
            FaceLeft.AddFrame(new Point(2, 1), new Point(3, 1));
            FaceRight.AddFrame(new Point(0, 1), new Point(1, 1));
            ghostColor = Color.Pink;

            homeTilePosition = LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile();
            homeTilePosition.Y += 3;
            scatteringTilePosition = new Vector2(2, 0);

            base.LoadContent(content);
        }

        protected override void SetTargetTile() {
            Point playerPosition = LevelManager.Instance.CurrentLevel.Player.currentTilePosition;
            Direction playerDirection = LevelManager.Instance.CurrentLevel.Player.MovingDirection;
            if (playerDirection == Direction.Up)
                targetTilePosition = new Vector2(playerPosition.X - 4, playerPosition.Y - 4);
            else if (playerDirection == Direction.Down)
                targetTilePosition = new Vector2(playerPosition.X, playerPosition.Y + 4);
            else if (playerDirection == Direction.Left)
                targetTilePosition = new Vector2(playerPosition.X - 4, playerPosition.Y);
            else if (playerDirection == Direction.Right)
                targetTilePosition = new Vector2(playerPosition.X + 4, playerPosition.Y);

        }
    }
}
