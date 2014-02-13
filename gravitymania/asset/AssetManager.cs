using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.asset
{
	/// <summary>
	/// Provides a generic way to manage the loading and unloading of assets dynamically based on a reference counter
	/// </summary>
	/// <typeparam name="A"></typeparam>
    public abstract class AssetManager<A> where A : Asset<A>
    {
        abstract protected void Load(A asset);
        abstract protected void UnLoad(A asset);

        private Dictionary<string, A> AssetMap = new Dictionary<string, A>(StringComparer.OrdinalIgnoreCase);
        private HashSet<A> ToLoad = new HashSet<A>();
        private HashSet<A> ToUnLoad = new HashSet<A>();

        public AssetManager()
        {
        }

		// Auto-load/unload all assets based on current use-counts
        public void RefreshAssets()
        {
            foreach (var asset in ToUnLoad)
            {
                if (asset.RefCount == 0 && asset.IsLoaded())
                {
                    UnLoad(asset);
                }
            }

            ToUnLoad.Clear();

            foreach (var asset in ToLoad)
            {
                if (asset.RefCount > 0 && !asset.IsLoaded())
                {
                    Load(asset);
                }
            }

            ToLoad.Clear();
        }

        public bool HasAsset(string name)
        {
            return AssetMap.ContainsKey(name);
        }

        public void AddAsset(string name, A asset)
        {
            if (HasAsset(name))
            {
                throw new Exception("asset '" + name + "' already exists.");
            }
            else
            {
				asset.Manager = this;
                asset.Name = name;
                asset.RefCount = 0;
                AssetMap[name] = asset;
            }
        }

        public void RemoveAsset(string name)
        {
			A asset = AssetMap[name];

			if (asset.RefCount > 0)
			{
				UnLoad(asset);
			}

            if (!AssetMap.Remove(name))
            {
                throw new Exception("asset '" + name + "' does not exist.");
            }

			asset.RefCount = 0;
			asset.Manager = null;
        }

        public A GetAsset(string name)
        {
            A result = null;
            if (AssetMap.TryGetValue(name, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<A> AllAssets()
        {
            return AssetMap.Values;
        }

        public A GrabAsset(string name, bool immediate = false)
        {
            A asset = GetAsset(name);

            GrabAsset(asset, immediate);

			return asset;
        }

        public void GrabAsset(A asset, bool immediate = false)
		{
            if (immediate && !asset.IsLoaded())
            {
                this.Load(asset);

                if (ToLoad.Contains(asset))
                {
                    ToLoad.Remove(asset);
                }

                if (ToUnLoad.Contains(asset))
                {
                    ToUnLoad.Remove(asset);
                }
            }
            else if (asset.RefCount == 0)
			{
                if (ToUnLoad.Contains(asset))
                {
                    ToUnLoad.Remove(asset);
                }
                else
                {
                    ToLoad.Add(asset);
                }
			}

			++asset.RefCount;
		}

        public void DropAsset(string name)
        {
            A asset = GetAsset(name);

            DropAsset(asset);
        }

        public void DropAsset(A asset)
		{
			if (asset.RefCount == 1)
			{
                if (ToLoad.Contains(asset))
                {
                    ToLoad.Remove(asset);
                }
                else
                {
                    ToUnLoad.Add(asset);
                }
			}

			--asset.RefCount;
		}
    }
}
