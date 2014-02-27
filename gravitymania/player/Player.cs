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

namespace gravitymania.player
{
    public class Player
    {
        private Texture2D Image;
        public Vector2 HalfWidth;
        public Vector2 Position;
        public Vector2 Velocity;
        public InputFrame<PlayerKey> InputState;

        public bool Grounded;

        public Player(MainGame game, Vector2 position, Vector2 halfWidth)
        {
            Position = position;
            HalfWidth = halfWidth;
            Velocity = new Vector2(0.0f, 0.0f);

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
                for (int i = 0; i < 2; ++i)
                {
                    if (game.Players[i] != this)
                    {
                        if (!game.Players[i].Grounded)
                        {
                            game.Players[i].Velocity.Y -= 10.0f;
                        }
                    }
                }

                Velocity.Y = 10.0f;
                Grounded = false;
            }
            else if (!Grounded)
            {
                Velocity.Y += -0.5f;

                if (Velocity.Y < 0.0f)
                {

                    for (int i = 0; i < 2; ++i)
                    {
                        if (game.Players[i] != this)
                        {
                            if (!game.Players[i].Grounded)
                            {
                                game.Players[i].Velocity.Y += 0.5f;
                            }
                        }
                    }
                }
               
            }

            Position += Velocity;

            if (Bounds.Min.Y < 0.0f)
            {
                Velocity.Y = 0.0f;
                Position.Y = HalfWidth.Y;
                Grounded = true;
                
            }
        }

        public void Render(SpriteBatch drawer, Camera camera)
        {
            Rectangle box = camera.TransformToView(Bounds);
            drawer.Draw(Image, box, Color.BlanchedAlmond);
        }
    }
}
