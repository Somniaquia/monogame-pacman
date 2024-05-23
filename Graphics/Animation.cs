namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class Animation
    {
        private List<Point> framePositions = new List<Point>();
        public Point animationFrameDimension;
        private int animationFrameSeperatorLength = 0;

        public bool AnimationPaused { get; set; }
        public int CurrentFrameIndex;
        private double currentFrameAge = 0;
        private double frameDuration = 50;

        public Animation(int animationFrameLength) {
            animationFrameDimension = new Point(animationFrameLength, animationFrameLength);
            AnimationPaused = false;
        }

        public Animation(Point animationFrameDimension) {
            this.animationFrameDimension = animationFrameDimension;
            AnimationPaused = true;
        }

        public Animation(int animationFrameLength, double frameDuration) {
            animationFrameDimension = new Point(animationFrameLength, animationFrameLength);
            this.frameDuration = frameDuration;
            AnimationPaused = false;
        }

        public Animation(Point animationFrameDimension, double frameDuration) {
            this.animationFrameDimension = animationFrameDimension;
            this.frameDuration = frameDuration;
            AnimationPaused = true;
        }

        // Used to tiles
        public Animation(int animationFrameLength, double frameDuration, int animationFrameSeperatorLength) {
            animationFrameDimension = new Point(animationFrameLength, animationFrameLength);
            this.frameDuration = frameDuration;
            this.animationFrameSeperatorLength = animationFrameSeperatorLength;
            AnimationPaused = false;
        }

        public void AddFrame(params Point[] positionsInSpriteSheet) {
            foreach (Point position in positionsInSpriteSheet) {
                framePositions.Add(position);
            }
        }

        public void ResetAnimation() {
            currentFrameAge = 0;
            CurrentFrameIndex = 0;
        }

        public void Update(GameTime gameTime) {
            if (!AnimationPaused) {
                currentFrameAge += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (currentFrameAge >= frameDuration) {
                    currentFrameAge = 0;
                    NextFrame();
                }
            }
        }

        public Rectangle GetCurrentFrameRectangle() {
            Point currentFrame = framePositions[CurrentFrameIndex];
            return new Rectangle(currentFrame.X * (animationFrameDimension.X + animationFrameSeperatorLength),
                currentFrame.Y * (animationFrameDimension.Y + animationFrameSeperatorLength),
                animationFrameDimension.X, animationFrameDimension.Y);
        }

        private void NextFrame() {
            if (CurrentFrameIndex < framePositions.Count - 1) {
                CurrentFrameIndex++;
            } else {
                CurrentFrameIndex = 0;
            }
        }
    }
}
