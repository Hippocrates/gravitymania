using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania;
using gravitymania.asset;

namespace gravitymaniaTest.asset
{
	public class DummyAsset: Asset<DummyAsset>
	{
		public bool active = false;

        public override bool IsLoaded()
        {
            return active;
        }
    }

	public class DummyAssetManager : AssetManager<DummyAsset>
	{
		protected override void Load(DummyAsset asset)
		{
			asset.active = true;
		}

		protected override void UnLoad(DummyAsset asset)
		{
			asset.active = false;
		}
	}

	[TestFixture]
	public class TestAssetManager
	{
		[Test]
		public void TestBasicAssetManagment()
		{
			DummyAsset d1 = new DummyAsset();
			DummyAsset d2 = new DummyAsset();

			DummyAssetManager manager = new DummyAssetManager();

			manager.AddAsset("d1", d1);

			Assert.AreEqual(1, manager.AllAssets().Count());

			Assert.AreEqual(d1.Name, "d1");
			Assert.AreSame(d1, manager.GetAsset("d1"));
			Assert.IsFalse(d1.active);

			d1.Grab();

			Assert.AreEqual(1, d1.RefCount);
			Assert.IsFalse(d1.active);

			manager.RefreshAssets();

			Assert.AreEqual(1, d1.RefCount);
			Assert.IsTrue(d1.active);

			d1.Grab();

			Assert.AreEqual(2, d1.RefCount);
			Assert.IsTrue(d1.active);

			manager.RefreshAssets();

			Assert.AreEqual(2, d1.RefCount);
			Assert.IsTrue(d1.active);

			d1.Drop();

			Assert.AreEqual(1, d1.RefCount);
			Assert.IsTrue(d1.active);

			manager.RefreshAssets();

			Assert.AreEqual(1, d1.RefCount);
			Assert.IsTrue(d1.active);

			d1.Drop();

			Assert.AreEqual(0, d1.RefCount);
			Assert.IsTrue(d1.active);

			manager.RefreshAssets();

			Assert.AreEqual(0, d1.RefCount);
			Assert.IsFalse(d1.active);

			manager.AddAsset("d2", d2);

			Assert.AreEqual("d2", d2.Name);
			Assert.AreEqual(2, manager.AllAssets().Count());

			Assert.AreSame(d2, manager.GetAsset("d2"));
			Assert.IsFalse(d2.active);

			d2.Grab();

			Assert.AreEqual(1, d2.RefCount);
			Assert.IsFalse(d2.active);
			Assert.IsFalse(d1.active);

			manager.RefreshAssets();

			Assert.AreEqual(1, d2.RefCount);
			Assert.IsTrue(d2.active);

			d2.Drop();
			d1.Grab();

			manager.RefreshAssets();

			Assert.AreEqual(1, d1.RefCount);
			Assert.IsTrue(d1.active);
			Assert.AreEqual(0, d2.RefCount);
			Assert.IsFalse(d2.active);

			manager.RemoveAsset("d1");

			Assert.AreEqual(1, manager.AllAssets().Count());

			Assert.IsNull(manager.GetAsset("d1"));

			Assert.AreEqual(0, d1.RefCount);
			Assert.IsFalse(d1.active);
		}
	}
}
