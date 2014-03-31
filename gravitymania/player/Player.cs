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
        private static readonly float JumpHeight = (JumpVelocity / 2.0f) * (JumpVelocity / Gravity);

        private Texture2D Image;
        public Vector2 HalfWidth;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 AffectedVelocity;
        public Vector2 LastKnownGroundPosition;
        public InputFrame<PlayerKey> InputState;
		public int PlayerIndex { get; private set; }

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
        }

        public AABBox Bounds
        {
            get
            {
                return new AABBox(Position - HalfWidth, Position + HalfWidth);
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

			bool resolved = true;

			do
			{
				TileRange intersectedTiles = game.Maps[PlayerIndex].GetTileRange(Bounds);

				bool foundAny = false;
				float closestDelta = 0.0f;
				CollisionResult bestResult;
				LineSegment collidingSegment;

				foreach (TileIndex i in intersectedTiles.IterateTiles())
				{
					foreach (LineSegment segment in game.Maps[PlayerIndex].GetTileGeometry(i.X, i.Y))
					{
						CollisionResult result;
						if (Algebra.CollideEllipseWithLine(GetCollision(), Velocity, segment, out result))
						{
							if (!foundAny || (closestDelta > result.Time))
							{
								foundAny = true;
								closestDelta = result.Time;
								bestResult = result;
								collidingSegment = segment;
							}
						}
					}
				}

				// TODO: finish this, and also deal with the case of being already inside a collision plane at the start
				if (foundAny)
				{
					resolved = false;
					//Position += 
				}

			}
			while(!resolved);

			/*
            Player other = OtherPlayer(game);

            if (!Grounded && !other.Grounded)
            {
                if (Velocity.Y > 0.0f && other.Velocity.Y > 0.0f)
                {
                    float ratio = (Math.Max(CurrentJumpHeight(), 0.0f) + Math.Max(other.CurrentJumpHeight(), 0.0f)) / JumpHeight;
                    if (ratio <= 1.0f)
                    {
                        ratio = 1.0f;
                    }


                    //AffectedVelocity = Velocity - ((JumpHeight * other.Velocity) / )
                }
            }

            Position += Velocity;

            if (Bounds.Min.Y < 0.0f)
            {
                Velocity.Y = 0.0f;
                Position.Y = HalfWidth.Y;
                Grounded = true;
            }*/

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
