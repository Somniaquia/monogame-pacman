namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Tilemap
    {
        [XmlElement("TileTypeNames")]
        public string TileTypeNames;
        private List<Tile> tiles;
        public Point TilemapDimensions;
        public bool IsShown { get; set; }

        private ContentManager content;

        public Tilemap() {
            TileTypeNames = @"			 
             -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
			 -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
			 -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
			 1 10 10 10 10 10 10 10 10 10 10 10 10 43 42 10 10 10 10 10 10 10 10 10 10 10 10  0
			 3  .  .  .  .  .  .  .  .  .  .  .  . 25 24  .  .  .  .  .  .  .  .  .  .  .  .  2
			 3  . 23 14 14 22  . 23 14 14 14 22  . 25 24  . 23 14 14 14 22  . 23 14 14 22  .  2
			 3  P 25  -  - 24  . 25  -  -  - 24  . 25 24  . 25  -  -  - 24  . 25  -  - 24  P  2
			 3  . 27 20 20 26  . 27 20 20 20 26  . 27 26  . 27 20 20 20 26  . 27 20 20 26  .  2
			 3  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  2
			 3  . 23 14 14 22  . 23 22  . 23 14 14 14 14 14 14 22  . 23 22  . 23 14 14 22  .  2
			 3  . 27 20 20 26  . 25 24  . 27 20 20 22 23 20 20 26  . 25 24  . 27 20 20 26  .  2
			 3  .  .  .  .  .  . 25 24  .  .  .  . 25 24  .  .  .  . 25 24  .  .  .  .  .  .  2
			 5 12 12 12 12 22  . 25 27 14 14 22  - 25 24  - 23 14 14 26 24  . 23 12 12 12 12  4
			 -  -  -  -  -  3  . 25 23 20 20 26  - 27 26  - 27 20 20 22 24  .  2  -  -  -  -  -
			 -  -  -  -  -  3  . 25 24  -  -  - f-  -  - f-  -  -  - 25 24  .  2  -  -  -  -  -
			 -  -  -  -  -  3  . 25 24  - 29 12 33  =  = 32 12 28  - 25 24  .  2  -  -  -  -  -
			10 10 10 10 10 26  . 27 26  -  2  -  -  -  -  -  -  3  - 27 26  . 27 10 10 10 10 10
			s- s- s- s- s- s-  .  -  -  -  2  -  -  -  -  -  -  3  -  -  -  . s- s- s- s- s- s-
			12 12 12 12 12 22  . 23 22  -  2  -  -  -  -  -  -  3  - 23 22  . 23 12 12 12 12 12
			 -  -  -  -  -  3  . 25 24  - 31 10 10 10 10 10 10 30  - 25 24  .  2  -  -  -  -  -
			 -  -  -  -  -  3  . 25 24  -  -  -  -  -  -  -  -  -  - 25 24  .  2  -  -  -  -  -
			 -  -  -  -  -  3  . 25 24  - 23 14 14 14 14 14 14 22  - 25 24  .  2  -  -  -  -  -
			 1 10 10 10 10 26  . 27 26  - 27 20 20 22 23 20 20 26  - 27 26  . 27 10 10 10 10  0
			 3  .  .  .  .  .  .  .  .  .  .  .  . 25 24  .  .  .  .  .  .  .  .  .  .  .  .  2
			 3  . 23 14 14 22  . 23 14 14 14 22  . 25 24  . 23 14 14 14 22  . 23 14 14 22  .  2
			 3  . 27 20 22 24  . 27 20 20 20 26  . 27 26  . 27 20 20 20 26  . 25 23 20 26  .  2
			 3  P  .  . 25 24  .  .  .  .  .  . f.  -  - f.  .  .  .  .  .  . 25 24  .  .  P  2
			 7 14 22  . 25 24  . 23 22  . 23 14 14 14 14 14 14 22  . 23 22  . 25 24  . 23 14  6
			 9 20 26  . 27 26  . 25 24  . 27 20 20 22 23 20 20 26  . 25 24  . 27 26  . 27 20  8
			 3  .  .  .  .  .  . 25 24  .  .  .  . 25 24  .  .  .  . 25 24  .  .  .  .  .  .  2
			 3  . 23 14 14 14 14 26 27 14 14 22  . 25 24  . 23 14 14 26 27 14 14 14 14 22  .  2
			 3  . 27 20 20 20 20 20 20 20 20 26  . 27 26  . 27 20 20 20 20 20 20 20 20 26  .  2
			 3  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  2
			 5 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12 12  4
			 -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
			 -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -";
            tiles = new List<Tile>();
            TilemapDimensions = new Point(28, 36);
            IsShown = true;
        }

        public void LoadContent(ContentManager content) {
            this.content = content;
            for (int y = 0; y < TilemapDimensions.Y; y++) {

                string currentRowString = TileTypeNames.TrimStart().Split("\n")[y];
                string[] dataInRows = currentRowString.TrimEnd(new[] { '\r', ' ' }).TrimStart(new[] { '\t' }).Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < TilemapDimensions.X; x++) {
                    Tile tile = new Tile(dataInRows[x], new Point(x, y));
                    tiles.Add(tile);
                }
            }

            foreach (Tile tile in tiles) {
                tile.LoadContent(content);
            }

            for (int y = 0; y < TilemapDimensions.Y; y++) {
                for (int x = 0; x < TilemapDimensions.X; x++) {
                    Tile currentTile = GetTile(new Point(x, y));
                    if (currentTile.TileType != TileType.Wall && currentTile.TileType != TileType.GhostEntrance) {
                        Tile upTile, downTile, leftTile, rightTile;

                        if (y == 0)
                            upTile = GetTile(new Point(x, TilemapDimensions.Y - 1));
                        else
                            upTile = GetTile(new Point(x, y - 1));

                        if (y == TilemapDimensions.Y - 1)
                            downTile = GetTile(new Point(x, 0));
                        else
                            downTile = GetTile(new Point(x, y + 1));

                        if (x == 0)
                            leftTile = GetTile(new Point(TilemapDimensions.X - 1, y));
                        else
                            leftTile = GetTile(new Point(x - 1, y));

                        if (x == TilemapDimensions.X - 1)
                            rightTile = GetTile(new Point(0, y));
                        else
                            rightTile = GetTile(new Point(x + 1, y));

                        if (upTile.TileType != TileType.Wall && upTile.TileType != TileType.GhostEntrance)
                            currentTile.ConnectedUp = true;
                        if (downTile.TileType != TileType.Wall && downTile.TileType != TileType.GhostEntrance)
                            currentTile.ConnectedDown = true;
                        if (leftTile.TileType != TileType.Wall && leftTile.TileType != TileType.GhostEntrance)
                            currentTile.ConnectedLeft = true;
                        if (rightTile.TileType != TileType.Wall && rightTile.TileType != TileType.GhostEntrance)
                            currentTile.ConnectedRight = true;
                    }
                }
            }
        }

        public void UnloadContent() {
            foreach (Tile tile in tiles) {
                tile.UnloadContent();
            }
        }

        public void Update(GameTime gameTime) {
            foreach (Tile tile in tiles) {
                tile.Update(gameTime);
            }
        }

        public void Draw() {
            if (IsShown) {
                foreach (Tile tile in tiles) {
                    tile.Draw();
                }
            }
        }

        public void Flash() {
            foreach (Tile tile in tiles) {
                if (tile.TileType == TileType.Wall) {
                    tile.Flash();
                }
            }
        }

        public Point GetTilePositionByPosition(Vector2 pixelPosition) {
            return new Point(((int)pixelPosition.X) / 8, ((int)pixelPosition.Y) / 8);
        }

        public Tile GetTile(Point position) {
            return tiles[GetTileIndex(position)];
        }

        public Vector2 GetPlayerStartTile() {
            return new Vector2(13.5f, 26);
        }

        public Vector2 GetGhostHouseEntranceTile() {
            return new Vector2(13.5f, 14);
        }

        private int GetTileIndex(Point position) {
            if (position.X == -1)
                return position.Y * (TilemapDimensions.X + 1) - 1;
            else if (position.X == TilemapDimensions.X)
                return position.Y * TilemapDimensions.X;
            return position.Y * TilemapDimensions.X + position.X;
        }
    }
}
