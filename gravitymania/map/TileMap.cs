using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using gravitymania.math;
using Microsoft.Xna.Framework;

namespace gravitymania.map
{
    public enum CollisionType
    {
        Empty = 0,
        SolidBox = 1, 
		AngleBottomRight = 2,
		AngleBottomLeft = 3,
		AngleTopLeft = 4,
		AngleTopRight = 5,
    }

	class CollisionTypeGeometry
	{
		private static readonly LineSegment[][] Geometries = new LineSegment[Enum.GetNames(typeof(CollisionType)).Length][];

		static CollisionTypeGeometry()
		{
            Vector2 bottomLeft = new Vector2(0.0f * TileMap.TileSize, 0.0f * TileMap.TileSize);
            Vector2 topLeft = new Vector2(0.0f * TileMap.TileSize, 1.0f * TileMap.TileSize);
            Vector2 bottomRight = new Vector2(1.0f * TileMap.TileSize, 0.0f * TileMap.TileSize);
            Vector2 topRight = new Vector2(1.0f * TileMap.TileSize, 0.0f * TileMap.TileSize);

			Geometries[(int)CollisionType.Empty] = new LineSegment[] { };
			Geometries[(int)CollisionType.SolidBox] = new LineSegment[] 
			{ 
				new LineSegment(topLeft, topRight), 
				new LineSegment(topRight, bottomRight),
				new LineSegment(bottomRight, bottomLeft),
				new LineSegment(bottomLeft, topLeft),
			};

			Geometries[(int)CollisionType.AngleBottomRight] = new LineSegment[] 
			{ 
				new LineSegment(bottomLeft, topRight),
				new LineSegment(topRight, bottomRight),
				new LineSegment(bottomRight, bottomLeft),
			};

			Geometries[(int)CollisionType.AngleBottomLeft] = new LineSegment[] 
			{ 
				new LineSegment(topLeft, bottomRight),
				new LineSegment(bottomRight, bottomLeft),
				new LineSegment(bottomLeft, topLeft),
			};

			Geometries[(int)CollisionType.AngleTopLeft] = new LineSegment[] 
			{ 
				new LineSegment(topRight, bottomLeft),
				new LineSegment(bottomLeft, topLeft),
				new LineSegment(topLeft, topRight), 
			};

			Geometries[(int)CollisionType.AngleTopRight] = new LineSegment[] 
			{ 
				new LineSegment(bottomRight, topLeft),
				new LineSegment(topLeft, topRight), 
				new LineSegment(topRight, bottomRight),
			};
		}

		public static LineSegment[] GetGeometryTemplate(CollisionType self)
		{
			return Geometries[(int)self];
		}
	}

    public struct Tile
    {
        public CollisionType Collision;
    }

	public struct TileIndex
	{
		public readonly int X;
		public readonly int Y;
		public TileIndex(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public struct TileRange
	{
		public TileRange(int left, int bottom, int right, int top)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public readonly int Left;
		public readonly int Top;
		public readonly int Bottom;
		public readonly int Right;

		public IEnumerable<TileIndex> IterateTiles()
		{
			for (int y = Bottom; y <= Top; ++y)
			{
				for (int x = Left; x <= Right; ++x)
				{
					yield return new TileIndex(x, y);
				}
			}
		}
	}

    public class TileMap
    {
        public const int TileSize = 16;

        private Tile emptyTile = new Tile() { Collision = CollisionType.Empty };

        private Tile[] tiles;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new Tile[width * height];
        }

        private int Index2d(int x, int y)
        {
            return (y * Width) + x;
        }

        public bool InRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public Tile GetTile(int x, int y)
        {
            if (InRange(x, y))
            {
                return tiles[Index2d(x, y)];
            }
            else
            {
                return emptyTile;
            }
        }

        public void SetTile(int x, int y, Tile tile)
        {
            tiles[Index2d(x, y)] = tile;
        }

        public TileRange GetTileRange(AABBox box)
        {
            Vector2 min = box.Min / TileSize;
            Vector2 max = box.Max / TileSize;

			return new TileRange((int)Math.Floor(min.X), (int)Math.Floor(min.Y), (int)Math.Ceiling(max.X), (int)Math.Ceiling(max.Y));
        }

        public AABBox GetTileBox(int x, int y)
        {
            Vector2 location = new Vector2(x*TileSize, y*TileSize);
            Vector2 offset = new Vector2(TileSize, TileSize);
            return new AABBox(location, location + offset); 
        }

		// Just to avoid re-allocating an empty array
		private readonly static LineSegment[] EmptyArray = new LineSegment[]{};

        public Vector2 GetTileOffset(int x, int y)
        {
            return new Vector2(x * TileSize, y * TileSize);
        }

		// TODO: this is actually pretty wasteful, or just return the raw line segment and have the client do the affine xform?
		// Or maybe cache this for a certain number of recently hit tiles?
		// Whatever, this method by itself can still be used for the initial data grab, a caching system can be built on top of it.
		public LineSegment[] GetTileGeometry(int x, int y)
		{
			Tile t = GetTile(x, y);
			if (t.Collision == CollisionType.Empty)
			{
				return EmptyArray;
			}

			return CollisionTypeGeometry.GetGeometryTemplate(t.Collision);
		}
    }

    public static class TileMapLoader
    {
        public static TileMap[] LoadFromStupidText()
        {
            string file1Data =
                "0000000000000000000000000" + 
                "0000000000000000000000000" + 
                "0000000000000000000000000" + 
                "0000000000000000000000000" + 
                "0000000000000000000000000" + 
                "0000000000000000000000000" + 
                "0000000000000000011000000" + 
                "0000000000000000011000000" + 
                "0000000000000001111000000" + 
                "0000000000000001111000000" + 
                "1111111111000111111111111" + 
                "1111111111000111111111111" + 
                "1111111111000111111111111";
            string file2Data =
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000110000" +
                "0000000000000000000110000" +
                "0000000000000000000110000" +
                "0000000000000000000110000" +
                "0000000000000000000110000" +
                "0000000000000000000110000" +
                "1111111111000111111111111" +
                "1111111111000111111111111" +
                "1111111111000111111111111";

            int width = 25;
            int height = file1Data.Length / width;

            return new TileMap[] { LoadFromText(file1Data, width, height), LoadFromText(file2Data, width, height) };
        }

        public static TileMap LoadFromText(string data, int width, int height)
        {
            TileMap map = new TileMap(width, height);

            int i = 0;

            for (int y = height - 1; y >= 0; --y)
            {
                for (int x = 0; x < width; ++x)
                {
                    map.SetTile(x, y, new Tile() { Collision = ((CollisionType)data[i] - 0x30) });
                    ++i;
                }
            }

            return map;
        }
    }
}
