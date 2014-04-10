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
        private const float CollisionBuffer = 0.5f;
        //private static readonly float JumpHeight = (JumpVelocity / 2.0f) * (JumpVelocity / Gravity);

        private Texture2D Image;
        public Vector2 HalfWidth;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 AffectedVelocity;
        public Vector2 LastKnownGroundPosition;
        public InputFrame<PlayerKey> InputState;
		public int PlayerIndex { get; private set; }
        public float GravityCharge;

        public bool Grounded;

        public Player(MainGame game, int playerIndex, Vector2 position, Vector2 halfWidth)
        {
			PlayerIndex = playerIndex;
            Position = position;
            HalfWidth = halfWidth;
            Velocity = new Vector2(0.0f, 0.0f);
            AffectedVelocity = new Vector2(0.0f, 0.0f);

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

        public AABBox GetCollisionBounds
        {
            get
            {
                AABBox box = new AABBox();
                box.AddBox(Bounds);
                box.AddInternalPoint(Bounds.Min + Velocity);
                box.AddInternalPoint(Bounds.Max + Velocity);
                box.AddInternalPoint(Bounds.Min + HalfWidth * CollisionBuffer);
                box.AddInternalPoint(Bounds.Max + HalfWidth * CollisionBuffer);
                return box;
            }
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

            if (Grounded && InputState.IsDown(PlayerKey.JUMP))
            {
                Velocity.Y = JumpVelocity;
                Grounded = false;
            }
            else if (!Grounded)
            {
                Velocity.Y -= Gravity;
            }

            Player other = OtherPlayer(game);

            Vector2 v = new Vector2(0.0f, Math.Min(-(other.Position.Y + Position.Y), -ChargeCoreRadius));
            float vlen = v.Length();

            Velocity += ((BasicChargeAmount * other.BasicChargeAmount) / (vlen * vlen * vlen)) * new Vector2(0.0f, -Math.Abs(other.Velocity.Y));

			bool resolved = true;

            int collisions = 0;

            Vector2 remainingVelocity = Velocity;

			do
			{
				TileRange intersectedTiles = game.Maps[PlayerIndex].GetTileRange(GetCollisionBounds);

                resolved = true;
				bool foundAny = false;
				float closestDelta = 0.0f;
				CollisionResult bestResult = new CollisionResult();
				LineSegment collidingSegment;

				foreach (TileIndex i in intersectedTiles.IterateTiles())
				{
                    Vector2 offset = game.Maps[PlayerIndex].GetTileOffset(i.X, i.Y);

					foreach (LineSegment segment in game.Maps[PlayerIndex].GetTileGeometry(i.X, i.Y))
					{
						CollisionResult result;

                        LineSegment xformedSegment = new LineSegment(segment.Start + offset, segment.End + offset);

                        if (Algebra.CollideEllipseWithLine(GetCollision(), remainingVelocity, xformedSegment, out result))
						{
							if (!foundAny || (closestDelta > result.Time))
							{
								foundAny = true;
								closestDelta = result.Time;
								bestResult = result;
                                collidingSegment = xformedSegment;
							}
						}
					}
				}

				// TODO: finish this, and also deal with the case of being already inside a collision plane at the start
				if (foundAny)
				{
                    ++collisions;
					resolved = false;

                    if (closestDelta > 0.0f)
                    {
                        Position += remainingVelocity * (closestDelta - 0.1f);
                        Grounded = true;
                        remainingVelocity = new Vector2(0.0f, 0.0f);
                    }
                    else
                    {
                        Position += new Vector2(0.0f, 3.0f);
                        remainingVelocity = new Vector2(0.0f, 0.0f);
                    }
				}

                if (remainingVelocity.LengthSquared() <= 0.001f)
                {
                    resolved = true;
                }

                if (collisions > 10)
                {
                    break;
                }

			}
            while (!resolved);

            Position += remainingVelocity;

            if (Bounds.Min.Y < 0.0f)
            {
                Velocity.Y = 0.0f;
                Position.Y = HalfWidth.Y;
                Grounded = true;
            }

            if (Grounded)
            {
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
