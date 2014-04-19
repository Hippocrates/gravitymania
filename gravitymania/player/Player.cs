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
					Velocity.X = -4.0f;
					/*
					if (Velocity.X >= -0.5f)
					{
						Velocity.X -= 0.9f;
					}
					else
					{
						Velocity.X -= 0.2f;
					}
					*/
                }
                else
                {
					Velocity.X = 4.0f;
					/*
					if (Velocity.X <= 0.5f)
					{
						Velocity.X += 0.9f;
					}
					else
					{
						Velocity.X += 0.2f;
					}
					*/
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

            Grounded = false;
            LeftWall = false;
            RightWall = false;

			// We should do the grounded/wall state as a seperate resolution step
			// In particular, it should detect closeness to a wall without having to be pushing into it

            Vector2 initialVelocity = Velocity;
            Vector2 initialPosition = Position;
            Vector2 destination = Position + Velocity;
            Vector2 initialDesitination = destination;
            LineEquation firstPlane = new LineEquation();

            CollisionResult result;

            collisionInfoThisFrame.Clear();

            // TODO: This still doesn't work very well
            // The problem seems to be points: the ellipse is 'bumping' over them on flat surfaces, or 
            // otherwise colliding with them _sooner_ than it would with lines
            for (int i = 0; i < 2; ++i)
            {
				if (i == 0)
				{
					result = GetFirstCollision(game);
				}
				else
				{
					result = GetFirstCollision(game, firstPlane.Normal);
				}

                if (!result.Hit)
                {
                    Position = destination;
                    break;
                }

                collisionInfoThisFrame.Add(result);

                float distance = Velocity.Length() * result.Time;
                float shortDist = Math.Max(distance - 0.00001f, 0.0f);

                Position += Velocity.GetNormal() * shortDist;

                Vector2 eRad = result.Normal * HalfWidth;
                float longRadius = eRad.Length() + 0.00001f;
                LineEquation currentPlane = new LineEquation(result.Position, result.Normal);

				if (i == 0)
				{
					firstPlane = currentPlane;
				}

				// The original (that is to say, improved) algorithm only did this for the 'first' 
				// plane, however I think this adjustment needs to be done for the second one as well
				// to avoid the problem of 'accumulating' velocity
				float adjustmentLength = (currentPlane.SignedDistance(destination) - longRadius);
				Vector2 adjustment = adjustmentLength * currentPlane.Normal;
				destination -= adjustment;
				Velocity = destination - Position;

				// This is a hack to avoid numeric problems causing objects to 'sink' into slopes as 
				// you ascend them:
				// Basically, it re-corrects your velocity to be _exactly_ the slope if you would otherwise
				// be about to sink into it.
				float direction = Vector2.Dot(Velocity, currentPlane.Normal);
				if (shortDist == 0.0f && result.Type == CollisionObject.Line && direction < 0.0f)
				{
					Vector2 lhs = currentPlane.LeftHandNormal();

					if (Vector2.Dot(Velocity, lhs) > 0.0f)
					{
						Velocity = lhs * Velocity.Length();
					}
					else
					{
						Velocity = currentPlane.RightHandNormal() * Velocity.Length();
					}
				}

                if (Vector2.Dot(result.Normal, new Vector2(0.0f, 1.0f)) > 0.7f)
                {
                    Grounded = true;
                }
            }

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
            Rectangle box = camera.GetSpriteBox(Bounds);
            drawer.Draw(Image, box, Color.BlanchedAlmond);
        }

		public CollisionResult GetFirstCollision(MainGame game)
		{
			return GetFirstCollision(game, Vector2.Zero);
		}

        public CollisionResult GetFirstCollision(MainGame game, Vector2 disallowedNormal)
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

                    if (Collide.CollideEllipseWithLine(GetCollision(), Velocity, segment, out result))
                    {
						if (!foundAny || (result.Hit && bestResult.Time > result.Time && result.Time >= 0.0f))
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
