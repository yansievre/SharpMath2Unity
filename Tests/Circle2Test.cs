using NUnit.Framework;
using Plugins.SharpMath2Unity.Geometry2;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Tests
{
    [TestFixture]
    [TestOf(typeof(Circle2))]
    public class Circle2Test
    {
        [Test]
        public void CircleEquality_WithEqualRadii_ReturnsTrue()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsTrue(circle1 == circle2);
        }

        [Test]
        public void CircleEquality_WithDifferentRadii_ReturnsFalse()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(10);
            Assert.IsFalse(circle1 == circle2);
        }

        [Test]
        public void CircleInequality_WithEqualRadii_ReturnsFalse()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsFalse(circle1 != circle2);
        }

        [Test]
        public void CircleInequality_WithDifferentRadii_ReturnsTrue()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(10);
            Assert.IsTrue(circle1 != circle2);
        }

        [Test]
        public void Equals_WithNullObject_ReturnsFalse()
        {
            var circle = new Circle2(5);
            Assert.IsFalse(circle.Equals(null));
        }

        [Test]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            var circle = new Circle2(5);
            var notACircle = new object();
            Assert.IsFalse(circle.Equals(notACircle));
        }

        [Test]
        public void Equals_WithSameRadius_ReturnsTrue()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsTrue(circle1.Equals(circle2));
        }

        [Test]
        public void GetHashCode_WithSameRadius_ReturnsEqualHashCodes()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.AreEqual(circle1.GetHashCode(), circle2.GetHashCode());
        }

        [Test]
        public void Contains_WithPointInside_ReturnsTrue()
        {
            var circle = new Circle2(5);
            var point = new float2(2, 2);
            Assert.IsTrue(Circle2.Contains(circle, new float2(0, 0), point, false));
        }

        [Test]
        public void Contains_WithPointOutside_ReturnsFalse()
        {
            var circle = new Circle2(5);
            var point = new float2(10, 10);
            Assert.IsFalse(Circle2.Contains(circle, new float2(0, 0), point, false));
        }

        [Test]
        public void Contains_WithPointOnEdgeAndStrict_ReturnsFalse()
        {
            var circle = new Circle2(5);
            var point = new float2(5, 0);
            Assert.IsFalse(Circle2.Contains(circle, new float2(0, 0), point, true));
        }

        [Test]
        public void Intersects_WithOverlappingCircles_ReturnsTrue()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsTrue(Circle2.Intersects(circle1, circle2, new float2(0, 0), new float2(3, 3)));
        }

        [Test]
        public void Intersects_WithNonOverlappingCircles_ReturnsFalse()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsFalse(Circle2.Intersects(circle1, circle2, new float2(0, 0), new float2(10, 10)));
        }

        [Test]
        public void IntersectMTV_WithOverlappingCircles_ReturnsNonNull()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsNotNull(Circle2.IntersectMTV(circle1, circle2, new float2(0, 0), new float2(3, 3)));
        }

        [Test]
        public void IntersectMTV_WithNonOverlappingCircles_ReturnsNull()
        {
            var circle1 = new Circle2(5);
            var circle2 = new Circle2(5);
            Assert.IsNull(Circle2.IntersectMTV(circle1, circle2, new float2(0, 0), new float2(10, 10)));
        }

        [Test]
        public void ProjectAlongAxis_WithHorizontalAxis_ReturnsCorrectProjection()
        {
            var circle = new Circle2(5);
            var axis = new float2(1, 0);
            var projection = Circle2.ProjectAlongAxis(circle, new float2(0, 0), axis);
            Math2.AssertEqual(projection.Min, 0f);
            Math2.AssertEqual(projection.Max, 10f);
        }
        
        [Test]
        public void ProjectAlongAxis_WithVerticalAxis_ReturnsCorrectProjection()
        {
            var circle = new Circle2(5);
            var axis = new float2(0, 1);
            var projection = Circle2.ProjectAlongAxis(circle, new float2(0, 0), axis);
            Math2.AssertEqual(projection.Min, 0f);
            Math2.AssertEqual(projection.Max, 10f);
        }
        
        [Test]
        public void ProjectAlongAxis_WithDiagonalAxis_ReturnsCorrectProjection()
        {
            var circle = new Circle2(5);
            var axis = math.normalize(new float2(1, 1));
            var projection = Circle2.ProjectAlongAxis(circle, new float2(0, 0), axis);
            Math2.AssertEqual(projection.Min, 2.07106781187f);
            Math2.AssertEqual(projection.Max, 12.0710678119f);
        }
    }
}