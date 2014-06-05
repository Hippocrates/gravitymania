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
using gravitymania.collision;

namespace gravitymania.game
{
    public class GameData
    {


        public Camera Camera;
        public Player Player;
        public TileMap Map;
        
    }

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

        public GameData[] Data;

        public InputEventManager InputEventManager;

        public RawKey[][] PlayerKeys = new RawKey[2][]
        {
            //new RawKey[] { new X360PadKey(PlayerIndex.One, Buttons.DPadLeft), new X360PadKey(PlayerIndex.One, Buttons.DPadRight), new X360PadKey(PlayerIndex.One, Buttons.A), },
            //new RawKey[] { new X360PadKey(PlayerIndex.Two, Buttons.DPadLeft), new X360PadKey(PlayerIndex.Two, Buttons.DPadRight), new X360PadKey(PlayerIndex.Two, Buttons.A), },
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
            Drawer = new SpriteBatch(Root.Graphics.Device);

            Data = new GameData[2];

            TileMap[] tempMaps = TileMapLoader.LoadFromStupidText();

            for (int i = 0; i < 2; ++i)
            {
                Data[i] = new GameData();
				Data[i].Camera = new Camera(Root.Graphics.ScreenWidth, Root.Graphics.ScreenHeight, new Rectangle(0, (int)(Root.Graphics.ScreenHeight / 2) * i, (int)Root.Graphics.ScreenWidth, (int)(Root.Graphics.ScreenHeight / 2)), DefaultFieldSize, new Vector2(DefaultFieldSize.X / 2, DefaultFieldSize.Y / 2), false, i == 1);
                Data[i].Map = tempMaps[i];
            }

            PrimitivesDrawer = new PrimitiveBatch(Root.Graphics.Device);

            onePixelTexture = new Texture2D(Root.Graphics.Device, 1, 1, false, SurfaceFormat.Color);
            onePixelTexture.SetData(new[] { Color.White });

            for (int i = 0; i < 2; ++i)
            {
                Data[i].Player = new Player(this, i, new Vector2(TileSize * 3 + TileSize / 2, TileSize * 4 + TileSize / 2), new Vector2(TileSize / 2, TileSize));
            }

            InputEventManager = new InputEventManager();

            InputEventManager.SetInputGenerator(new EventKey(EventCode.Left, InputPlayerIndex.One), new HoldEventGenerator(RawKey.Find("a")));
            InputEventManager.SetInputGenerator(new EventKey(EventCode.Right, InputPlayerIndex.One), new HoldEventGenerator(RawKey.Find("d")));
            InputEventManager.SetInputGenerator(new EventKey(EventCode.Jump, InputPlayerIndex.One), new HoldEventGenerator(RawKey.Find("w")));

            InputEventManager.SetInputGenerator(new EventKey(EventCode.Left, InputPlayerIndex.Two), new HoldEventGenerator(RawKey.Find("LEFT")));
            InputEventManager.SetInputGenerator(new EventKey(EventCode.Right, InputPlayerIndex.Two), new HoldEventGenerator(RawKey.Find("RIGHT")));
            InputEventManager.SetInputGenerator(new EventKey(EventCode.Jump, InputPlayerIndex.Two), new HoldEventGenerator(RawKey.Find("UP")));

            InputEventManager.SetInputEventListener(new EventKey(EventCode.Left, InputPlayerIndex.One), Data[(int)InputPlayerIndex.One].Player);
            InputEventManager.SetInputEventListener(new EventKey(EventCode.Right, InputPlayerIndex.One), Data[(int)InputPlayerIndex.One].Player);
            InputEventManager.SetInputEventListener(new EventKey(EventCode.Jump, InputPlayerIndex.One), Data[(int)InputPlayerIndex.One].Player);

            InputEventManager.SetInputEventListener(new EventKey(EventCode.Left, InputPlayerIndex.Two), Data[(int)InputPlayerIndex.Two].Player);
            InputEventManager.SetInputEventListener(new EventKey(EventCode.Right, InputPlayerIndex.Two), Data[(int)InputPlayerIndex.Two].Player);
            InputEventManager.SetInputEventListener(new EventKey(EventCode.Jump, InputPlayerIndex.Two), Data[(int)InputPlayerIndex.Two].Player);
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
				if (state.GetButtonState(KeyboardKey.Find("j")) == ButtonState.Pressed)
				{
					Data[0].Camera.Position += new Vector2(5.0f, 0.0f);
				}

				if (state.GetButtonState(KeyboardKey.Find("i")) == ButtonState.Pressed)
				{
                    Data[0].Camera.Position += new Vector2(0.0f, 5.0f);
				}

				if (state.GetButtonState(KeyboardKey.Find("k")) == ButtonState.Pressed)
				{
                    Data[0].Camera.Position += new Vector2(0.0f, -5.0f);
				}

				if (state.GetButtonState(KeyboardKey.Find("l")) == ButtonState.Pressed)
				{
                    Data[0].Camera.Position += new Vector2(-5.0f, 0.0f);
				}
                
                InputEventManager.RunInput(state);
			}
        }

        public void Update()
        {
			if (FrameAdvance.ShouldUpdateThisFrame())
			{
				for (int i = 0; i < 2; ++i)
				{
					Data[i].Player.Update(this);
				}

				FrameAdvance.Update();
			}
        }

        public void Draw()
        {
            Drawer.Begin();

			for (int i = 0; i < 2; ++i)
			{
                TileRange bounds = Data[i].Map.GetTileRange(Data[i].Camera.GetFieldBounds());

				for (int y = bounds.Bottom; y <= bounds.Top; ++y)
				{
					for (int x = bounds.Left; x <= bounds.Right; ++x)
					{
                        Tile t = Data[i].Map.GetTile(x, y);

						if (t.Collision == CollisionType.SolidBox)
						{
                            Rectangle box = Data[i].Camera.GetSpriteBox(Data[i].Map.GetTileBox(x, y));
							Drawer.Draw(onePixelTexture, box, Color.Tomato);
						}
					}
				}
			}

			Drawer.Draw(onePixelTexture, new Rectangle(0, (int)(Root.Graphics.ScreenHeight / 2) - 4, (int) Root.Graphics.ScreenWidth, 8), Color.RosyBrown);

			Drawer.End();

			for (int i = 0; i < 2; ++i)
			{
                PrimitivesDrawer.Begin(Matrix.Identity, Data[i].Camera.WorldToDevice);

                PrimitivesDrawer.DrawEllipse(Data[i].Player.Collision, Color.Fuchsia);

				//Players[i].Render(Drawer, Cameras[i]);

				Color[] colors = new Color[] { Color.FloralWhite, Color.Aquamarine, Color.LightGreen };

                for (int j = 0; j < Data[i].Player.collisionInfoThisFrame.Count; ++j)
				{
                    CollisionResult c = Data[i].Player.collisionInfoThisFrame[j];
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

					PrimitivesDrawer.DrawPoint(c.Position, 2.0f, Color.ForestGreen);
				}

				PrimitivesDrawer.End();
			}
        }
    }
}
