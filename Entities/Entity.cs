namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public enum Direction { Null, Left, Right, Up, Down }

    public class Entity
    {
        protected Animation currentAnimation;

        public Vector2 Position;
        public Direction MovingDirection;
        protected Direction pendingDirection; 
        public readonly float _fullSpeed = 1.33f;
        public float CurrentSpeed;
        protected float desiredMovingDistance;
        protected float movingDistance;

        public Point currentTilePosition;
        protected Tile currentTile;
        protected Point movingTilePosition;
        protected Tile movingTile;
        protected float distanceFromMovingTile;
        protected Point pendingTilePosition;
        protected Tile pendingTile;
        protected float distanceFromPendingTile;

        protected Vector2 positionInCurrentTile;

        public virtual void Initialize() {
            distanceFromMovingTile = 0;
            distanceFromPendingTile = 0;
        }

        protected void UpdateCheckingTileInfo() {
            currentTilePosition = LevelManager.Instance.CurrentLevel.Map.GetTilePositionByPosition(Position);
            currentTile = LevelManager.Instance.CurrentLevel.Map.GetTile(currentTilePosition);

            positionInCurrentTile.X = Position.X % 8;
            positionInCurrentTile.Y = Position.Y % 8;
            UpdateMovingInfo();
            UpdatePendingInfo();
        }

        private void UpdateMovingInfo() {
            if (MovingDirection == Direction.Up)
                movingTilePosition = new Point(currentTilePosition.X, currentTilePosition.Y - 1);
            else if (MovingDirection == Direction.Down)
                movingTilePosition = new Point(currentTilePosition.X, currentTilePosition.Y + 1);
            else if (MovingDirection == Direction.Left)
                movingTilePosition = new Point(currentTilePosition.X - 1, currentTilePosition.Y);
            else if (MovingDirection == Direction.Right)
                movingTilePosition = new Point(currentTilePosition.X + 1, currentTilePosition.Y);
            movingTile = LevelManager.Instance.CurrentLevel.Map.GetTile(movingTilePosition);

            if (MovingDirection == Direction.Up)
                distanceFromMovingTile = Math.Abs(positionInCurrentTile.Y - 4);
            if (MovingDirection == Direction.Down)
                distanceFromMovingTile = Math.Abs(4 - positionInCurrentTile.Y);
            if (MovingDirection == Direction.Left)
                distanceFromMovingTile = Math.Abs(positionInCurrentTile.X - 4);
            if (MovingDirection == Direction.Right)
                distanceFromMovingTile = Math.Abs(4 - positionInCurrentTile.X);
        }

        private void UpdatePendingInfo() {
            if (pendingDirection == Direction.Up)
                pendingTilePosition = new Point(currentTilePosition.X, currentTilePosition.Y - 1);
            else if (pendingDirection == Direction.Down)
                pendingTilePosition = new Point(currentTilePosition.X, currentTilePosition.Y + 1);
            else if (pendingDirection == Direction.Left)
                pendingTilePosition = new Point(currentTilePosition.X - 1, currentTilePosition.Y);
            else if (pendingDirection == Direction.Right)
                pendingTilePosition = new Point(currentTilePosition.X + 1, currentTilePosition.Y);
            pendingTile = LevelManager.Instance.CurrentLevel.Map.GetTile(pendingTilePosition);

            if (pendingDirection == Direction.Up)
                distanceFromPendingTile = positionInCurrentTile.Y - 4;
            if (pendingDirection == Direction.Down)
                distanceFromPendingTile = 4 - positionInCurrentTile.Y;
            if (pendingDirection == Direction.Left)
                distanceFromPendingTile = positionInCurrentTile.X - 4;
            if (pendingDirection == Direction.Right)
                distanceFromPendingTile = 4 - positionInCurrentTile.X;
        }
    }
}
