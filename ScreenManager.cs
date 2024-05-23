namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    class ScreenManager
    {
        private static ScreenManager instance;
        public static ScreenManager Instance {
            get {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        }

        public Point WindowDimensions { get; set; }
        public int Scale { get; set; }

        public GraphicsDevice GraphicsDevice { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        ContentManager content;

        Dictionary<string, Texture2D> spriteSheets = new Dictionary<string, Texture2D>();
        Texture2D emptyTexture;

        public ScreenManager() {
            WindowDimensions = new Point(510, 510);
            Scale = 2;
        }

        public void LoadContent(ContentManager content) {
            this.content = content;
        }

        private void AddSpriteSheet(string spriteSheetName) {
            Texture2D spriteSheet = content.Load<Texture2D>("Textures/" + spriteSheetName);
            spriteSheets.Add(spriteSheetName, spriteSheet);
        }

        public void Draw(string spriteSheetName, Rectangle sourceRectangle, Rectangle destinationRectangle) {
            Rectangle resizedDestinationRectangle =
                new Rectangle((destinationRectangle.X * Scale), (destinationRectangle.Y * Scale),
                (destinationRectangle.Width * Scale), (destinationRectangle.Height * Scale));
            SpriteBatch.Draw(spriteSheets[spriteSheetName], resizedDestinationRectangle, sourceRectangle, Color.White);
        }

        public void Draw(string spriteSheetName, Rectangle sourceRectangle, Rectangle destinationRectangle, Color color) {
            Rectangle resizedDestinationRectangle =
                new Rectangle((destinationRectangle.X * Scale), (destinationRectangle.Y * Scale),
                (destinationRectangle.Width * Scale), (destinationRectangle.Height * Scale));
            SpriteBatch.Draw(spriteSheets[spriteSheetName], resizedDestinationRectangle, sourceRectangle, color);
        }

        public void DrawRectangle(Rectangle destinationRectangle, Color color, float opacity) {
            emptyTexture = new Texture2D(GraphicsDevice, 1, 1);
            emptyTexture.SetData(new Color[] { color });
            Rectangle resizedDestinationRectangle =
                new Rectangle((destinationRectangle.X * Scale), (destinationRectangle.Y * Scale),
                (destinationRectangle.Width * Scale), (destinationRectangle.Height * Scale));
            SpriteBatch.Draw(emptyTexture, resizedDestinationRectangle, color * opacity);
        }
    }
}