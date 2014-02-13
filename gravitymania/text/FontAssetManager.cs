using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using gravitymania.asset;
using Microsoft.Xna.Framework.Graphics;

namespace gravitymania.text
{
	public class FontAssetManager : AssetManager<FontAsset>
	{
		public ContentManager Content { get; set; }

		public FontAssetManager()
		{
		}

		protected override void Load(FontAsset resource)
		{
			resource.Font = Content.Load<SpriteFont>(resource.FontName);
		}

		protected override void UnLoad(FontAsset resource)
		{
			resource.Font = null;
		}
	}
}
