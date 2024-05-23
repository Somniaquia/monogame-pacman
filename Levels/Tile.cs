namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public enum TileType { Blank, Pellet, PowerPellet, Wall, GhostEntrance }

    public class Tile
    {
        private Animation blankAnimation = new Animation(8, 200, 1);
        private Animation pelletAnimation = new Animation(8, 200, 1);
        private Animation powerPelletAnimation = new Animation(8, 200, 1);
        private List<Animation> wallAnimations = new List<Animation>();

        private string tileTypeName;
        private Point tilePosition;
        private Animation tileAnimation;

        public TileType TileType { get; set; }
        public bool UpwardsTurnForbidden { get; set; }
        public bool SlowMoving { get; set; }
        public bool ConnectedUp { get; set; }
        public bool ConnectedDown { get; set; }
        public bool ConnectedLeft { get; set; }
        public bool ConnectedRight { get; set; }

        public Tile(string tileTypeName, Point tilePosition) {
            TileType = TileType.Blank;
            tileAnimation = blankAnimation;
            this.tileTypeName = tileTypeName;
            this.tilePosition = tilePosition;

            UpwardsTurnForbidden = false;
            SlowMoving = false;
            ConnectedUp = false;
            ConnectedDown = false;
            ConnectedLeft = false;
            ConnectedRight = false;
        }

        public void LoadContent(ContentManager content) {
            blankAnimation.AddFrame(new Point(12, 2));
            blankAnimation.AnimationPaused = true;
            pelletAnimation.AddFrame(new Point(13, 2));
            pelletAnimation.AnimationPaused = true;
            powerPelletAnimation.AddFrame(new Point(15, 2), new Point(12, 2));
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 16; x++) {
                    Animation wallPartAnimation = new Animation(8, 200, 1);
                    wallPartAnimation.AnimationPaused = true;
                    wallPartAnimation.AddFrame(new Point(x, y), new Point(x, y + 3));
                    wallAnimations.Add(wallPartAnimation);
                }
            }

            if (tileTypeName == "-") {
                TileType = TileType.Blank;
                tileAnimation = blankAnimation;
            } else if (tileTypeName == "f-") {
                TileType = TileType.Blank;
                tileAnimation = blankAnimation;
                UpwardsTurnForbidden = true;
            } else if (tileTypeName == "s-") {
                TileType = TileType.Blank;
                tileAnimation = blankAnimation;
                SlowMoving = true;
            } else if (tileTypeName == ".") {
                TileType = TileType.Pellet;
                tileAnimation = pelletAnimation;
            } else if (tileTypeName == "f.") {
                TileType = TileType.Pellet;
                tileAnimation = pelletAnimation;
                UpwardsTurnForbidden = true;
            } else if (tileTypeName == "P") {
                TileType = TileType.PowerPellet;
                tileAnimation = powerPelletAnimation;
            } else if (tileTypeName == "=") {
                TileType = TileType.GhostEntrance;
                tileAnimation = wallAnimations[46];
            } else {
                TileType = TileType.Wall;
                tileAnimation = wallAnimations[Convert.ToInt32(tileTypeName)];
            }
        }

        public void UnloadContent() {
            // TODO: Unload Content.
        }

        public void Update(GameTime gameTime) {
            tileAnimation.Update(gameTime);
        }

        public void SwitchTileType(string tileTypeName) {
            if (tileTypeName == "-") {
                TileType = TileType.Blank;
                tileAnimation = blankAnimation;
            } else if (tileTypeName == ".") {
                TileType = TileType.Pellet;
                tileAnimation = pelletAnimation;
            } else if (tileTypeName == "P") {
                TileType = TileType.PowerPellet;
                tileAnimation = powerPelletAnimation;
            }
        }

        public void Flash() {
            tileAnimation.AnimationPaused = false;
        }

        public void Draw() {
            ScreenManager.Instance.Draw("Tiles", tileAnimation.GetCurrentFrameRectangle(), 
                new Rectangle(tilePosition.X * 8, tilePosition.Y * 8, 8, 8));
        }
    }
}