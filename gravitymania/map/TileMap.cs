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
    }

    public struct Tile
    {
        public CollisionType Collision;
    }

    public class TileMap
    {
        public readonly int TileSize = 16;

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

        public Tuple<int, int, int, int> GetTileBounds(AABBox box)
        {
            Vector2 min = box.Min / TileSize;
            Vector2 max = box.Max / TileSize;

            return new Tuple<int, int, int, int>((int)Math.Floor(min.X), (int)Math.Floor(min.Y), (int)Math.Ceiling(max.X), (int) Math.Ceiling(max.Y));
        }

        public AABBox GetTileBox(int x, int y)
        {
            Vector2 location = new Vector2(x*TileSize, y*TileSize);
            Vector2 offset = new Vector2(TileSize, TileSize);
            return new AABBox(location, location + offset); 
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
