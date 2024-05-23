namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Inky : Ghost
    {
        public override void LoadContent(ContentManager content) {
            FaceUp.AddFrame(new Point(4, 2), new Point(5, 2));
            FaceDown.AddFrame(new Point(6, 2), new Point(7, 2));
            FaceLeft.AddFrame(new Point(2, 2), new Point(3, 2));
            FaceRight.AddFrame(new Point(0, 2), new Point(1, 2));
            ghostColor = Color.Cyan;

            homeTilePosition = LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile();
            homeTilePosition.Y += 3;
            homeTilePosition.X -= 2;
            scatteringTilePosition = new Vector2(LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X - 1,
                LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y - 1);

            base.LoadContent(content);
        }

        protected override void SetTargetTile() {
            Point playerPosition = LevelManager.Instance.CurrentLevel.Player.currentTilePosition;
            Direction playerDirection = LevelManager.Instance.CurrentLevel.Player.MovingDirection;

            Vector2 offsetPositionVector = Vector2.Zero;
            if (playerDirection == Direction.Up)
                offsetPositionVector = new Vector2(playerPosition.X - 2, playerPosition.Y - 2);
            else if (playerDirection == Direction.Down)
                offsetPositionVector = new Vector2(playerPosition.X, playerPosition.Y + 2);
            else if (playerDirection == Direction.Left)
                offsetPositionVector = new Vector2(playerPosition.X - 2, playerPosition.Y);
            else if (playerDirection == Direction.Right)
                offsetPositionVector = new Vector2(playerPosition.X + 2, playerPosition.Y);

            Point blinkyPosition = LevelManager.Instance.CurrentLevel.Blinky.currentTilePosition;
            Vector2 blinkyPositionVector = new Vector2(blinkyPosition.X, blinkyPosition.Y);

            Vector2 targetTilePositionVector = offsetPositionVector - (blinkyPositionVector - offsetPositionVector);
            targetTilePosition = new Vector2((int)targetTilePositionVector.X, (int)targetTilePositionVector.Y);
        }
    }
}
