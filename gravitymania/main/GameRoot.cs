using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using gravitymania.console;
using gravitymania.graphics;
using System.IO;
using gravitymania.main;
using gravitymania.input;
using gravitymania.camera;
using gravitymania.text;
using gravitymania.game;
using gravitymania.mapedit;

namespace gravitymania
{
    /// <summary>
    /// Controls all System-level game resources
	/// TODO: abstract everything as much as possible, so that 
	/// the programs can just sub-class this, and get at whatever they need.
	/// Add the 'resource' systems, i.e. have them be set-up appropriately
    /// </summary>
    public class GameRoot : Microsoft.Xna.Framework.Game
    {
        public GraphicsManager Graphics { get; private set; }
        public GameConsole Console { get; private set; }
        public ConsoleDispatcher ConsoleDispatcher { get; private set; }

        // These potentially belong to a different level of abstraction
		public FontAssetManager FontManager { get; private set; }
		public TextureAssetManager TextureManager { get; private set; }

        private RootConsoleExecutor DefaultCommands;
        private GameState CurrentState;
        private MainGame MainGame;
        private MapEditor MapEditor;

        public GameRoot()
        {
            Graphics = new GraphicsManager(this);
            IsFixedTimeStep = true;
            // uses slightly more than 60 to avoid the integer rounding cutoff
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60.1f);

            ConsoleDispatcher = new ConsoleDispatcher();
            Console = new GameConsole();
            Console.CommandEntered += ConsoleDispatcher.RunDispatchers;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void BeginRun()
        {
            DefaultCommands = new RootConsoleExecutor(this);
            ConsoleDispatcher.PushDispatcher(DefaultCommands);

            this.SwitchToGame();

            base.BeginRun();
        }

        protected override void EndRun()
        {
            if (MainGame != null)
            {
                MainGame.End();
            }

            if (MapEditor != null)
            {
                MapEditor.End();
            }

            base.EndRun();
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            if (CurrentState != null)
            {
                CurrentState.Resume();
            }

            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            if (CurrentState != null)
            {
                CurrentState.Suspend();
            }

            base.OnActivated(sender, args);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";

			// This should be replaced by appropriate 'resource' system calls
            Console.LoadContent(Graphics.Device, Content);
			FontManager = new FontAssetManager() { Content = Content };
			TextureManager = new TextureAssetManager() { Graphics = Graphics.Device };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputState state = InputState.ReadCurrentState();

            Console.Update(state.Keys);

            if (CurrentState != null)
            {
                if (!Console.IsOpen)
                {
                    CurrentState.Input(state);
                }

                CurrentState.Update();
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.  Note that I am ignoring game time since I'm using 
        /// fixed-step timing in this game
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			Graphics.Device.RasterizerState = RasterizerState.CullNone;

            // Ensure the camera is reporting the correct viewport size
            //camera.ViewportSize = new Vector2(Graphics.Device.Viewport.Width, Graphics.Device.Viewport.Height);

			// This should be moved into the 'game speficic' area, and the console should probably be responsible for creating its own sprite batch
            Graphics.ClearScreen(Color.Black);

            if (CurrentState != null)
            {
                CurrentState.Draw();
            }

            Console.Draw();

            base.Draw(gameTime);
        }

        public void SwitchToEditor()
        {
            if (MapEditor == null)
            {
                MapEditor = new MapEditor(this);
                MapEditor.Begin();
            }

            this.SwitchToState(MapEditor);
        }

        public void SwitchToGame()
        {
            if (MainGame == null)
            {
                MainGame = new MainGame(this);
                MainGame.Begin();
            }

            this.SwitchToState(MainGame);
        }

        private void SwitchToState(GameState state)
        {
            if (CurrentState != null)
            {
                CurrentState.Suspend();
            }

            CurrentState = state;
            CurrentState.Resume();
        }
    }
}
