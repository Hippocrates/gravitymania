using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.asset;
using Microsoft.Xna.Framework.Graphics;

namespace gravitymania.graphics
{
	// eventually we could probably sub-class this into file textures and resource textures
    public class TextureAsset : Asset<TextureAsset>
    {
        public TextureAsset(string name, string textureFile)
        {
            Name = name;
			TextureFile = textureFile;
            Texture = null;
        }

        public override bool IsLoaded()
        {
            return Texture != null;
        }

		public string TextureFile { get; private set; }
        public Texture2D Texture { get; internal set; }
    }
}
