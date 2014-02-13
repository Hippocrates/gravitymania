using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania;
using gravitymania.camera;

namespace gravitymaniaTest.camera
{
    [TestFixture]
    class TestCamera
    {
        [Test]
        public void TestCameraTransformZero()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(400, 300);

			Camera cam = new Camera(viewportSize, viewFieldSize, Vector2.Zero);

            Vector2 position = cam.TransformToView(Vector2.Zero);

            Assert.AreEqual(viewportSize.X / 2, position.X, 0.0000001);
            Assert.AreEqual(viewportSize.Y / 2, position.Y, 0.0000001);
        }

        [Test]
        public void TestCameraTransformLocation()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(400, 300);

            Vector2 cameraCenterLocation = new Vector2(33, 44);

			Camera cam = new Camera(viewportSize, viewFieldSize, cameraCenterLocation);

            Vector2 position = cam.TransformToView(cameraCenterLocation);

            Assert.AreEqual(viewportSize.X / 2, position.X, 0.0000001);
            Assert.AreEqual(viewportSize.Y / 2, position.Y, 0.0000001);
        }

        [Test]
        public void TestCameraTransformLimits()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(400, 300);

            Vector2 lowerLeft = (-viewFieldSize) / 2;
            Vector2 upperRight = viewFieldSize / 2;

			Camera cam = new Camera(viewportSize, viewFieldSize, Vector2.Zero);

            Vector2 llTransformed = cam.TransformToView(lowerLeft);

            Assert.AreEqual(0.0f, llTransformed.X, 0.00001);
            Assert.AreEqual(viewportSize.Y, llTransformed.Y, 0.00001);

            Vector2 urTransformed = cam.TransformToView(upperRight);

            Assert.AreEqual(viewportSize.X, urTransformed.X, 0.00001);
            Assert.AreEqual(0.0f, urTransformed.Y, 0.00001);
        }

        [Test]
        public void TestCameraTransformLimitsOffset()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(400, 300);

            Vector2 cameraPosition = new Vector2(33, 55);

            Vector2 lowerLeft = ((-viewFieldSize) / 2) + cameraPosition;
            Vector2 upperRight = (viewFieldSize / 2) + cameraPosition;

			Camera cam = new Camera(viewportSize, viewFieldSize, cameraPosition);

            Vector2 llTransformed = cam.TransformToView(lowerLeft);

            Assert.AreEqual(0.0f, llTransformed.X, 0.00001);
            Assert.AreEqual(viewportSize.Y, llTransformed.Y, 0.00001);

            Vector2 urTransformed = cam.TransformToView(upperRight);

            Assert.AreEqual(viewportSize.X, urTransformed.X, 0.00001);
            Assert.AreEqual(0.0f, urTransformed.Y, 0.00001);
        }

        [Test]
        public void TestViewportToWorld()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(400, 300);
            Vector2 cameraPosition = new Vector2(0.0f, 0.0f);

			Camera cam = new Camera(viewportSize, viewFieldSize, cameraPosition);

            Vector2 screenPosition = new Vector2(viewportSize.X / 2, viewportSize.Y / 2);
            
            Vector2 result = cam.TransformToWorld(screenPosition);

            Assert.AreEqual(0.0f, result.X);
            Assert.AreEqual(0.0f, result.Y);

            screenPosition = new Vector2(viewportSize.X / 4, viewportSize.Y / 4);

            result = cam.TransformToWorld(screenPosition);

            Assert.AreEqual(-viewFieldSize.X / 4, result.X);
            Assert.AreEqual(viewFieldSize.Y / 4, result.Y);

            screenPosition = new Vector2(3*viewportSize.X / 4, viewportSize.Y / 4);

            result = cam.TransformToWorld(screenPosition);

            Assert.AreEqual(viewFieldSize.X / 4, result.X);
            Assert.AreEqual(viewFieldSize.Y / 4, result.Y);
        }

        [Test]
        public void TestCameraGetViewLockPosition()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(400, 300);
            Vector2 cameraPosition = new Vector2(0.0f, 0.0f);

			Camera cam = new Camera(viewportSize, viewFieldSize, cameraPosition);

            Vector2 targetPosition = new Vector2(5.0f, 5.0f);

            Vector2 screenLock = new Vector2(-15.0f, 5.0f);

            Vector2 cameraResult = cam.GetViewLockPosition(targetPosition, screenLock);

            Assert.AreEqual(214.375f, cameraResult.X);
            Assert.AreEqual(-141.875f, cameraResult.Y);
        }

        [Test]
        public void TestCameraGetViewLockPositionRegression()
        {
            Vector2 viewportSize = new Vector2(640, 480);
            Vector2 viewFieldSize = new Vector2(650, 490);
            // This came up because I wasn't correctly subtracting the current camera position from the equation.
            // It didn't show up because all the other tests had the camera at 0,0
            Vector2 cameraPosition = new Vector2(50.0f, 50.0f);

			Camera cam = new Camera(viewportSize, viewFieldSize, cameraPosition);

            // first, clamp the camera position to the maximum allowed off-screen positions
            float emergencyScreenMaxX = 512.0f;

            Vector2 playerPosition = new Vector2(400.4609f, 0.0f);

            // no, the Y parameters are not reversed, remember that the screen is 'upside-down' according to the world space
            cam.Position = cam.GetViewLockPosition(playerPosition, new Vector2(emergencyScreenMaxX, 0.0f));

            Vector2 adjustedPlayerScreenPos = cam.TransformToView(playerPosition);

            Assert.AreEqual(emergencyScreenMaxX, adjustedPlayerScreenPos.X);
        }
    }
}
