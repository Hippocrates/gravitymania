using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.asset;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml;

namespace gravitymania.graphics
{
    public class TextureAssetManager : AssetManager<TextureAsset>
    {
		public GraphicsDevice Graphics { get; set; }

        public TextureAssetManager()
        {
        }

        protected override void Load(TextureAsset resource)
        {
			resource.Texture = Texture2D.FromStream(Graphics, new FileStream(resource.TextureFile, FileMode.Open));
        }

        protected override void UnLoad(TextureAsset resource)
        {
            resource.Texture.Dispose();
            resource.Texture = null;
        }
    }
}
