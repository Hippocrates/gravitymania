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
using Microsoft.Xna.Framework.Input;

namespace gravitymania.player
{
    public class Player : InputEventListener
    {
        private const float Gravity = 0.9f;
        private const float JumpVelocity = 10.0f;
        private const float UpwardCharge = 600.0f;
        private const float DownwardCharge = -300.0f;
        private const float ChargeCoreRadius = 32.0f;
        private const float CollisionBuffer = 0.1f;

        private Texture2D Image;
        public Vector2 HalfWidth;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 LastKnownGroundPosition;
		public Vector2 LastKnownGroundPlane;
        public InputFrame<PlayerKey> InputState;
		public int PlayerIndex { get; private set; }
        public float GravityCharge;

        public bool Grounded;
		public bool LeftWall;
		public bool RightWall;

        public List<CollisionResult> collisionInfoThisFrame = new List<CollisionResult>();

        public Player(MainGame game, int playerIndex, Vector2 position, Vector2 halfWidth)
        {
			PlayerIndex = playerIndex;
            Position = position;
            HalfWidth = halfWidth;
            Velocity = new Vector2(0.0f, 0.0f);
			LastKnownGroundPlane = new Vector2(0.0f, 1.0f);

            Image = new Texture2D(game.Root.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Image.SetData(new[] { Color.White });

            Grounded = true;
            GravityCharge = 0.0f;
        }

        public void InputEvent(EventKey e, EventData d)
        {
            if (e.Code == EventCode.Left)
            {
                // do something
            }
            else if (e.Code == EventCode.Right)
            {
                // do something
            }
            else if (e.Code == EventCode.Jump)
            {
                HoldEventData data = d as HoldEventData;

                if (data.State == ButtonState.Pressed && Grounded)
                {
                    Velocity.Y = JumpVelocity;
                    Grounded = false;
                }
            }
        }

        public float BasicChargeAmount
        {
            get
            {
                if (Velocity.Y > 0.0f)
                {
                    return UpwardCharge;
                }
                else if (Velocity.Y < 0.0f)
                {
                    return DownwardCharge;
                }
                else
                {
                    return 0.0f;
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
            /*
			Direction inputDirection = InputState.IsDown(PlayerKey.LEFT) ? Direction.Left : (InputState.IsDown(PlayerKey.RIGHT) ? Direction.Right : Direction.None);

			// TODO: implement actual accelleration/decelleration and the like
            if (inputDirection != Direction.None)
            {
				if (Grounded)
				{
					// Change this so that going 'down' is faster than going 'up'
					Velocity = LastKnownGroundPlane.GetRightNorm() * 4.0f * inputDirection.Sign();
				}
				else
				{
					Velocity.X = 4.0f * inputDirection.Sign();
				}
            }
            else
            {
				if (Math.Abs(Velocity.X) > 0.7f)
				{
					Velocity.X -= Math.Sign(Velocity.X) * 0.7f;
				}
				else
				{
					Velocity.X = 0.0f;
				}
            }

			if (Math.Abs(Velocity.X) > 4.0f)
			{
				Velocity.X = Math.Sign(Velocity.X) * 4.0f;
			}
            */

            Velocity.Y -= Gravity;
            
            Player other = OtherPlayer(game);

            Vector2 v = new Vector2(0.0f, Math.Min(-(other.Position.Y + Position.Y), -ChargeCoreRadius));
            float vlen = v.Length();

            Velocity += ((BasicChargeAmount * other.BasicChargeAmount) / (vlen * vlen * vlen)) * new Vector2(0.0f, -Math.Abs(other.Velocity.Y));

            Grounded = false;
            LeftWall = false;
            RightWall = false;

			// We should do the grounded/wall state as a seperate resolution step
			// In particular, it should detect closeness to a wall without having to be pushing into it

            Vector2 initialVelocity = Velocity;
            Vector2 initialPosition = Position;
            LineEquation firstPlane = new LineEquation();
            CollisionResult result;

            collisionInfoThisFrame.Clear();

			float currentTime = 0.0f;

            for (int i = 0; i < 2; ++i)
            {
				if (i == 0)
				{
					result = GetFirstCollision(game, currentTime);
				}
				else
				{
					result = GetFirstCollision(game, currentTime, firstPlane.Normal);
				}

                if (!result.Hit)
                {
					Position += Velocity * (1.0f - currentTime);
                    break;
                }

				// Decrease the time 'remaining' in the current frame
				currentTime -= result.Time;
                collisionInfoThisFrame.Add(result);

				// Ensure that there is some floating-point tolerance added to collisions
                float distanceToHit = Velocity.Length() * result.Time;
				float distanceWithTolerance = Math.Max(distanceToHit - 0.0001f, 0.0f);

                Position += Velocity.GetNormalized() * distanceWithTolerance;

                Vector2 eRad = result.Normal * HalfWidth;
                float longRadius = eRad.Length() + 0.0001f;
                LineEquation currentPlane = new LineEquation(result.Position, result.Normal);

				if (i == 0)
				{
					firstPlane = currentPlane;
				}

				// re-direct velocity to be the projection onto the current plane
				// (Note, this makes landing on a slope a little off-putting, it may make sense to favor heavier x-projection)
				Vector2 planeLhs = currentPlane.LeftNorm();
				Vector2 planeRhs = currentPlane.RightNorm();

				if (Vector2.Dot(Velocity, planeLhs) > 0.0f)
				{
					Velocity = Vector2.Dot(planeLhs, Velocity) * planeLhs;
				}
				else
				{
					Velocity = Vector2.Dot(planeRhs, Velocity) * planeRhs;
				}

				// Apply friction, unless we are travelling 'up' a wall, as this is super annoying
				// (Could probably change this to _any_ wall if no wall jumping
				if (Vector2.Dot(Velocity.GetNormalized(), new Vector2(0.0f, 1.0f)) < 0.3f)
				{
					// multiply by dynamic friction (this should be returned by the collision result structure
					Velocity *= 0.7f;

					// If we are on a 'groundable' slope, and our velocity is within a tolerable
					// threshold, immediately stop all movement alltogehter
					if (Velocity.Length() < 0.8f && Vector2.Dot(result.Normal, new Vector2(0.0f, 1.0f)) > 0.7f)
					{
						// Eventually, this should be that the velocity is set to the relative velocity of the object
						// we are on, but for static objects it shouldn't matter
						Velocity = new Vector2(0.0f, 0.0f);
					}
				}

                if (Vector2.Dot(result.Normal, new Vector2(0.0f, 1.0f)) > 0.7f)
                {
                    Grounded = true;
					RightWall = false;
					LeftWall = false;
					LastKnownGroundPlane = result.Normal;
                }

				if (!Grounded && Vector2.Dot(result.Normal, new Vector2(-1.0f, 0.0f)) == 1.0f)
				{
					RightWall = true;
				}

				if (!Grounded && Vector2.Dot(result.Normal, new Vector2(1.0f, 0.0f)) == 1.0f)
				{
					LeftWall = true;
				}
            }

            if (Bounds.Min.Y < 0.0f)
            {
                Velocity.Y = 0.0f;
                Position.Y = HalfWidth.Y;
                Grounded = true;
				LastKnownGroundPlane = new Vector2(0.0f, 1.0f);
            }

            if (Grounded)
            {
				LeftWall = false;
				RightWall = false;
                LastKnownGroundPosition = Position;
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
            Rectangle box = camera.GetSpriteBox(Bounds);
            drawer.Draw(Image, box, Color.BlanchedAlmond);
        }

		public CollisionResult GetFirstCollision(MainGame game, float currentTime = 0.0f)
		{
			return GetFirstCollision(game, currentTime, Vector2.Zero);
		}

		public CollisionResult GetFirstCollision(MainGame game, Vector2 disallowedNormal)
		{
			return GetFirstCollision(game, 0.0f, Vector2.Zero);
		}

        public CollisionResult GetFirstCollision(MainGame game, float currentTime, Vector2 disallowedNormal)
        {
            TileRange intersectedTiles = game.Maps[PlayerIndex].GetTileRange(GetCollisionBounds());

            bool foundAny = false;
            CollisionResult bestResult = new CollisionResult() { Time = 0.0f, Hit = false, };

            foreach (TileIndex i in intersectedTiles.IterateTiles())
            {
                Vector2 offset = game.Maps[PlayerIndex].GetTileOffset(i.X, i.Y);

                foreach (LineSegment segment in game.Maps[PlayerIndex].GetTileGeometry(i.X, i.Y))
                {
                    CollisionResult result;

					if (Collide.CollideEllipseWithLine(GetCollision(), Velocity * (1.0f - currentTime), segment, out result))
					{
						if (result.Hit && (!foundAny || bestResult.Time > result.Time))
						{
							// This additional check is to avoid 'bumpiness' when transfering between pairs of line segments 
							// Normally, the slightly altered normal when hitting the endpoints would cause it to treat 
							// it as a discrete event, but since the normals are almost identical, they should not 
							// cause a second collision event
							if (Vector2.Dot(result.Normal, disallowedNormal) < (1.0f - 0.001f))
							{
								bestResult = result;
								foundAny = true;
							}
						}
					}
                }
            }

            return bestResult;
        }
    }
}
