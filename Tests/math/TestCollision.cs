using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using gravitymania.math;
using Microsoft.Xna.Framework;

namespace gravitymaniatest.math
{
    public class TestCollision
    {
        [Test]
        public void TestCollideEllipseWithPoint()
        {
            Ellipse ellipse = new Ellipse(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f));
            Vector2 velocity = new Vector2(1.0f, 0.0f);
            Vector2 point = new Vector2(1.5f, 0.0f);


            CollisionResult result;
            bool collided = Collide.CollideEllipseWithPoint(ellipse, velocity, point, out result);

            Assert.IsTrue(collided);
            Assert.That(result.Time, Is.EqualTo(0.5f).Within(0.00001f));
            Assert.That(result.Position.X, Is.EqualTo(point.X).Within(0.00001f));
            Assert.That(result.Position.Y, Is.EqualTo(point.Y).Within(0.00001f));
        }

        [Test]
        public void TestCollideEllipseWithLine()
        {
            Ellipse ellipse = new Ellipse(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f));
            Vector2 velocity = new Vector2(1.0f, 0.0f);
            LineSegment line = new LineSegment(new Vector2(1.5f, -2.0f), new Vector2(1.5f, 2.0f));


            CollisionResult result;
            bool collided = Collide.CollideEllipseWithLine(ellipse, velocity, line, out result);

            Assert.IsTrue(collided);
            Assert.That(result.Time, Is.EqualTo(0.5f).Within(0.00001f));
            Assert.That(result.Position.X, Is.EqualTo(line.Start.X).Within(0.00001f));
            Assert.That(result.Position.Y, Is.EqualTo(0.0).Within(0.00001f));
        }

        [Test]
        public void TestCollideEllipseWithLine2()
        {
            Ellipse ellipse = new Ellipse(new Vector2(0.0f, 0.0f), new Vector2(2.0f, 1.0f));
            Vector2 velocity = new Vector2(1.0f, 0.0f);
            LineSegment line = new LineSegment(new Vector2(2.5f, -2.0f), new Vector2(2.5f, 2.0f));


            CollisionResult result;
            bool collided = Collide.CollideEllipseWithLine(ellipse, velocity, line, out result);

            Assert.IsTrue(collided);
            Assert.That(result.Time, Is.EqualTo(0.5f).Within(0.00001f));
            Assert.That(result.Position.X, Is.EqualTo(line.Start.X).Within(0.00001f));
            Assert.That(result.Position.Y, Is.EqualTo(0.0).Within(0.00001f));
        }

        [Test]
        public void TestCollideEllipseWithLine3()
        {
            Ellipse ellipse = new Ellipse(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f));
            Vector2 velocity = new Vector2(1.0f, 1.0f);
            LineSegment line = new LineSegment(new Vector2(2.0f, 0.0f), new Vector2(0.0f, 2.0f));

            CollisionResult result;
            bool collided = Collide.CollideEllipseWithLine(ellipse, velocity, line, out result);

            Assert.IsTrue(collided);
            Assert.That(result.Time, Is.EqualTo(1.0f - 1.0f / Math.Sqrt(2.0)).Within(0.00001f));
            Assert.That(result.Position.X, Is.EqualTo(1.0f).Within(0.00001f));
            Assert.That(result.Position.Y, Is.EqualTo(1.0f).Within(0.00001f));
        }

        [Test]
        public void TestCollideEllipseWithLine4()
        {
            Ellipse ellipse = new Ellipse(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 5.0f));
            Vector2 velocity = new Vector2(1.0f, 0.0f);
            LineSegment line = new LineSegment(new Vector2(3.0f, 0.0f), new Vector2(1.0f, 2.0f));

            CollisionResult result;
            bool collided = Collide.CollideEllipseWithLine(ellipse, velocity, line, out result);

            Assert.IsTrue(collided);
            Assert.That(result.Time, Is.EqualTo(0.08348483).Within(0.00001f));
            Assert.That(result.Position.X, Is.EqualTo(line.End.X).Within(0.00001f));
            Assert.That(result.Position.Y, Is.EqualTo(line.End.Y).Within(0.00001f));
        }
    }
}
