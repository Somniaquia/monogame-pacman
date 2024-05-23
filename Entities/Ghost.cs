namespace Pacman
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public enum GhostState { Chase, Scatter, Frightened, Retreating }
    public enum GhostHouseState { Outside, Entering, Inside, Leaving }

    public abstract class Ghost : Entity
    {
        protected Animation FaceUp = new Animation(16, 100);
        protected Animation FaceDown = new Animation(16, 100);
        protected Animation FaceLeft = new Animation(16, 100);
        protected Animation FaceRight = new Animation(16, 100);
        protected Animation Frightened = new Animation(16, 100);
        protected Animation FrightenedFlash = new Animation(16, 100);
        protected Animation EyesUp = new Animation(16);
        protected Animation EyesDown = new Animation(16);
        protected Animation EyesLeft = new Animation(16);
        protected Animation EyesRight = new Animation(16);
        protected Animation Scores = new Animation(16);

        protected bool isDisplayingAsScore;
        protected double displayingAsScoreAge;

        protected Color ghostColor;
        protected Vector2 homeTilePosition;
        protected Vector2 scatteringTilePosition;

        protected float normalSpeedModifier;
        protected float tunnelSpeedModifier;
        protected float frightenedSpeedModifier;

        protected GhostState ghostState;
        protected GhostHouseState ghostHouseState;
        protected bool leaveToLeft;
        public bool PendingToLeave { get; set; }
        protected Vector2 targetTilePosition;

        protected bool isShown;
        public bool IsMovable;

        public virtual void LoadContent(ContentManager content) {
            Frightened.AddFrame(new Point(8, 0), new Point(9, 0));
            FrightenedFlash.AddFrame(new Point(10, 0), new Point(11, 0), new Point(8, 0), new Point(9, 0));
            EyesUp.AddFrame(new Point(10, 1));
            EyesDown.AddFrame(new Point(11, 1));
            EyesLeft.AddFrame(new Point(9, 1));
            EyesRight.AddFrame(new Point(8, 1));
            Scores.AddFrame(new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0));
            Scores.AnimationPaused = true;
        }

        public override void Initialize() {
            if (LevelManager.Instance.IsUsingLevel) {
                base.Initialize();

                pendingDirection = Direction.Null;
                ghostState = GhostState.Scatter;
                leaveToLeft = true;
                PendingToLeave = false;
                if (ghostColor == Color.Red) {
                    MovingDirection = Direction.Left;
                    currentAnimation = FaceLeft;
                    ghostHouseState = GhostHouseState.Outside;
                } else {
                    ghostHouseState = GhostHouseState.Inside;
                    if (ghostColor == Color.Pink) {
                        MovingDirection = Direction.Down;
                        currentAnimation = FaceDown;
                    } else {
                        MovingDirection = Direction.Up;
                        currentAnimation = FaceUp;
                    }
                }

                Position = homeTilePosition * 8;
                Position.X += 4;
                Position.Y += 4;

                normalSpeedModifier = LevelManager.Instance.CurrentLevel._GhostNormalSpeedModifier;
                frightenedSpeedModifier = LevelManager.Instance.CurrentLevel._GhostFrightenedSpeedModifier;
                tunnelSpeedModifier = LevelManager.Instance.CurrentLevel._GhostTunnelSpeedModifier;
                CurrentSpeed = _fullSpeed * normalSpeedModifier;

                isShown = false;
                IsMovable = false;
                isDisplayingAsScore = false;
                displayingAsScoreAge = 1000;

                UpdateCheckingTileInfo();
                distanceFromMovingTile = 0;
            } else {
                if (LevelManager.Instance.CurrentIntermission.IntermissionIndex == 1) {
                    currentAnimation = FaceLeft;
                    Position = new Vector2(ScreenManager.Instance.WindowDimensions.X + 32 + 32,
                        ScreenManager.Instance.WindowDimensions.Y / 2);
                    CurrentSpeed = _fullSpeed * 0.95f;
                    MovingDirection = Direction.Left;
                    isShown = true;
                    IsMovable = true;
                } else if (LevelManager.Instance.CurrentIntermission.IntermissionIndex == 2) {
                    currentAnimation = FaceLeft;
                    Position = new Vector2(ScreenManager.Instance.WindowDimensions.X + 32 + 32,
                        ScreenManager.Instance.WindowDimensions.Y / 2);
                    CurrentSpeed = _fullSpeed * 0.8f;
                    MovingDirection = Direction.Left;
                    isShown = true;
                    IsMovable = true;
                } else if (LevelManager.Instance.CurrentIntermission.IntermissionIndex == 3) {
                    ForceSwitchAnimation("PatchedLeft");
                    Position = new Vector2(ScreenManager.Instance.WindowDimensions.X + 32 + 48,
                        ScreenManager.Instance.WindowDimensions.Y / 2);
                    CurrentSpeed = _fullSpeed;
                    MovingDirection = Direction.Left;
                    isShown = true;
                    IsMovable = true;
                }
            }
        }

        public void UnloadContent() {
            // TODO: Figure out what to do when UnloadContent is called.
        }

        public void Update(GameTime gameTime) {
            if (LevelManager.Instance.IsUsingLevel) {
                if (isDisplayingAsScore) {
                    if (displayingAsScoreAge >= 0)
                        displayingAsScoreAge -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    else {
                        isDisplayingAsScore = false;
                        IsMovable = true;
                        LevelManager.Instance.CurrentLevel.Player.IsShown = true;
                        ChangeCurrentAnimation();
                    }
                }
                if (IsMovable) {
                    if (!LevelManager.Instance.CurrentLevel.IsLevelPaused
                        || (ghostState == GhostState.Retreating && LevelManager.Instance.CurrentLevel.LevelEvent == LevelEvent.None)) {
                        InteractWithMap();
                        currentAnimation.Update(gameTime);
                    }
                }
            } else {
                if (IsMovable) {
                    if (MovingDirection == Direction.Left)
                        Position.X -= CurrentSpeed;
                    else
                        Position.X += CurrentSpeed;
                }
                currentAnimation.Update(gameTime);
            }
        }

        protected void InteractWithMap() {
            if (ghostHouseState == GhostHouseState.Outside) {
                if (currentTile.SlowMoving && (ghostState == GhostState.Chase || ghostState == GhostState.Scatter || ghostState == GhostState.Frightened))
                    ChangeMoveSpeed("Tunnel");
                else if (ghostState == GhostState.Chase || ghostState == GhostState.Scatter)
                    ChangeMoveSpeed("Normal");
                else if (ghostState == GhostState.Frightened)
                    ChangeMoveSpeed("Frightened");
                else if (ghostState == GhostState.Retreating)
                    ChangeMoveSpeed("Retreating");
            } else {
                if (ghostHouseState == GhostHouseState.Entering)
                    ChangeMoveSpeed("Retreating");
                else if (ghostHouseState == GhostHouseState.Inside)
                    ChangeMoveSpeed("Tunnel");
                else if (ghostHouseState == GhostHouseState.Leaving)
                    ChangeMoveSpeed("Tunnel");
            }
            
            desiredMovingDistance = CurrentSpeed;

            while (desiredMovingDistance != 0) {
                UpdateCheckingTileInfo();

                if (ghostHouseState == GhostHouseState.Outside) {
                    if (distanceFromMovingTile == 0) {
                        if (ghostState == GhostState.Frightened) {
                            List<Direction> availableDirections = new List<Direction>();
                            Random random = new Random();

                            if (currentTile.ConnectedUp && MovingDirection != Direction.Down && !currentTile.UpwardsTurnForbidden)
                                availableDirections.Add(Direction.Up);
                            if (currentTile.ConnectedDown && MovingDirection != Direction.Up)
                                availableDirections.Add(Direction.Down);
                            if (currentTile.ConnectedLeft && MovingDirection != Direction.Right)
                                availableDirections.Add(Direction.Left);
                            if (currentTile.ConnectedRight && MovingDirection != Direction.Left)
                                availableDirections.Add(Direction.Right);

                            pendingDirection = Direction.Null;
                            MovingDirection = availableDirections[random.Next(availableDirections.Count - 1)];
                        } else {
                            if (ghostState == GhostState.Scatter) {
                                if (ghostColor == Color.Red)
                                    SetTargetTile();
                                else
                                    targetTilePosition = scatteringTilePosition;
                            } else if (ghostState == GhostState.Retreating)
                                targetTilePosition = LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile();
                            else if (ghostState == GhostState.Chase)
                                SetTargetTile();

                            if (pendingDirection == Direction.Null || movingTile.TileType == TileType.Wall)
                                EmergencySetMovingDirection();
                            else if (pendingDirection == Direction.Up && currentTile.ConnectedUp
                                || pendingDirection == Direction.Down && currentTile.ConnectedDown
                                || pendingDirection == Direction.Left && currentTile.ConnectedLeft
                                || pendingDirection == Direction.Right && currentTile.ConnectedRight) {

                                MovingDirection = pendingDirection;
                                pendingDirection = Direction.Null;
                            }
                            SetPendingDirection();
                        }
                    }

                    if (Position.X == LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().X * 8 + 4
                            && Position.Y == LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y * 8 + 4) {
                        if (ghostState == GhostState.Retreating) {
                            MovingDirection = Direction.Down;
                            ghostHouseState = GhostHouseState.Entering;
                            System.Diagnostics.Debug.WriteLine("Entering Ghost House");
                        }
                    }
                } else {
                    if (distanceFromMovingTile == 0 || Position.X == homeTilePosition.X * 8 + 4 
                    || Position.X == LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().X * 8 + 4) {

                        if (ghostHouseState == GhostHouseState.Entering
                        && Position.Y == (LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y + 3) * 8 + 4) {
                            if (Position.X == homeTilePosition.X * 8 + 4) {
                                LevelManager.Instance.CurrentLevel.retreatingGhosts -= 1;
                                ghostHouseState = GhostHouseState.Inside;
                                LevelManager.Instance.CurrentLevel.housedGhosts.Add(this);
                                ghostState = LevelManager.Instance.CurrentLevel.ghostCycle;
                                ChangeCurrentAnimation();

                                MovingDirection = Direction.Null;
                            } else {
                                if (Position.X > homeTilePosition.X * 8 + 4) {
                                    MovingDirection = Direction.Left;
                                } else {
                                    MovingDirection = Direction.Right;
                                }
                            }
                        } else if (ghostHouseState == GhostHouseState.Inside) {
                            if (Position.Y == (LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y + 3) * 8 + 4) {
                                if (PendingToLeave) {
                                    ghostHouseState = GhostHouseState.Leaving;
                                    if (Position.X == LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().X * 8 + 4) {
                                        MovingDirection = Direction.Null;
                                    } else if (Position.X > LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().X * 8 + 4) {
                                        LevelManager.Instance.CurrentLevel.ElroyPossible = true;
                                        MovingDirection = Direction.Left;
                                    } else if (Position.X < LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().X * 8 + 4) {
                                        MovingDirection = Direction.Right;
                                    }
                                }
                            } else {
                                if (Position.Y == (LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y + 2) * 8 + 4 + 4) {
                                    MovingDirection = Direction.Down;
                                } else if (Position.Y == (LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y + 4) * 8 + 4 - 4) {
                                    MovingDirection = Direction.Up;
                                }
                            }
                        } else if (ghostHouseState == GhostHouseState.Leaving) {
                            if (Position.Y == LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y * 8 + 4) {
                                ghostHouseState = GhostHouseState.Outside;

                                if (leaveToLeft)
                                    MovingDirection = Direction.Left;
                                else
                                    MovingDirection = Direction.Right;
                                leaveToLeft = false;
                            } else {
                                if (Position.Y == (LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().Y + 3) * 8 + 4
                                    && Position.X == LevelManager.Instance.CurrentLevel.Map.GetGhostHouseEntranceTile().X * 8 + 4) {
                                    MovingDirection = Direction.Up;
                                }
                            }
                        }
                    }
                }

                ChangeCurrentAnimation();
                UpdateCheckingTileInfo();
                TryMove();
                TryTouchPlayer();
            }
        }

        private void ChangeCurrentAnimation() {
            if (!isDisplayingAsScore) {
                if (ghostState == GhostState.Frightened) {
                    return;
                } else if (ghostState == GhostState.Retreating) {
                    if (MovingDirection == Direction.Up)
                        currentAnimation = EyesUp;
                    else if (MovingDirection == Direction.Down)
                        currentAnimation = EyesDown;
                    else if (MovingDirection == Direction.Left)
                        currentAnimation = EyesLeft;
                    else if (MovingDirection == Direction.Right)
                        currentAnimation = EyesRight;
                } else {
                    if (MovingDirection == Direction.Up)
                        currentAnimation = FaceUp;
                    else if (MovingDirection == Direction.Down)
                        currentAnimation = FaceDown;
                    else if (MovingDirection == Direction.Left)
                        currentAnimation = FaceLeft;
                    else if (MovingDirection == Direction.Right)
                        currentAnimation = FaceRight;
                }
            }
        }

        protected void TryMove() {
            if (movingTile.TileType != TileType.Wall || distanceFromMovingTile != 0) {
                float checkingDistance = Math.Min(distanceFromMovingTile, Math.Abs(distanceFromMovingTile - 4));
                if (checkingDistance == distanceFromMovingTile) {
                    if (CurrentSpeed > distanceFromMovingTile && distanceFromMovingTile != 0)
                        movingDistance = distanceFromMovingTile;
                    else
                        movingDistance = desiredMovingDistance;
                } else {
                    if (CurrentSpeed > Math.Abs(distanceFromMovingTile - 4) && distanceFromMovingTile != 4)
                        movingDistance = Math.Abs(distanceFromMovingTile - 4);
                    else
                        movingDistance = desiredMovingDistance;
                }

                if (MovingDirection == Direction.Up)
                    Position.Y -= movingDistance;
                else if (MovingDirection == Direction.Down)
                    Position.Y += movingDistance;
                else if (MovingDirection == Direction.Left)
                    Position.X -= movingDistance;
                else if (MovingDirection == Direction.Right)
                    Position.X += movingDistance;
                desiredMovingDistance -= movingDistance;
                currentAnimation.AnimationPaused = false;
            } else {
                desiredMovingDistance = 0;
                currentAnimation.AnimationPaused = true;
            }

            if (Position.X < 0)
                Position.X = LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X * 8 + Position.X;
            else if (Position.X > LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X * 8)
                Position.X -= LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.X * 8;
            if (Position.Y < 0)
                Position.Y = LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y * 8 + Position.Y;
            else if (Position.Y > LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y * 8)
                Position.Y -= LevelManager.Instance.CurrentLevel.Map.TilemapDimensions.Y * 8;
        }

        protected void SetPendingDirection() {
            if (ghostState != GhostState.Frightened && ghostHouseState == GhostHouseState.Outside && movingTile.TileType != TileType.Wall) {
                Vector2 movingTilePositionVector = new Vector2(movingTilePosition.X, movingTilePosition.Y);
                Vector2 targetTilePositionVector = new Vector2(targetTilePosition.X, targetTilePosition.Y);

                float upDirectionDistance = float.MaxValue,
                    downDirectionDistance = float.MaxValue,
                    leftDirectionDistance = float.MaxValue,
                    rightDirectionDistance = float.MaxValue;

                if (movingTile.ConnectedUp && MovingDirection != Direction.Down
                    && !movingTile.UpwardsTurnForbidden) {
                    upDirectionDistance = Vector2.Distance(
                        new Vector2(movingTilePositionVector.X, movingTilePositionVector.Y - 1), targetTilePositionVector);
                }
                if (movingTile.ConnectedDown && MovingDirection != Direction.Up) {
                    downDirectionDistance = Vector2.Distance(
                        new Vector2(movingTilePositionVector.X, movingTilePositionVector.Y + 1), targetTilePositionVector);
                }
                if (movingTile.ConnectedLeft && MovingDirection != Direction.Right) {
                    leftDirectionDistance = Vector2.Distance(
                        new Vector2(movingTilePositionVector.X - 1, movingTilePositionVector.Y), targetTilePositionVector);
                }
                if (movingTile.ConnectedRight && MovingDirection != Direction.Left) {
                    rightDirectionDistance = Vector2.Distance(
                        new Vector2(movingTilePositionVector.X + 1, movingTilePositionVector.Y), targetTilePositionVector);
                }

                float[] distances = { upDirectionDistance, downDirectionDistance, leftDirectionDistance, rightDirectionDistance };
                float minDistance = distances.Min();
                
                if (minDistance == upDirectionDistance)
                    pendingDirection = Direction.Up;
                else if (minDistance == leftDirectionDistance)
                    pendingDirection = Direction.Left;
                else if (minDistance == downDirectionDistance)
                    pendingDirection = Direction.Down;
                else if (minDistance == rightDirectionDistance)
                    pendingDirection = Direction.Right;
            }
        }

        protected void EmergencySetMovingDirection() {
            if (MovingDirection == Direction.Null) {
                MovingDirection = Direction.Left;
            } else {
                if (ghostState != GhostState.Frightened) {
                    Vector2 currentTilePositionVector = new Vector2(currentTilePosition.X, currentTilePosition.Y);
                    Vector2 targetTilePositionVector = new Vector2(targetTilePosition.X, targetTilePosition.Y);

                    float upDirectionDistance = float.MaxValue,
                        downDirectionDistance = float.MaxValue,
                        leftDirectionDistance = float.MaxValue,
                        rightDirectionDistance = float.MaxValue;
                    if (currentTile.ConnectedUp && MovingDirection != Direction.Down
                        && !currentTile.UpwardsTurnForbidden) {
                        upDirectionDistance = Vector2.Distance(
                            new Vector2(currentTilePositionVector.X, currentTilePositionVector.Y - 1), targetTilePositionVector);
                    }
                    if (currentTile.ConnectedDown && MovingDirection != Direction.Up) {
                        downDirectionDistance = Vector2.Distance(
                            new Vector2(currentTilePositionVector.X, currentTilePositionVector.Y + 1), targetTilePositionVector);
                    }
                    if (currentTile.ConnectedLeft && MovingDirection != Direction.Right) {
                        leftDirectionDistance = Vector2.Distance(
                            new Vector2(currentTilePositionVector.X - 1, currentTilePositionVector.Y), targetTilePositionVector);
                    }
                    if (currentTile.ConnectedRight && MovingDirection != Direction.Left) {
                        rightDirectionDistance = Vector2.Distance(
                            new Vector2(currentTilePositionVector.X + 1, currentTilePositionVector.Y), targetTilePositionVector);
                    }

                    float[] distances = { upDirectionDistance, downDirectionDistance, leftDirectionDistance, rightDirectionDistance };
                    float minDistance = distances.Min();
                    if (minDistance == upDirectionDistance)
                        MovingDirection = Direction.Up;
                    else if (minDistance == leftDirectionDistance)
                        MovingDirection = Direction.Left;
                    else if (minDistance == downDirectionDistance)
                        MovingDirection = Direction.Down;
                    else if (minDistance == rightDirectionDistance)
                        MovingDirection = Direction.Right;
                }
            }
        }

        protected abstract void SetTargetTile();

        public void ReceiveGhostState(GhostStateEvent ghostStateEvent) {
            if (ghostStateEvent != GhostStateEvent.None) {
                if (ghostStateEvent == GhostStateEvent.ShowGhosts) {
                    isShown = true;
                } else if (ghostStateEvent == GhostStateEvent.HideGhosts) {
                    isShown = false;
                } else if (ghostStateEvent == GhostStateEvent.EnterFrightenedMode) {
                    if (ghostState == GhostState.Chase || ghostState == GhostState.Scatter || ghostState == GhostState.Frightened) {
                        if (ghostState != GhostState.Frightened)
                            ReverseDirection();
                        ghostState = GhostState.Frightened;
                        currentAnimation = Frightened; 
                        currentAnimation.ResetAnimation();
                    }
                } else if (ghostStateEvent == GhostStateEvent.WarnLeavingFrightenedMode) {
                    if (ghostState == GhostState.Frightened) {
                        currentAnimation = FrightenedFlash;
                        currentAnimation.ResetAnimation();
                    }
                } else if (ghostStateEvent == GhostStateEvent.LeaveFrightenedMode) {
                    if (ghostState == GhostState.Frightened) {
                        ghostState = LevelManager.Instance.CurrentLevel.ghostCycle;
                        ChangeCurrentAnimation();
                    }
                } else if (ghostStateEvent == GhostStateEvent.EnterChaseMode) {
                    if (ghostState == GhostState.Chase || ghostState == GhostState.Scatter) {
                        ghostState = GhostState.Chase;
                        ReverseDirection();
                        if (ghostHouseState == GhostHouseState.Inside)
                            leaveToLeft = true;
                    }
                } else if (ghostStateEvent == GhostStateEvent.EnterScatterMode) {
                    if (ghostState == GhostState.Chase || ghostState == GhostState.Scatter) {
                        ghostState = GhostState.Scatter;
                        ReverseDirection();
                        if (ghostHouseState == GhostHouseState.Inside)
                            leaveToLeft = true;
                    }
                }
            }
        }

        protected void ReverseDirection() {
            if (ghostHouseState == GhostHouseState.Outside) {
                switch (MovingDirection) {
                    case Direction.Up:
                        MovingDirection = Direction.Down;
                        break;
                    case Direction.Down:
                        MovingDirection = Direction.Up;
                        break;
                    case Direction.Left:
                        MovingDirection = Direction.Right;
                        break;
                    case Direction.Right:
                        MovingDirection = Direction.Left;
                        break;
                }
                UpdateCheckingTileInfo();
                pendingDirection = Direction.Null;
                ChangeCurrentAnimation();
            }
        }

        private void TryTouchPlayer() {
            if (currentTilePosition == LevelManager.Instance.CurrentLevel.Player.currentTilePosition
                && LevelManager.Instance.CurrentLevel.LevelPausedAge <= 0) {
                if (ghostState == GhostState.Frightened) {
                    AudioManager.Instance.PlaySoundEffect("eat_ghost");
                    isDisplayingAsScore = true;
                    IsMovable = false;
                    LevelManager.Instance.CurrentLevel.LevelPausedAge = 1000;
                    displayingAsScoreAge = 1000;
                    currentAnimation = Scores;
                    LevelManager.Instance.CurrentLevel.Player.IsShown = false;

                    if (LevelManager.Instance.CurrentLevel.GhostsEatenInRow == 0) {
                        currentAnimation.CurrentFrameIndex = 0;
                        LevelManager.Instance.AddToScore(200);
                    } else if (LevelManager.Instance.CurrentLevel.GhostsEatenInRow == 1) {
                        currentAnimation.CurrentFrameIndex = 1;
                        LevelManager.Instance.AddToScore(400);
                    } else if (LevelManager.Instance.CurrentLevel.GhostsEatenInRow == 2) {
                        currentAnimation.CurrentFrameIndex = 2;
                        LevelManager.Instance.AddToScore(800);
                    } else if (LevelManager.Instance.CurrentLevel.GhostsEatenInRow == 3) {
                        currentAnimation.CurrentFrameIndex = 3;
                        LevelManager.Instance.AddToScore(1600);
                        if (LevelManager.Instance.CurrentLevel.GhostsCapturedThisLevel == 16)
                            LevelManager.Instance.AddToScore(12000);
                    }

                    LevelManager.Instance.CurrentLevel.GhostsEatenInRow++;
                    LevelManager.Instance.CurrentLevel.retreatingGhosts += 1;
                    ghostState = GhostState.Retreating;
                } else if (LevelManager.Instance.CurrentLevel.LevelEvent == LevelEvent.None && ghostState != GhostState.Retreating) {
                    LevelManager.Instance.CurrentLevel.LevelEvent = LevelEvent.PlayerDeath;
                }
            }
        }

        protected virtual void ChangeMoveSpeed(string speedModifierName) {
            float speedModifier = 0;
            if (speedModifierName == "Normal") {
                speedModifier = normalSpeedModifier;
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

        public virtual void ForceSwitchAnimation(string animationName) {
            if (animationName == "FaceLeft")
                currentAnimation = FaceLeft;
            else if (animationName == "FaceRight") {
                currentAnimation = FaceRight;
            } else if (animationName == "Frightened") {
                currentAnimation = Frightened;
            }
        }

        public virtual void Draw() {
            if (isShown) {
                if (!isDisplayingAsScore) {
                    ScreenManager.Instance.Draw("Ghosts", currentAnimation.GetCurrentFrameRectangle(),
                        new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 
                        currentAnimation.animationFrameDimension.X, currentAnimation.animationFrameDimension.Y));
                } else {
                    ScreenManager.Instance.Draw("Scores", currentAnimation.GetCurrentFrameRectangle(),
                        new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16));
                }
                
                if (LevelManager.Debug) {
                    ScreenManager.Instance.DrawRectangle(new Rectangle(currentTilePosition.X * 8, currentTilePosition.Y * 8, 8, 8), ghostColor, 0.5f);
                    ScreenManager.Instance.DrawRectangle(new Rectangle((int)targetTilePosition.X * 8, (int)targetTilePosition.Y * 8, 8, 8), ghostColor, 0.25f);
                }
            }
        }
    }
}
