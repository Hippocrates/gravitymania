using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gravitymania.graphics
{
    public class GraphicsManager
    {
        public Microsoft.Xna.Framework.Game Owner { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public GraphicsDevice Device { get { return Graphics.GraphicsDevice; } }

        public uint ScreenWidth { get { return (uint) Device.Viewport.Width; } }
        public uint ScreenHeight { get { return (uint) Device.Viewport.Height; } }
        public bool IsFullScreen { get { return Graphics.IsFullScreen; } }

        public GraphicsManager(Microsoft.Xna.Framework.Game owner)
        {
            Owner = owner;
            Graphics = new GraphicsDeviceManager(Owner);
            Graphics.SynchronizeWithVerticalRetrace = true;
            this.SetScreenMode(DefaultScreenWidth, DefaultScreenHeight, DefaultIsFullscreen);
        }

        public void ClearScreen(Color clearColor)
        {
            Device.Clear(clearColor);     
        }

        public void SetScreenMode(uint width, uint height, bool fullscreen)
        {
            Graphics.PreferredBackBufferWidth = (int) width;
            Graphics.PreferredBackBufferHeight = (int) height;
            Graphics.IsFullScreen = fullscreen;
            Graphics.ApplyChanges();
        }

        private const uint DefaultScreenWidth = 1024;
        private const uint DefaultScreenHeight = 768;
        private const bool DefaultIsFullscreen = false;
    }
}
