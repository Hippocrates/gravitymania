using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.asset
{
	// TODO: add reference to owning manager, and methods to grab, drop, query state, and all that jazz
	// Curiously recursive template pattern ftw!
    public abstract class Asset<A> : IComparable<Asset<A>> where A : Asset<A>
    {
        public string Name { get; internal set; }
        public int RefCount { get; internal set; }
        public AssetManager<A> Manager { get; internal set; }

		public void Grab()
		{
			Manager.GrabAsset(this as A);
		}

		public void Drop()
		{
			Manager.DropAsset(this as A);
		}

        public abstract bool IsLoaded();

        public int CompareTo(Asset<A> other)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(Name, other.Name);
        }
    }
}
