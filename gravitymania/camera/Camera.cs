using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using gravitymania.math;

namespace gravitymania.camera
{
	public class Camera
	{
        public Matrix WorldToViewport { get; private set; }
        public Matrix ViewportToWorld
        {
            get
            {
                return Matrix.Invert(WorldToViewport);
            }
        }

        public Vector2 ViewportSize
        {
            get { return _ViewportSize; }
            set
            {
                _ViewportSize = value;
                RecalculateWorldToViewMatrix();
            }
        }

        public Vector2 FieldSize
        {
            get { return _FieldSize; }
            set
            {
                _FieldSize = value;
                RecalculateWorldToViewMatrix();
            }
        }

        public Vector2 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                RecalculateWorldToViewMatrix();
            }
        }

        public Vector2 DrawOffset
        {
            get
            {
                return _DrawOffset;
            }
            set
            {
                _DrawOffset = value;
            }
        }

        private bool InvertY;

        /// <summary>
        /// Build a new camera given the physical viewport in pixels and the field of view size in game units.
        /// </summary>
        public Camera(Vector2 viewportSize, Vector2 fieldSize, Vector2 initialPosition, Vector2 drawOffset = new Vector2(), bool invertY = false)
        {
            _ViewportSize = viewportSize;
            _FieldSize = fieldSize;
            _Position = initialPosition;
            _DrawOffset = drawOffset;
            InvertY = invertY;

            RecalculateWorldToViewMatrix();
        }

        /// <summary>
        /// Takes a world-space vector and transforms it into screen co-ordinates
        /// </summary>
        public Vector2 TransformToView(Vector2 position)
        {
            return Vector2.Transform(position, WorldToViewport) + DrawOffset;
        }

        public Vector2 GetViewLockPosition(Vector2 targetPosition, Vector2 screenLock)
        {
            return targetPosition - (TransformToWorld(screenLock) - Position);
        }


        /// <summary>
        /// Takes a screen-space vector and transforms it into world co-ordinates
        /// </summary>
        public Vector2 TransformToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, ViewportToWorld) - DrawOffset;
        }

        public AABBox GetFieldBounds()
        {
            return new AABBox(Position - (FieldSize / 2.0f), Position + (FieldSize / 2.0f));
        }

        /// <summary>
        /// Takes a world-space AABB and transforms it into a screen rectangle
        /// </summary>
        public Rectangle TransformToView(AABBox fieldBounds)
        {
            // This method needs to compensate for the fact that I'm fliping the y co-ords
            // to be the opposite of screen co-ords
            Vector2 l = TransformToView(fieldBounds.Min);
            Vector2 u = TransformToView(fieldBounds.Max);

            Vector2 lower;
            Vector2 upper;

            if (InvertY)
            {
                lower = new Vector2(l.X, l.Y);
                upper = new Vector2(u.X, u.Y);
            }
            else
            {
                lower = new Vector2(l.X, u.Y);
                upper = new Vector2(u.X, l.Y);
            }

            return new Rectangle((int)lower.X, (int)lower.Y, (int)(upper.X - lower.X), (int)(upper.Y - lower.Y));
        }

        private void RecalculateWorldToViewMatrix()
        {
            float xScale = ViewportSize.X / FieldSize.X;
            float yScale = ViewportSize.Y / FieldSize.Y;

            Vector2 screenCenter = new Vector2(ViewportSize.X / 2f, ViewportSize.Y / 2f);

            if (InvertY)
            {
                WorldToViewport = Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) * Matrix.CreateScale(xScale, yScale, 1.0f) * Matrix.CreateTranslation(new Vector3(screenCenter, 0.0f));
            }
            else
            {
                WorldToViewport = Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) * Matrix.CreateScale(xScale, -yScale, 1.0f) * Matrix.CreateTranslation(new Vector3(screenCenter, 0.0f));
            }
        }

        public Matrix CreateOrthographicProjection()
        {
            return Matrix.CreateOrthographicOffCenter(0f, ViewportSize.X, ViewportSize.Y, 0f, 0f, 1f);
        }

        private Vector2 _Position;
        private Vector2 _ViewportSize;
        private Vector2 _FieldSize;
        private Vector2 _DrawOffset;
    }
}

