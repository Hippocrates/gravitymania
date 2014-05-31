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

	public enum TileDirection
	{
		Up = 0x00,
		Down = 0x01,
		Left = 0x02,
		Right = 0x03,
	}

	public static class TileDirectionMethods
	{
		private static readonly TileIndex[] Directions = new TileIndex[4]
		{
			new TileIndex(0, 1),
			new TileIndex(0, -1),
			new TileIndex(-1, 0),
			new TileIndex(1, 0),
		};

		public static TileIndex GetOffset(this TileDirection direction)
		{
			return Directions[(int)direction];
		}

		public static TileDirection GetOpposite(this TileDirection direction)
		{
			if ((int)direction % 2 == 1)
			{
				return (TileDirection)((int)direction - 1);
			}
			else
			{
				return (TileDirection)((int)direction + 1);
			}
			//return (TileDirection)((int)direction / 2) + (((int)direction + 1) % 2);
		}

		public static TileIndex GetOppositeOffset(this TileDirection direction)
		{
			return Directions[(int)direction.GetOpposite()];
		}
	}

	public class CollisionTypeGeometry
	{
		private bool[] Solidity;
		private LineSegment[] Geometry;
		private TileDirection?[] Associations;

		CollisionTypeGeometry(LineSegment[] geometry, TileDirection?[] associations, bool[] solidity)
		{
			Solidity = solidity.ToArray();
			Geometry = geometry.ToArray();
			Associations = associations.ToArray();
		}

		public int NumGeometries()
		{
			return Geometry.Length;
		}

		public bool SolidInDirection(TileDirection direction)
		{
			return Solidity[(int)direction];
		}

		public LineSegment GetGeometry(int i)
		{
			return Geometry[i];
		}

		public TileDirection? DirectionalAssociation(int i)
		{
			return Associations[i];
		}

		private static readonly CollisionTypeGeometry[] CollisionInfo = new CollisionTypeGeometry[Enum.GetNames(typeof(CollisionType)).Length];

		static CollisionTypeGeometry()
		{
            Vector2 bottomLeft = new Vector2(0.0f * TileMap.TileSize, 0.0f * TileMap.TileSize);
            Vector2 topLeft = new Vector2(0.0f * TileMap.TileSize, 1.0f * TileMap.TileSize);
            Vector2 bottomRight = new Vector2(1.0f * TileMap.TileSize, 0.0f * TileMap.TileSize);
            Vector2 topRight = new Vector2(1.0f * TileMap.TileSize, 1.0f * TileMap.TileSize);

			CollisionInfo[(int)CollisionType.Empty] = new CollisionTypeGeometry(new LineSegment[] { }, new TileDirection?[] { }, new bool[] { false, false, false, false });

			CollisionInfo[(int)CollisionType.SolidBox] = new CollisionTypeGeometry(
				new LineSegment[]
				{ 
					new LineSegment(topLeft, topRight),
					new LineSegment(bottomRight, bottomLeft),
					new LineSegment(bottomLeft, topLeft),
					new LineSegment(topRight, bottomRight),
				},
				new TileDirection?[]
				{
					TileDirection.Up,
					TileDirection.Down,
					TileDirection.Left,
					TileDirection.Right,
				},
				new bool[]
				{
					true, true, true, true,
				});

			CollisionInfo[(int)CollisionType.AngleBottomRight] = new CollisionTypeGeometry(
				new LineSegment[] 
				{ 
					new LineSegment(bottomLeft, topRight),
					new LineSegment(topRight, bottomRight),
					new LineSegment(bottomRight, bottomLeft),
				},
				new TileDirection?[]
				{
					null,
					TileDirection.Right,
					TileDirection.Down,
				},
				new bool[]
				{
					false, true, false, true,
				});

			CollisionInfo[(int)CollisionType.AngleBottomLeft] = new CollisionTypeGeometry(
				new LineSegment[] 
				{ 
					new LineSegment(topLeft, bottomRight),
					new LineSegment(bottomRight, bottomLeft),
					new LineSegment(bottomLeft, topLeft),
				},
				new TileDirection?[]
				{
					null,
					TileDirection.Down,
					TileDirection.Left,
				},
				new bool[]
				{
					false, true, true, false,
				});

			CollisionInfo[(int)CollisionType.AngleTopLeft] = new CollisionTypeGeometry(
				new LineSegment[] 
				{ 
					new LineSegment(topRight, bottomLeft),
					new LineSegment(bottomLeft, topLeft),
					new LineSegment(topLeft, topRight), 
				},
				new TileDirection?[]
				{
					null,
					TileDirection.Left,
					TileDirection.Up,
				},
				new bool[]
				{
					true, false, true, false,
				});

			CollisionInfo[(int)CollisionType.AngleTopRight] = new CollisionTypeGeometry(
				new LineSegment[] 
				{ 
					new LineSegment(bottomRight, topLeft),
					new LineSegment(topLeft, topRight), 
					new LineSegment(topRight, bottomRight),
				},
				new TileDirection?[]
				{
					null,
					TileDirection.Up,
					TileDirection.Right,
				},
				new bool[]
				{
					true, false, false, true,
				});
		}

		public static CollisionTypeGeometry GetGeometryTemplate(CollisionType self)
		{
			return CollisionInfo[(int)self];
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

        private Tile emptyTile = new Tile() { Collision = CollisionType.SolidBox };

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

		private CollisionTypeGeometry GetTileGeometryTemplate(int x, int y)
		{
			return CollisionTypeGeometry.GetGeometryTemplate(GetTile(x, y).Collision);
		}

		public IEnumerable<LineSegment> GetTileGeometry(int x, int y)
		{
			Tile t = GetTile(x, y);

			if (t.Collision != CollisionType.Empty)
			{
				Vector2 offset = GetTileOffset(x, y);

				CollisionTypeGeometry geometry = CollisionTypeGeometry.GetGeometryTemplate(t.Collision);

				// This removes 'unncessary' lines when adjacent to solid tiles, saves time, and removes some collision artifacts
				for (int i = 0; i < geometry.NumGeometries(); ++i)
				{
					TileDirection? direction = geometry.DirectionalAssociation(i);

					bool solidAdjacent = false;

					if (direction != null)
					{
						TileDirection d = direction ?? TileDirection.Up;
						TileIndex idx = d.GetOffset();
						solidAdjacent = GetTileGeometryTemplate(x + idx.X, y + idx.Y).SolidInDirection(d.GetOpposite());
					}

					if (!solidAdjacent)
					{
						LineSegment segment = geometry.GetGeometry(i);
						yield return new LineSegment(segment.Start + offset, segment.End + offset);
					}
					else
					{
						//Console.WriteLine("Look at the savings!");
					}
				}
			}
		}
    }

    public static class TileMapLoader
    {
        public static TileMap[] LoadFromStupidText()
        {
            /*
            string file1Data =
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000005111" +
				"000000000000000000000000000511" +
				"000000000000000000000000000051" +
				"000000000000000001100000000005" +
				"000000000000000001100000000000" +
				"000000000000000111110000000211" + 
                "222222222200000111110000002111" + 
                "111111111100011111113000021111" + 
                "111111111100011111111300211111" + 
                "111111111100011111111111111111";
            string file2Data =
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
				"000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000011110000000011000000000" +
				"000000011110000000011000000000" +
				"000000011110000000011000000000" +
				"000000000000000000011000000000" +
				"000000000000000000011100000000" +
				"000000000000000000011100000000" +
				"000000000000000000011110000000" +
				"222222222200000000011110000000" +
                "111111111100011111111111130000" +
                "111111111100011111111111113000" +
                "111111111100011111111111111300";
            */

            string file1Data =
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000001100000000000" +
                "000000000000000001100000000000" +
                "000000000000000111110000000211" +
                "000000000000000111110000002111" +
                "111111111100011111113000021111" +
                "111111111100011111111300211111" +
                "111111111100011111111111111111";
            string file2Data =
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000000000000000000000000000" +
                "000000011110000000011000000000" +
                "000000011110000000011000000000" +
                "000000011110000000011000000000" +
                "000000000000000000011000000000" +
                "000000000000000000011100000000" +
                "000000000000000000011100000000" +
                "000000000000000000011110000000" +
                "000000000000000000011110000000" +
                "111111111100011111111111130000" +
                "111111111100011111111111113000" +
                "111111111100011111111111111300";
            int width = 30;
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
