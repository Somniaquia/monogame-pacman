namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class TextObject
    {
        public bool IsShown;
        private double shownAge;
        public string Text;
        private Vector2 origin;
        private Dictionary<char, Point> Letters = new Dictionary<char, Point>();
        private double flashLength = 250;

        public void LoadContent() {
            
        }

        public void Initialize(Vector2 originInTiles, string Text, Color Color) {
            origin = originInTiles * 8;
            this.Text = Text;
            IsShown = true;
            shownAge = 0;
            int offset = 0;
            if (Color == Color.White)
                offset = 0;
            else if (Color == Color.Red)
                offset = 1;
            else if (Color == Color.Pink)
                offset = 2;
            else if (Color == Color.Cyan)
                offset = 3;
            else if (Color == Color.Orange)
                offset = 4;
            else if (Color == Color.PeachPuff)
                offset = 5;
            else if (Color == Color.Yellow)
                offset = 6;
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Letters.Clear();
            for (int i = 0; i < 15; i++)
                Letters.Add(letters[i], new Point(i, offset * 4));
            for (int i = 0; i < 11; i++)
                Letters.Add(letters[i + 15], new Point(i, 1 + offset * 4));
            for (int i = 0; i < 10; i++)
                Letters.Add(i.ToString().ToCharArray()[0], new Point(i, 2 + offset * 4));
            Letters.Add(' ', new Point(12, 1 + offset * 4));
            Letters.Add('!', new Point(11, 1 + offset * 4));
        }

        public void Initialize(Vector2 originInTiles, string Text, Color Color, double flashLength) {
            origin = originInTiles * 8;
            this.Text = Text;
            IsShown = true;
            shownAge = 0;
            int offset = 0;
            if (Color == Color.White)
                offset = 0;
            else if (Color == Color.Red)
                offset = 1;
            else if (Color == Color.Pink)
                offset = 2;
            else if (Color == Color.Cyan)
                offset = 3;
            else if (Color == Color.Orange)
                offset = 4;
            else if (Color == Color.PeachPuff)
                offset = 5;
            else if (Color == Color.Yellow)
                offset = 6;
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Letters.Clear();
            for (int i = 0; i < 15; i++)
                Letters.Add(letters[i], new Point(i, offset * 4));
            for (int i = 0; i < 11; i++)
                Letters.Add(letters[i + 15], new Point(i, 1 + offset * 4));
            for (int i = 0; i < 10; i++)
                Letters.Add(i.ToString().ToCharArray()[0], new Point(i, 2 + offset * 4));
            Letters.Add(' ', new Point(12, 1 + offset * 4));
            Letters.Add('!', new Point(11, 1 + offset * 4));
            this.flashLength = flashLength;
        }

        public void Update(GameTime gameTime) {
            shownAge += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (shownAge >= flashLength) {
                shownAge = 0;
                if (IsShown)
                    IsShown = false;
                else
                    IsShown = true;
            }
        }

        public void Draw() {
            if (IsShown) {
                int firstLetterOffset = -(Text.Length - 1) * 4;

                for (int i = 0; i < Text.Length; i++) {
                    Point currentFramePosition = Letters[Text.ToCharArray()[i]];
                    ScreenManager.Instance.Draw("Text", new Rectangle(currentFramePosition.X * 8, currentFramePosition.Y * 8, 8, 8),
                        new Rectangle((int)origin.X + firstLetterOffset + i * 8, (int)origin.Y, 8, 8));
                }
            }
        }
    }
}
