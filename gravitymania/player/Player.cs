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
using gravitymania.collision;

namespace gravitymania.player
{
    public enum JumpState
    {
        Idle,
        Jumping,
        JumpHeightCancel,
    }

    public class Player : InputEventListener, EllipseCollisionObject
    {
        private const float JumpingGravity = 0.5f;
        private const float JumpCancelGravity = 1.1f;
        private const float IdleGravity = 0.7f;
        private const float JumpVelocity = 9.0f;
        private const float UpwardCharge = 600.0f;
        private const float DownwardCharge = -500.0f;
        private const float ChargeCoreRadius = 32.0f;
        private const float CollisionBuffer = 0.1f;
        private const float MaxGroundedVelocity = 7.0f;
        private const float MaxAerialVelocity = 5.0f;
        private const float GroundLinearDamping = 1.0f;
        private const float AerialLinearDamping = 1.0f;
        private const float GroundAccellerationFactor = 3.0f;
        private const float AerialLowSpeedThreshold = 2.0f;
        private const float AerialLowSpeedAccellerationFactor = 1.0f;
        private const float AerialHighSpeedAccellerationFactor = 0.5f; // should be a factor of the 'default' friction
        private const float AerialOvermaxAccellerationFactor = 0.1f;
        private const float AerialUnderzeroAccellerationFactor = 0.3f;

        private Texture2D Image;
        public Vector2 HalfWidth;

        public Vector2 LastKnownGroundPosition;
		public Vector2 LastKnownGroundPlane;
        public Vector2 LastKnownGroundVelocity;
		public int PlayerIndex { get; private set; }
        public float GravityCharge;

        public bool HeldLeft;
        public bool HeldRight;
        public bool HeldJump;
        public JumpState JumpState;

        private Vector2 Position;
        private Vector2 Velocity;

        private bool Grounded;
        private bool LeftWall;
        private bool RightWall;

        Vector2 CollisionObject.Position
        {
            get { return Position; }
            set { Position = value; }
        }

        Vector2 CollisionObject.Velocity
        {
            get { return Velocity; }
            set { Velocity = value; }
        }


        public AABBox RoughBounds
        {
            get
            {
                return new AABBox(Position - HalfWidth, Position + HalfWidth);
            }
        }

        public Ellipse Collision
        {
            get
            {
                return new Ellipse(Position, HalfWidth);
            }
        }

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

            JumpState = JumpState.Idle;
        }

        public void InputEvent(EventKey e, EventData d)
        {
            if (e.Code == EventCode.Left)
            {
                HoldEventData data = d as HoldEventData;
                HeldLeft = (data.State == ButtonState.Pressed);
            }
            else if (e.Code == EventCode.Right)
            {
                HoldEventData data = d as HoldEventData;
                HeldRight = (data.State == ButtonState.Pressed);
            }
            else if (e.Code == EventCode.Jump)
            {
                HoldEventData data = d as HoldEventData;

                HeldJump = (data.State == ButtonState.Pressed);

                if (HeldJump && Grounded)
                {
                    Velocity.Y = JumpVelocity;
                    JumpState = JumpState.Jumping;
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

        public void UpdateDirectionMovement(float direction)
        {
            if (Grounded)
            {
                Vector2 groundDirection;
                if (direction > 0.0f)
                {
                    groundDirection = LastKnownGroundPlane.GetRightNorm().GetNormalized();
                }
                else
                {
                    groundDirection = LastKnownGroundPlane.GetLeftNorm().GetNormalized();
                }

                float velocityFactor = ((MaxGroundedVelocity) - (Velocity.Length())) / MaxGroundedVelocity;
                Velocity += (velocityFactor * GroundAccellerationFactor) * groundDirection;
            }
            else
            {
                if ((Velocity.X * LastKnownGroundVelocity.X) >= 0.0f && MathUtil.InRange(Math.Abs(Velocity.X), 0.0f, AerialLowSpeedThreshold))
                {
                    float velocityFactor = ((direction * MaxAerialVelocity) - Velocity.X) / MaxAerialVelocity;
                    Velocity.X += velocityFactor * AerialLowSpeedAccellerationFactor;
                }
                else if ((Velocity.X * LastKnownGroundVelocity.X) >= 0.0f && MathUtil.InRange(Math.Abs(Velocity.X), AerialLowSpeedThreshold, Math.Abs(LastKnownGroundVelocity.X)))
                {
                    float velocityFactor = ((direction * MaxAerialVelocity) - Velocity.X) / MaxAerialVelocity;
                    Velocity.X += velocityFactor * AerialHighSpeedAccellerationFactor;
                }
                else if (Math.Abs(Velocity.X) > Math.Abs(LastKnownGroundVelocity.X))
                {
                    float velocityFactor = ((direction * MaxAerialVelocity) - Velocity.X) / MaxAerialVelocity;
                    Velocity.X += velocityFactor * AerialOvermaxAccellerationFactor;
                }
                else
                {
                    float velocityFactor = ((direction * MaxAerialVelocity) - Velocity.X) / MaxAerialVelocity;
                    Velocity.X += velocityFactor * AerialUnderzeroAccellerationFactor;
                }
            }
        }

        public void ApplyLinearDamping()
        {
            float direction = Math.Sign(Velocity.X);

            if (Grounded)
            {
                if (Math.Abs(Velocity.X) < GroundLinearDamping)
                {
                    Velocity.X = 0.0f;
                }
                else
                {
                    Velocity.X -= (direction * GroundLinearDamping);
                }

                Velocity.Y = 0.0f;
            }
            else
            {
                if (Math.Abs(Velocity.X) < AerialLinearDamping)
                {
                    Velocity.X = 0.0f;
                }
                else
                {
                    Velocity.X -= (direction * AerialLinearDamping);
                }
            }
        }

        public void Update(MainGame game)
        {
            Player other = OtherPlayer(game);

            Vector2 v = new Vector2(0.0f, Math.Min(-(other.Position.Y + Position.Y), -ChargeCoreRadius));
            float vlen = v.Length();

            Velocity += ((BasicChargeAmount * other.BasicChargeAmount) / (vlen * vlen * vlen)) * new Vector2(0.0f, -Math.Abs(other.Velocity.Y));

            if (HeldLeft ^ HeldRight)
            {
                float direction = (HeldLeft ? -1.0f : 1.0f);
                UpdateDirectionMovement(direction);
            }
            else
            {
                ApplyLinearDamping();
            }

            if (JumpState == JumpState.Jumping && !HeldJump)
            {
                JumpState = JumpState.JumpHeightCancel;
            }

            if (JumpState == JumpState.Jumping)
            {
                Velocity.Y -= JumpingGravity;
            }
            else if (JumpState == JumpState.JumpHeightCancel)
            {
                Velocity.Y -= JumpCancelGravity;
            }
            else if (JumpState == JumpState.Idle)
            {
                Velocity.Y -= IdleGravity;
            }

            if (Velocity.Y < 0.0f)
            {
                JumpState = JumpState.Idle;
            }

            bool wasGrounded = Grounded;

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
					result = GameCollision.GetFirstCollision(this, game.Data[this.PlayerIndex], currentTime);
				}
				else
				{
                    result = GameCollision.GetFirstCollision(this, game.Data[this.PlayerIndex], currentTime, firstPlane.Normal);
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
				// (Could probably change this to wall touching if no wall jumping
				if (Grounded || Vector2.Dot(Velocity.GetNormalized(), new Vector2(0.0f, 1.0f)) < 0.3f)
				{
                    // If you are grounded and run into a wall, don't 'run' up the wall
                    if (Grounded && Math.Abs(Vector2.Dot(result.Normal, new Vector2(1.0f, 0.0f))) > 0.7f)
                    {
                        Velocity.Y = 0.0f;
                    }
                    else
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

            if (RoughBounds.Min.Y < 0.0f)
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

                bool s = false;
                if (s)
                {
                    Console.WriteLine(LeftWall);
                    Console.WriteLine(RightWall);
                }

                LastKnownGroundPosition = Position;
                LastKnownGroundVelocity = Velocity;
                JumpState = JumpState.Idle;
            }
            // This prevents you from rocketing off into space when leaving an upward slope
            else if (!Grounded && wasGrounded && Velocity.Y > 0 && JumpState == JumpState.Idle)
            {
                Velocity.Y = -JumpCancelGravity;
                LastKnownGroundVelocity = Velocity;
            }
        }

        public float CurrentJumpHeight()
        {
            return Position.Y - LastKnownGroundPosition.Y;
        }

        private Player OtherPlayer(MainGame game)
        {
            return game.Data[(PlayerIndex+1)%2].Player;
        }

        public void Render(SpriteBatch drawer, Camera camera)
        {
            Rectangle box = camera.GetSpriteBox(RoughBounds);
            drawer.Draw(Image, box, Color.BlanchedAlmond);
        }
    }
}
