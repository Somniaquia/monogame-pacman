namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Clyde : Ghost
    {
        public override void LoadContent(ContentManager content) {
            FaceUp.AddFrame(new Point(4, 3), new Point(5, 3));
            FaceDown.AddFrame(new Point(6, 3), new Point(7, 3));
            FaceLeft.AddFrame(new Point(2, 3), new Point(3, 3));
            FaceRight.AddFrame(new Point(0, 3), new Point(1, 3));
            ghostColor = Color.Orange;

            homeTilePosition = LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile();
            homeTilePosition.Y += 3;
            homeTilePosition.X += 2;
            scatteringTilePosition = new Vector2(0, LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y - 1);

            base.LoadContent(content);
        }

        protected override void SetTargetTile() {
            Point playerPosition = LevelManager.Instance.CurrentLevel.Player.currentTilePosition;
            Vector2 playerPositionVector = new Vector2(playerPosition.X, playerPosition.Y);
            Vector2 clydePositionVector = new Vector2(currentTilePosition.X, currentTilePosition.Y);
            if (Vector2.Distance(playerPositionVector, clydePositionVector) > 8) {
                targetTilePosition = playerPositionVector;
            } else {
                targetTilePosition = scatteringTilePosition;
            }
        }
    }
}
