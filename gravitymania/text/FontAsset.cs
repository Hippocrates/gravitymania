using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.asset;
using Microsoft.Xna.Framework.Graphics;

namespace gravitymania.text
{
	public class FontAsset : Asset<FontAsset>
	{
		public FontAsset(string name, string fontName)
		{
			Name = name;
			FontName = fontName;
			Font = null;
		}

        public override bool IsLoaded()
        {
            return Font != null;
        }

		public SpriteFont Font { get; internal set; }
		public string FontName { get; internal set; }
	}
}
