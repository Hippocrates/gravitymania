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
using gravitymania.player;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.game
{
    

    public class MainGame : GameState
    {
        public static readonly Vector2 DefaultFieldSize = new Vector2(640.0f, 240.0f);

        public GameRoot Root { get; private set; }

        private SpriteBatch Drawer;
        private Texture2D onePixelTexture;

        // GameData
        public Camera[] Cameras;
        public Player[] Players;
        public TileMap[] Maps;

        public RawKey[][] PlayerKeys = new RawKey[2][]
        {
            new RawKey[] { RawKey.Find("a"), RawKey.Find("d"), RawKey.Find("w") },
            new RawKey[] { RawKey.Find("LEFT"), RawKey.Find("RIGHT"), RawKey.Find("UP") },
        };

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
                Cameras[i] = new Camera(new Vector2(Root.Graphics.ScreenWidth, Root.Graphics.ScreenHeight / 2.0f), DefaultFieldSize, new Vector2(DefaultFieldSize.X / 2, DefaultFieldSize.Y / 2), new Vector2(0.0f, i * 240), i == 1);
            }
            
            Maps = TileMapLoader.LoadFromStupidText();

            Drawer = new SpriteBatch(Root.Graphics.Device);

            onePixelTexture = new Texture2D(Root.Graphics.Device, 1, 1, false, SurfaceFormat.Color);
            onePixelTexture.SetData(new[] { Color.White });

            Players = new Player[2];

            for (int i = 0; i < 2; ++i)
            {
                Players[i] = new Player(this, i, new Vector2(TileSize * 3 + TileSize / 2, TileSize * 3 + TileSize / 2), new Vector2(TileSize / 2, TileSize / 2));
            }
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
            for (int i = 0; i < PlayerKeys.Length; ++i)
            {
                for (int j = 0; j < PlayerKeys[i].Length; ++j)
                {
                    Players[i].InputState.SetState((PlayerKey)j, state.GetButtonState(PlayerKeys[i][j]) == ButtonState.Pressed);
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < 2; ++i)
            {
                Players[i].Update(this);
            }
        }

        public void Draw()
        {
            Drawer.Begin();

            for (int i = 0; i < 2; ++i)
            {
                TileRange bounds = Maps[i].GetTileRange(Cameras[i].GetFieldBounds());

                for (int y = bounds.Bottom; y <= bounds.Top; ++y)
                {
                    for (int x = bounds.Left; x <= bounds.Right; ++x)
                    {
                        Tile t = Maps[i].GetTile(x, y);

                        if (t.Collision == CollisionType.SolidBox)
                        {
                            Rectangle box = Cameras[i].TransformToView(Maps[i].GetTileBox(x, y));
                            Drawer.Draw(onePixelTexture, box, Color.Tomato);
                        }
                    }
                }
            }

            for (int i = 0; i < 2; ++i)
            {
                Players[i].Render(Drawer, Cameras[i]);
            }

            Drawer.Draw(onePixelTexture, new Rectangle(0, 236, 640, 8), Color.RosyBrown);

            Drawer.End();
        }
    }
}
