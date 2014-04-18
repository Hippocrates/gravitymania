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
using gravitymania.graphics;

namespace gravitymania.game
{
    

    public class MainGame : GameState
    {
		public FrameAdvanceManager FrameAdvance = new FrameAdvanceManager();
		public RawKey FrameAdvanceKey = RawKey.Find("\\");
		public RawKey UnPauseKey = RawKey.Find("p");
		public bool FrameAdvanceKeyState = false;

        public static readonly Vector2 DefaultFieldSize = new Vector2(640.0f, 240.0f);

        public GameRoot Root { get; private set; }

        private SpriteBatch Drawer;
        private PrimitiveBatch PrimitivesDrawer;
        private Texture2D onePixelTexture;

        // GameData
        public Camera[] Cameras;
        public Player[] Players;
        public TileMap[] Maps;

        public RawKey[][] PlayerKeys = new RawKey[2][]
        {
            new RawKey[] { new X360PadKey(PlayerIndex.One, Buttons.DPadLeft), new X360PadKey(PlayerIndex.One, Buttons.DPadRight), new X360PadKey(PlayerIndex.One, Buttons.A), },
            new RawKey[] { new X360PadKey(PlayerIndex.Two, Buttons.DPadLeft), new X360PadKey(PlayerIndex.Two, Buttons.DPadRight), new X360PadKey(PlayerIndex.Two, Buttons.A), },
            //new RawKey[] { RawKey.Find("a"), RawKey.Find("d"), RawKey.Find("w") },
            //new RawKey[] { RawKey.Find("LEFT"), RawKey.Find("RIGHT"), RawKey.Find("UP") },
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

            PrimitivesDrawer = new PrimitiveBatch(Root.Graphics.Device);

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
			if (state.GetButtonState(FrameAdvanceKey) == ButtonState.Pressed)
			{
				if (!FrameAdvanceKeyState)
				{
					FrameAdvanceKeyState = true;
					FrameAdvance.FrameAdvance();
				}
			}
			else
			{
				FrameAdvanceKeyState = false;
			}

			if (state.GetButtonState(UnPauseKey) == ButtonState.Pressed)
			{
				FrameAdvance.UnPause();
			}

			if (FrameAdvance.ShouldUpdateThisFrame())
			{

				for (int i = 0; i < PlayerKeys.Length; ++i)
				{
					for (int j = 0; j < PlayerKeys[i].Length; ++j)
					{
						Players[i].InputState.SetState((PlayerKey)j, state.GetButtonState(PlayerKeys[i][j]) == ButtonState.Pressed);
					}
				}
			}
        }

        public void Update()
        {
			if (FrameAdvance.ShouldUpdateThisFrame())
			{
				for (int i = 0; i < 2; ++i)
				{
					Players[i].Update(this);
				}

				FrameAdvance.Update();
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

                PrimitivesDrawer.Begin(Cameras[i].CreateOrthographicProjection(), Cameras[i].WorldToViewport * Matrix.CreateTranslation(Cameras[i].DrawOffset.X, Cameras[i].DrawOffset.Y, 0.0f));
                
                Color[] colors = new Color[] { Color.FloralWhite, Color.Aquamarine, Color.LightGreen };

                for (int j = 0; j < Players[i].collisionInfoThisFrame.Count; ++j)
                {
                    CollisionResult c = Players[i].collisionInfoThisFrame[j];
                    Vector2 lineDir = c.Normal.GetLeftNorm();
                    LineSegment segment = new LineSegment(c.Position + (lineDir * 8.0f), c.Position - (lineDir * 8.0f));

                    Color color;

                    if (j > colors.Length)
                    {
                        color = Color.Black;
                    }
                    else
                    {
                        color = colors[j];
                    }

                    PrimitivesDrawer.DrawSegment(segment.Start, segment.End, color);

                    PrimitivesDrawer.DrawPoint(c.Position, 2.0f, Color.FloralWhite);

                   
                }

                PrimitivesDrawer.End();
            }



            Drawer.Draw(onePixelTexture, new Rectangle(0, 236, 640, 8), Color.RosyBrown);

            Drawer.End();
        }
    }
}
