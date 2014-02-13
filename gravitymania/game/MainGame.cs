using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.main;
using gravitymania.input;
using Microsoft.Xna.Framework;
using gravitymania.camera;
using gravitymania.map;
using Microsoft.Xna.Framework.Graphics;
using gravitymania.math;

namespace gravitymania.game
{
    

    public class MainGame : GameState
    {
        public static readonly Vector2 DefaultFieldSize = new Vector2(640.0f, 240.0f);

        private GameRoot Root;

        private SpriteBatch Drawer;
        private Texture2D onePixelTexture;

        // GameData
        public Camera[] Cameras;
        public TileMap[] Maps;
        public readonly int TileSize = 16;
        

        public MainGame(GameRoot root)
        {
            Root = root;
        }

        public void Begin()
        {
            Cameras = new Camera[2];
            for (int i = 0; i < 2; ++i)
            {
                Cameras[i] = new Camera(new Vector2(Root.Graphics.ScreenWidth, Root.Graphics.ScreenHeight / 2.0f), DefaultFieldSize, new Vector2(DefaultFieldSize.X / 2, DefaultFieldSize.Y / 2), i == 1);
            }
            
            Maps = TileMapLoader.LoadFromStupidText();

            Drawer = new SpriteBatch(Root.Graphics.Device);

            onePixelTexture = new Texture2D(Root.Graphics.Device, 1, 1, false, SurfaceFormat.Color);
            onePixelTexture.SetData(new[] { Color.White });
        }

        public void End()
        {
        }

        public void Suspend()
        {
        }

        public void Resume()
        {
        }

        public void Input(InputState state)
        {

        }

        public void Update()
        {
        }

        public void Draw()
        {
            Drawer.Begin();

            for (int i = 0; i < 2; ++i)
            {
                Tuple<int, int, int, int> bounds = Maps[i].GetTileBounds(Cameras[i].GetFieldBounds());

                for (int y = bounds.Item2; y <= bounds.Item4; ++y)
                {
                    for (int x = bounds.Item1; x <= bounds.Item3; ++x)
                    {
                        Tile t = Maps[i].GetTile(x, y);

                        if (t.Collision == CollisionType.SolidBox)
                        {
                            Rectangle box = Cameras[i].FieldBoundsToViewRectangle(Maps[i].GetTileBox(x, y));
                            box.Y += (i * 240);
                            Drawer.Draw(onePixelTexture, box, Color.Tomato);
                        }
                    }
                }
            }

            Drawer.Draw(onePixelTexture, new Rectangle(0, 236, 640, 8), Color.RosyBrown);

            Drawer.End();
        }
    }
}
