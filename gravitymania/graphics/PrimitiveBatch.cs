﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gravitymania.math;

namespace gravitymania.graphics
{
    public class TriangleBatch<V> where V : struct, IVertexType
    {
        public int BufferSize { get; set; }
        public Matrix Projection { get; set; }
        public Matrix View { get; set; }
        public Texture2D Texture { get; set; }
        public bool ColorEnabled { get; set; }

        private GraphicsDevice Graphics;
        private V[] VertexBuffer;
        private int BufferLocation;
        private BasicEffect Effect;
        private bool Started;

        private SamplerState Sampler;

        public TriangleBatch(GraphicsDevice graphics, int bufferSize = 20)
        {
            BufferSize = bufferSize;
            Graphics = graphics;
            Effect = new BasicEffect(Graphics);
            Started = false;

            Sampler = new SamplerState()
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Linear,
            };
        }

        private void ResetBuffer()
        {
            if (VertexBuffer == null || VertexBuffer.Length != BufferSize * 3)
            {
                VertexBuffer = new V[BufferSize * 3];
            }

            BufferLocation = 0;
        }

        public void Begin()
        {
            if (Started)
            {
                throw new Exception("Error, already started");
            }
            else
            {
                ResetBuffer();
                Effect.Projection = Projection;
                Effect.View = View;
                Effect.Texture = Texture;
                Effect.TextureEnabled = Texture != null;
                Effect.VertexColorEnabled = true;
                Started = true;

                Effect.CurrentTechnique.Passes[0].Apply();
                Graphics.SamplerStates[0] = Sampler;
            }
        }

        public void End()
        {
            Flush();
            Started = false;
        }

        public void Flush()
        {
            Graphics.DrawUserPrimitives(PrimitiveType.TriangleList, VertexBuffer, 0, BufferLocation / 3);
        }

        public void AddVertex(V vertex)
        {
            if (Started)
            {
                VertexBuffer[BufferLocation] = vertex;
                ++BufferLocation;

                if (BufferLocation == VertexBuffer.Length)
                {
                    Flush();
                }
            }
            else
            {
                throw new Exception("Error, not started");
            }
        }
    }

    public class PrimitiveBatch : IDisposable
    {

#if XBOX || WINDOWS_PHONE
        public const int CircleSegments = 16;
#else
        public const int CircleSegments = 32;
#endif

        private const int DefaultBufferSize = 500;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        private BasicEffect _basicEffect;

        // the device that we will issue draw calls to.
        private GraphicsDevice _device;

        // hasBegun is flipped to true once Begin is called, and is used to make
        // sure users don't call End before Begin is called.
        private bool _hasBegun;

        private bool _isDisposed;
        private VertexPositionColor[] _lineVertices;
        private int _lineVertsCount;
        private VertexPositionColor[] _triangleVertices;
        private int _triangleVertsCount;


        /// <summary>
        /// the constructor creates a new PrimitiveBatch and sets up all of the internals
        /// that PrimitiveBatch will need.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, DefaultBufferSize)
        {
        }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            _device = graphicsDevice;

            _triangleVertices = new VertexPositionColor[bufferSize - bufferSize % 3];
            _lineVertices = new VertexPositionColor[bufferSize - bufferSize % 2];

            // set up a new basic effect, and enable vertex colors.
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.VertexColorEnabled = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void SetProjection(ref Matrix projection)
        {
            _basicEffect.Projection = projection;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (_basicEffect != null)
                    _basicEffect.Dispose();

                _isDisposed = true;
            }
        }


        /// <summary>
        /// Begin is called to tell the PrimitiveBatch what kind of primitives will be
        /// drawn, and to prepare the graphics card to render those primitives.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="view">The view.</param>
        public void Begin(Matrix projection, Matrix view)
        {
            if (_hasBegun)
            {
                throw new InvalidOperationException("End must be called before Begin can be called again.");
            }

            //tell our basic effect to begin.
            _basicEffect.Projection = projection;
            _basicEffect.View = view;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            _hasBegun = true;
        }

        public bool IsReady()
        {
            return _hasBegun;
        }

		private Vector2[] MakeBoxVerts(AABBox box)
		{
			return new Vector2[] { box.Min, new Vector2(box.Max.X, box.Min.Y), box.Max, new Vector2(box.Min.X, box.Max.Y), };
		}

		public void DrawBox(AABBox box, float red, float green, float blue)
		{
			DrawPolygon(MakeBoxVerts(box), red, green, blue);
		}

		public void DrawBox(AABBox box, Color color)
		{
			DrawPolygon(MakeBoxVerts(box), color);
		}

		public void DrawSolidBox(AABBox box, float red, float green, float blue, bool outline = false)
		{
			DrawSolidPolygon(MakeBoxVerts(box), red, green, blue);
		}

		public void DrawSolidBox(AABBox box, Color color, bool outline = false)
		{
			DrawSolidPolygon(MakeBoxVerts(box), color, outline);
		}

		public void DrawPolygon(Vector2[] vertices, float red, float green, float blue)
		{
			DrawPolygon(vertices, vertices.Length, red, green, blue);
		}

		public void DrawPolygon(Vector2[] vertices, Color color)
		{
			DrawPolygon(vertices, vertices.Length, color);
		}

        public void DrawPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawPolygon(vertices, count, new Color(red, green, blue));
        }

        public void DrawPolygon(Vector2[] vertices, int count, Color color)
        {
            if (!this.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            for (int i = 0; i < count - 1; i++)
            {
                this.AddVertex(vertices[i], color, PrimitiveType.LineList);
                this.AddVertex(vertices[i + 1], color, PrimitiveType.LineList);
            }

            this.AddVertex(vertices[count - 1], color, PrimitiveType.LineList);
            this.AddVertex(vertices[0], color, PrimitiveType.LineList);
        }

		public void DrawSolidPolygon(Vector2[] vertices, float red, float green, float blue, bool outline = false)
        {
            DrawSolidPolygon(vertices, vertices.Length, new Color(red, green, blue), outline);
        }

		public void DrawSolidPolygon(Vector2[] vertices, Color color, bool outline = false)
		{
			DrawSolidPolygon(vertices, vertices.Length, color, outline);
		}

		public void DrawSolidPolygon(Vector2[] vertices, int count, float red, float green, float blue, bool outline = false)
        {
			DrawSolidPolygon(vertices, count, new Color(red, green, blue), outline);
        }

        public void DrawSolidPolygon(Vector2[] vertices, int count, Color color, bool outline = false)
        {
            if (!this.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            if (count == 2)
            {
                DrawPolygon(vertices, count, color);
                return;
            }

            Color colorFill = color * (outline ? 0.5f : 1.0f);

            for (int i = 1; i < count - 1; i++)
            {
                this.AddVertex(vertices[0], colorFill, PrimitiveType.TriangleList);
                this.AddVertex(vertices[i], colorFill, PrimitiveType.TriangleList);
                this.AddVertex(vertices[i + 1], colorFill, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(vertices, count, color);
            }
        }

		public void DrawEllipse(Vector2 position, Vector2 radii, float red, float green, float blue)
		{
			DrawEllipse(new Ellipse(position, radii), new Color(red, green, blue));
		}

		public void DrawEllipse(Vector2 position, Vector2 radii, Color color)
		{
			DrawEllipse(new Ellipse(position, radii), color);
		}

		public void DrawEllipse(Ellipse ellipse, float red, float green, float blue)
		{
			DrawEllipse(ellipse, new Color(red, green, blue));
		}

		public void DrawEllipse(Ellipse ellipse, Color color)
		{
			if (!this.IsReady())
			{
				throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
			}
			const double increment = Math.PI * 2.0 / CircleSegments;
			double theta = 0.0;

			for (int i = 0; i < CircleSegments; i++)
			{
				Vector2 v1 = ellipse.Position + ellipse.Size * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Vector2 v2 = ellipse.Position + ellipse.Size * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

				this.AddVertex(v1, color, PrimitiveType.LineList);
				this.AddVertex(v2, color, PrimitiveType.LineList);

				theta += increment;
			}
		}

        public void DrawCircle(Vector2 center, float radius, float red, float green, float blue)
        {
            DrawCircle(center, radius, new Color(red, green, blue));
        }

        public void DrawCircle(Vector2 center, float radius, Color color)
        {
			DrawEllipse(new Ellipse(center, new Vector2(radius, radius)), color);
			/*
            if (!this.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            const double increment = Math.PI * 2.0 / CircleSegments;
            double theta = 0.0;

            for (int i = 0; i < CircleSegments; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center +
                             radius *
                             new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                this.AddVertex(v1, color, PrimitiveType.LineList);
                this.AddVertex(v2, color, PrimitiveType.LineList);

                theta += increment;
            }
			*/
        }

        public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float green, float blue, bool outline = false)
        {
			DrawSolidCircle(center, radius, axis, new Color(red, green, blue), outline);
        }

		public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color, bool outline = false)
        {
			DrawSolidEllipse(new Ellipse(center, new Vector2(radius, radius)), axis, color, outline);
			/*
            if (!this.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            const double increment = Math.PI * 2.0 / CircleSegments;
            double theta = 0.0;

            Color colorFill = color * 0.5f;

            Vector2 v0 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < CircleSegments - 1; i++)
            {
                Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = center +
                             radius *
                             new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                this.AddVertex(v0, colorFill, PrimitiveType.TriangleList);
                this.AddVertex(v1, colorFill, PrimitiveType.TriangleList);
                this.AddVertex(v2, colorFill, PrimitiveType.TriangleList);

                theta += increment;
            }
            DrawCircle(center, radius, color);

            DrawSegment(center, center + axis * radius, color);
			*/
        }

		public void DrawSolidEllipse(Vector2 position, Vector2 radii, Vector2 axis, float red, float green, float blue, bool outline = false)
		{
			DrawSolidEllipse(new Ellipse(position, radii), axis, new Color(red, green, blue), outline);
		}

		public void DrawSolidEllipse(Vector2 position, Vector2 radii, Vector2 axis, Color color, bool outline = false)
		{
			DrawSolidEllipse(new Ellipse(position, radii), axis, color, outline);
		}

		public void DrawSolidEllipse(Ellipse ellipse, Vector2 axis, float red, float green, float blue, bool outline = false)
		{
			DrawSolidEllipse(ellipse, axis, new Color(red, green, blue), outline);
		}

		public void DrawSolidEllipse(Ellipse ellipse, Vector2 axis, Color color, bool outline = false)
		{
			if (!this.IsReady())
			{
				throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
			}
			const double increment = Math.PI * 2.0 / CircleSegments;
			double theta = 0.0;

			Color colorFill = color * 0.5f;

			Vector2 v0 = ellipse.Position + ellipse.Size * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
			theta += increment;

			for (int i = 1; i < CircleSegments - 1; i++)
			{
				Vector2 v1 = ellipse.Position + ellipse.Size * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Vector2 v2 = ellipse.Position + ellipse.Size * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

				this.AddVertex(v0, colorFill, PrimitiveType.TriangleList);
				this.AddVertex(v1, colorFill, PrimitiveType.TriangleList);
				this.AddVertex(v2, colorFill, PrimitiveType.TriangleList);

				theta += increment;
			}

			if (outline)
			{
				DrawEllipse(ellipse, color);
			}

			axis.Normalize();
			DrawSegment(ellipse.Position, ellipse.Position + axis * ellipse.Size, color);
		}

        public void DrawSegment(Vector2 start, Vector2 end, float red, float green, float blue)
        {
            DrawSegment(start, end, new Color(red, green, blue));
        }

        public void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!this.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            this.AddVertex(start, color, PrimitiveType.LineList);
            this.AddVertex(end, color, PrimitiveType.LineList);
        }

        public void DrawPoint(Vector2 p, float size, Color color)
        {
            Vector2[] verts = new Vector2[4];
            float hs = size / 2.0f;
            verts[0] = p + new Vector2(-hs, -hs);
            verts[1] = p + new Vector2(hs, -hs);
            verts[2] = p + new Vector2(hs, hs);
            verts[3] = p + new Vector2(-hs, hs);

            DrawSolidPolygon(verts, 4, color, true);
        }

        public void DrawArrow(Vector2 start, Vector2 end, float length, float width, bool drawStartIndicator, Color color)
        {
            // Draw connection segment between start- and end-point
            DrawSegment(start, end, color);

            // Precalculate halfwidth
            float halfWidth = width / 2;

            // Create directional reference
            Vector2 rotation = (start - end);
            rotation.Normalize();

            // Calculate angle of directional vector
            float angle = (float)Math.Atan2(rotation.X, -rotation.Y);
            // Create matrix for rotation
            Matrix rotMatrix = Matrix.CreateRotationZ(angle);
            // Create translation matrix for end-point
            Matrix endMatrix = Matrix.CreateTranslation(end.X, end.Y, 0);

            // Setup arrow end shape
            Vector2[] verts = new Vector2[3];
            verts[0] = new Vector2(0, 0);
            verts[1] = new Vector2(-halfWidth, -length);
            verts[2] = new Vector2(halfWidth, -length);

            // Rotate end shape
            Vector2.Transform(verts, ref rotMatrix, verts);
            // Translate end shape
            Vector2.Transform(verts, ref endMatrix, verts);

            // Draw arrow end shape
            DrawSolidPolygon(verts, 3, color, false);

            if (drawStartIndicator)
            {
                // Create translation matrix for start
                Matrix startMatrix = Matrix.CreateTranslation(start.X, start.Y, 0);
                // Setup arrow start shape
                Vector2[] baseVerts = new Vector2[4];
                baseVerts[0] = new Vector2(-halfWidth, length / 4);
                baseVerts[1] = new Vector2(halfWidth, length / 4);
                baseVerts[2] = new Vector2(halfWidth, 0);
                baseVerts[3] = new Vector2(-halfWidth, 0);

                // Rotate start shape
                Vector2.Transform(baseVerts, ref rotMatrix, baseVerts);
                // Translate start shape
                Vector2.Transform(baseVerts, ref startMatrix, baseVerts);
                // Draw start shape
                DrawSolidPolygon(baseVerts, 4, color, false);
            }
        }

        private void AddVertex(Vector2 vertex, Color color, PrimitiveType primitiveType)
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
            }
            if (primitiveType == PrimitiveType.LineStrip || primitiveType == PrimitiveType.TriangleStrip)
            {
                throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");
            }

            if (primitiveType == PrimitiveType.TriangleList)
            {
                if (_triangleVertsCount >= _triangleVertices.Length)
                {
                    FlushTriangles();
                }
                _triangleVertices[_triangleVertsCount].Position = new Vector3(vertex, -0.1f);
                _triangleVertices[_triangleVertsCount].Color = color;
                _triangleVertsCount++;
            }
            if (primitiveType == PrimitiveType.LineList)
            {
                if (_lineVertsCount >= _lineVertices.Length)
                {
                    FlushLines();
                }
				_lineVertices[_lineVertsCount].Position = new Vector3(vertex, -0.2f);
                _lineVertices[_lineVertsCount].Color = color;
                _lineVertsCount++;
            }
        }


        /// <summary>
        /// End is called once all the primitives have been drawn using AddVertex.
        /// it will call Flush to actually submit the draw call to the graphics card, and
        /// then tell the basic effect to end.
        /// </summary>
        public void End()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before End can be called.");
            }

            // Draw whatever the user wanted us to draw
            FlushTriangles();
            FlushLines();

            _hasBegun = false;
        }

        private void FlushTriangles()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }
            if (_triangleVertsCount >= 3)
            {
                int primitiveCount = _triangleVertsCount / 3;
                // submit the draw call to the graphics card
                _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
				_device.RasterizerState = RasterizerState.CullNone;
                _device.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleVertices, 0, primitiveCount);
                _triangleVertsCount -= primitiveCount * 3;
            }
        }

        private void FlushLines()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            }
            if (_lineVertsCount >= 2)
            {
                int primitiveCount = _lineVertsCount / 2;
                // submit the draw call to the graphics card
                _device.SamplerStates[0] = SamplerState.AnisotropicClamp;
                _device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, primitiveCount);
                _lineVertsCount -= primitiveCount * 2;
            }
        }
    }
}