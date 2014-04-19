using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using gravitymania.math;

namespace gravitymania.camera
{
	public static class CameraUtil
	{
		public static Matrix ScreenOffsetMatrix(uint screenWidth, uint screenHeight, Rectangle bounds)
		{
			float xScale = (float)bounds.Width / (float)screenWidth;
			float yScale = (float)bounds.Height / (float)screenHeight;

			float xDisplacement = ((float)bounds.Left + ((float)bounds.Width / 2.0f) / (float)screenWidth) * 2.0f;
			float yDisplacement = (((float)bounds.Top + ((float)bounds.Height / 2.0f)) / (float)screenHeight) * 2.0f;

			return Matrix.CreateScale(xScale, yScale, 1.0f) * Matrix.CreateTranslation(1.0f - xDisplacement, 1.0f - yDisplacement, 0.0f);
		}

		public static Matrix DeviceToScreenMatrix(uint screenWidth, uint screenHeight)
		{
			float halfWidth = (float)screenWidth / 2.0f;
			float halfHeight = (float)screenHeight / 2.0f;
			return Matrix.CreateScale(halfWidth, -halfHeight, 1.0f) * Matrix.CreateTranslation(halfWidth, halfHeight, 0.0f);
		}

		public static Matrix ScreenToDeviceMatrix(uint screenWidth, uint screenHeight)
		{
			return Matrix.Invert(DeviceToScreenMatrix(screenWidth, screenHeight));
		}

		public static Matrix WorldToDeviceMatrix(Vector2 viewField, Vector2 position)
		{
			Vector2 scale = new Vector2(2.0f / viewField.X, 2.0f / viewField.Y);
			return Matrix.CreateTranslation(-position.X, -position.Y, 1.0f) * Matrix.CreateScale(scale.X, scale.Y, 1.0f);
		}

		public static Matrix DeviceToWorldMatrix(Vector2 fieldSize, Vector2 position)
		{
			return Matrix.Invert(WorldToDeviceMatrix(fieldSize, position));
		}
	}


	public class Camera
	{
		public uint ScreenWidth;
		public uint ScreenHeight;
		public Rectangle Viewport;
		public Vector2 Position;
		public Vector2 ViewField;
		public bool FlipX;
		public bool FlipY;

		public Matrix WorldToDevice
		{
			get
			{
				return WorldToDeviceRaw * CameraUtil.ScreenOffsetMatrix(ScreenWidth, ScreenHeight, Viewport);
			}
		}

		public Matrix WorldToDeviceRaw
		{
			get
			{
				return CameraUtil.WorldToDeviceMatrix(ViewField, Position) * Matrix.CreateScale(FlipX ? -1.0f : 1.0f, FlipY ? -1.0f : 1.0f, 1.0f);
			}
		}

		public Matrix DeviceToWorldRaw
		{
			get
			{
				return Matrix.Invert(WorldToDeviceRaw);
			}
		}

		public Matrix DeviceToWorld
		{
			get
			{
				return Matrix.Invert(WorldToDevice);
			}
		}

		public Matrix WorldToScreen
		{
			get
			{
				return WorldToDevice * CameraUtil.DeviceToScreenMatrix(ScreenWidth, ScreenHeight);
			}
		}

		public Matrix ScreenToWorld
		{
			get
			{
				return Matrix.Invert(WorldToScreen);
			}
		}

		public Camera(uint screenWidth, uint screenHeight, Rectangle viewport, Vector2 viewField, Vector2 initialPosition, bool flipX = false, bool flipY = false)
        {
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;
			Viewport = viewport;
			ViewField = viewField;
			Position = initialPosition;
			FlipX = flipX;
			FlipY = flipY;
        }

        public AABBox GetFieldBounds()
        {
			return new AABBox(Position - (ViewField / 2.0f), Position + (ViewField / 2.0f));
        }

        /// <summary>
        /// Takes a world-space AABB and transforms it into a screen rectangle
        /// </summary>
        public Rectangle GetSpriteBox(AABBox fieldBounds)
        {
			Vector2 temp = Vector2.Transform(fieldBounds.Min, WorldToDevice);

            // This method needs to compensate for the fact that I'm fliping the y co-ords
            // to be the opposite of screen co-ords
			Vector2 lower = Vector2.Transform(fieldBounds.Min, WorldToScreen);
			Vector2 upper = Vector2.Transform(fieldBounds.Max, WorldToScreen);

			Vector2 l = new Vector2(Math.Min(lower.X, upper.X), Math.Min(lower.Y, upper.Y));
			Vector2 u = new Vector2(Math.Max(lower.X, upper.X), Math.Max(lower.Y, upper.Y));

			return new Rectangle(MathUtil.RealRound(l.X), MathUtil.RealRound(l.Y), MathUtil.RealRound(u.X - l.X), MathUtil.RealRound(u.Y - l.Y));
        }
    }
}

