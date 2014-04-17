using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gravitymania.camera;
using gravitymania.game;
using gravitymania.main;
using gravitymania.math;
using gravitymania.input;
using gravitymania.map;

namespace gravitymania.player
{
    public class Player
    {
        private const float Gravity = 0.5f;
        private const float JumpVelocity = 10.0f;
        private const float UpwardCharge = 600.0f;
        private const float DownwardCharge = -300.0f;
        private const float ChargeCoreRadius = 32.0f;
        private const float CollisionBuffer = 0.1f;
        //private static readonly float JumpHeight = (JumpVelocity / 2.0f) * (JumpVelocity / Gravity);

        private Texture2D Image;
        public Vector2 HalfWidth;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 LastKnownGroundPosition;
        public InputFrame<PlayerKey> InputState;
		public int PlayerIndex { get; private set; }
        public float GravityCharge;

        public bool Grounded;
		public bool LeftWall;
		public bool RightWall;

        public Player(MainGame game, int playerIndex, Vector2 position, Vector2 halfWidth)
        {
			PlayerIndex = playerIndex;
            Position = position;
            HalfWidth = halfWidth;
            Velocity = new Vector2(0.0f, 0.0f);

            Image = new Texture2D(game.Root.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Image.SetData(new[] { Color.White });

            InputState = new InputFrame<PlayerKey>();

            Grounded = true;
            GravityCharge = 0.0f;
        }

        public float BasicChargeAmount
        {
            get
            {
                if (Grounded)
                {
                    return 0.0f;
                }
                else if (Velocity.Y > 0.0f)
                {
                    return UpwardCharge;
                }
                else
                {
                    return DownwardCharge;
                }
            }
        }

        public AABBox Bounds
        {
            get
            {
                return new AABBox(Position - HalfWidth, Position + HalfWidth);
            }
        }

        public AABBox GetCollisionBounds()
        {
            AABBox box = new AABBox(Bounds);
            box.AddInternalPoint(Bounds.Min + Velocity);
            box.AddInternalPoint(Bounds.Max + Velocity);
            box.AddInternalPoint(Bounds.Min + HalfWidth * CollisionBuffer);
            box.AddInternalPoint(Bounds.Max + HalfWidth * CollisionBuffer);
            return box;
        }

		public Ellipse GetCollision()
		{
			return new Ellipse(Position, HalfWidth);
		}

        public void Update(MainGame game)
        {
            if (InputState.IsDown(PlayerKey.LEFT) || InputState.IsDown(PlayerKey.RIGHT))
            {
                if (InputState.IsDown(PlayerKey.LEFT))
                {
                    Velocity.X = -5.0f;
                }
                else
                {
                    Velocity.X = 5.0f;
                }
            }
            else
            {
                Velocity.X = 0.0f;
            }

			if (LeftWall && InputState.IsDown(PlayerKey.JUMP))
			{
				Velocity = new Vector2(0.0f, JumpVelocity);
				Grounded = false;
			}

			if (RightWall && InputState.IsDown(PlayerKey.JUMP))
			{
				Velocity = new Vector2(0.0f, JumpVelocity);
				Grounded = false;
			}

            if (Grounded && InputState.IsDown(PlayerKey.JUMP))
            {
                Velocity.Y = JumpVelocity;
				Grounded = false;
            }
            else
            {
                Velocity.Y -= Gravity;
            }

            Player other = OtherPlayer(game);

            Vector2 v = new Vector2(0.0f, Math.Min(-(other.Position.Y + Position.Y), -ChargeCoreRadius));
            float vlen = v.Length();

            Velocity += ((BasicChargeAmount * other.BasicChargeAmount) / (vlen * vlen * vlen)) * new Vector2(0.0f, -Math.Abs(other.Velocity.Y));

			bool resolved = true;

            int collisions = 0;

			float currentTime = 0.0f;

			Grounded = false;
			LeftWall = false;
			RightWall = false;

			do
			{
				TileRange intersectedTiles = game.Maps[PlayerIndex].GetTileRange(GetCollisionBounds());

                resolved = true;
				bool foundAny = false;
				CollisionResult bestResult = new CollisionResult() { Time = 0.0f, StartsInside = false };
				LineSegment collidingSegment = new LineSegment();

				foreach (TileIndex i in intersectedTiles.IterateTiles())
				{
                    Vector2 offset = game.Maps[PlayerIndex].GetTileOffset(i.X, i.Y);

					foreach (LineSegment segment in game.Maps[PlayerIndex].GetTileGeometry(i.X, i.Y))
					{
						CollisionResult result;

						if (Collide.CollideEllipseWithLine(GetCollision(), Velocity, segment, out result))
						{
							if (!foundAny || (bestResult.Time > result.Time && result.Time >= currentTime))
							{
								foundAny = true;
								bestResult = result;
								collidingSegment = segment;
							}
						}
					}
				}

				// TODO: right now, its treating the 'resovled' position as still colliding.  It should not do this, perhaps add
				// an adjacency tolerance to the calculations.
				// Still quite buggy, in particular, gets stuck on corners a lot
				// Wall friction should suck less:
				// -> In particular, wall friction should not be used unless you are falling down
				if (foundAny)
				{
                    ++collisions;
					resolved = false;

                    if (bestResult.StartsInside)
                    {
                        Position += bestResult.Normal * (bestResult.PenetrationDistance + 0.0001f);
                        Velocity = new Vector2(0.0f, 0.0f);
                    }
                    else
                    {
                        Vector2 distanceToHit = Velocity * (bestResult.Time - currentTime);
                        float realDistanceToHit = distanceToHit.Length();

                        if (realDistanceToHit > 0.00001f)
                        {
                            Vector2 dirV = distanceToHit;
                            dirV.Normalize();
                            Vector2 changeInPosition = dirV * (realDistanceToHit - 0.00001f);
                            Position += changeInPosition;
                            bestResult.Position -= dirV * 0.00001f;
                        }
                        
                        currentTime = bestResult.Time;

                        if (bestResult.Normal.Y > 0.0f && bestResult.Normal.Y >= Math.Abs(bestResult.Normal.X))
                        {
                            Grounded = true;

                            if (PlayerIndex == 0)
                            {
                                Console.WriteLine("HerE");
                            }
                        }
                        else if (bestResult.Normal.Y == 0.0f)
                        {
                            if (bestResult.Normal.X > 0.0f)
                            {
                                LeftWall = true;
                            }
                            else
                            {
                                 RightWall = true;
                            }
                        }

                        Vector2 resolutionPosition = bestResult.Position + (Velocity * (1.0f - currentTime));
                        LineEquation collidingEquation = new LineEquation(bestResult.Position, bestResult.Normal);

                        

                        Vector2 resolutionProjection = collidingEquation.ClosestPoint(resolutionPosition);

                        // This is to prevent 'clinging' to walls while rising, as it is very annoying
                        if (bestResult.Normal.Y == 0.0f && Velocity.Y > 0.0f)
                        {
                            if (Velocity.Y > 0.0f)
                            {
                                Velocity.X = 0.0f;
                            }
                        }
                        // Otherwise, apply friction
                        else
                        {
                            Velocity = (resolutionProjection - bestResult.Position) * 0.8f;
                        }
                    }
				}

				if (Velocity.LengthSquared() <= 0.001f)
                {
                    resolved = true;
                }
                
                if (collisions > 10)
                {
					Velocity = new Vector2(0.0f, 0.0f);
					Console.WriteLine("Yikes!");
                    break;
                }

			}
            while (!resolved);

			// We should do the grounded/wall state as a seperate resolution step
			// In particular, it should detect closeness to a wall without having to be pushing into it

            Position += Velocity * (1.0f - currentTime);

            if (Bounds.Min.Y < 0.0f)
            {
                Velocity.Y = 0.0f;
                Position.Y = HalfWidth.Y;
                Grounded = true;
            }

            if (Grounded)
            {
				LeftWall = false;
				RightWall = false;
                LastKnownGroundPosition = Position - new Vector2(0.0f, HalfWidth.Y);
            }
        }

        public float CurrentJumpHeight()
        {
            return Position.Y - LastKnownGroundPosition.Y;
        }

        private Player OtherPlayer(MainGame game)
        {
            return game.Players[(PlayerIndex+1)%2];
        }

        public void Render(SpriteBatch drawer, Camera camera)
        {
            Rectangle box = camera.TransformToView(Bounds);
            drawer.Draw(Image, box, Color.BlanchedAlmond);
        }
    }
}
