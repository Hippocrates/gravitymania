using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.game;
using Microsoft.Xna.Framework;
using gravitymania.map;
using gravitymania.math;

namespace gravitymania.collision
{
    public interface CollisionObject
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        AABBox RoughBounds { get; }
    }

    public interface EllipseCollisionObject : CollisionObject
    {
        Ellipse Collision { get; }
    }

    public interface TilemapCollisionObject : CollisionObject
    {
        TileMap Map { get; }
    }
    
    public static class GameCollision
    {
        public static AABBox MovingBounds(this CollisionObject self)
        {
            AABBox bounds = self.RoughBounds;
            Vector2 velocity = self.Velocity;

            AABBox box = new AABBox(self.RoughBounds);
            box.AddInternalPoint(bounds.Min + velocity);
            box.AddInternalPoint(bounds.Max + velocity);
            return box;
        }

        public static CollisionResult GetFirstCollision(EllipseCollisionObject obj, GameData gameData, float currentTime = 0.0f)
        {
            return GetFirstCollision(obj, gameData, currentTime, Vector2.Zero);
        }

        public static CollisionResult GetFirstCollision(EllipseCollisionObject obj, GameData gameData, Vector2 disallowedNormal)
        {
            return GetFirstCollision(obj, gameData, 0.0f, Vector2.Zero);
        }

        public static CollisionResult GetFirstCollision(EllipseCollisionObject obj, GameData gameData, float currentTime, Vector2 disallowedNormal)
        {
            TileRange intersectedTiles = gameData.Map.GetTileRange(MovingBounds(obj));

            bool foundAny = false;
            CollisionResult bestResult = new CollisionResult() { Time = 0.0f, Hit = false, };

            foreach (TileIndex i in intersectedTiles.IterateTiles())
            {
                Vector2 offset = gameData.Map.GetTileOffset(i.X, i.Y);

                foreach (LineSegment segment in gameData.Map.GetTileGeometry(i.X, i.Y))
                {
                    CollisionResult result;

                    if (Collide.CollideEllipseWithLine(obj.Collision, obj.Velocity * (1.0f - currentTime), segment, out result))
                    {
                        if (result.Hit && (!foundAny || bestResult.Time > result.Time))
                        {
                            // This additional check is to avoid 'bumpiness' when transfering between pairs of line segments 
                            // Normally, the slightly altered normal when hitting the endpoints would cause it to treat 
                            // it as a discrete event, but since the normals are almost identical, they should not 
                            // cause a second collision event
                            if (Vector2.Dot(result.Normal, disallowedNormal) < (1.0f - 0.001f))
                            {
                                bestResult = result;
                                foundAny = true;
                            }
                        }
                    }
                }
            }

            return bestResult;
        }
    }
}
