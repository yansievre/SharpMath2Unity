using System;
using NUnit.Framework;
using Plugins.SharpMath2Unity.Geometry2;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Tests
{
    [TestFixture]
    public class Line2Tests
    {
        [Test]
        public void Constructor_WithDifferentStartAndEnd_InitializesPropertiesCorrectly()
        {
            var start = new float2(0, 0);
            var end = new float2(5, 5);
            var line = new Line2(start, end);

            Assert.AreEqual(start, line.start);
            Assert.AreEqual(end, line.end);
            Assert.IsFalse(line.horizontal);
            Assert.IsFalse(line.vertical);
        }
        
        [Test]
        public void Constructor_WithHorizontalLine_InitializesPropertiesCorrectly()
        {
            var start = new float2(0, 0);
            var end = new float2(5, 0);
            var line = new Line2(start, end);

            Assert.AreEqual(start, line.start);
            Assert.AreEqual(end, line.end);
            Assert.IsTrue(line.horizontal);
            Assert.IsFalse(line.vertical);
        }
        
        [Test]
        public void Constructor_WithVerticalLine_InitializesPropertiesCorrectly()
        {
            var start = new float2(0, 0);
            var end = new float2(0, 5);
            var line = new Line2(start, end);

            Assert.AreEqual(start, line.start);
            Assert.AreEqual(end, line.end);
            Assert.IsFalse(line.horizontal);
            Assert.IsTrue(line.vertical);
        }

        [Test]
        public void Constructor_WithSameStartAndEnd_ThrowsArgumentException()
        {
            var start = new float2(0, 0);
            var end = new float2(0, 0);

            Assert.Throws<ArgumentException>(() => new Line2(start, end));
        }

        [Test]
        public void Parallel_WithParallelLines_ReturnsTrue()
        {
            var line1 = new Line2(new float2(0, 0), new float2(5, 5));
            var line2 = new Line2(new float2(1, 1), new float2(6, 6));

            Assert.IsTrue(Line2.Parallel(line1, line2));
        }

        [Test]
        public void Parallel_WithNonParallelLines_ReturnsFalse()
        {
            var line1 = new Line2(new float2(0, 0), new float2(5, 5));
            var line2 = new Line2(new float2(0, 0), new float2(5, 0));

            Assert.IsFalse(Line2.Parallel(line1, line2));
        }

        [Test]
        public void Contains_WithPointOnLine_ReturnsTrue()
        {
            var line = new Line2(new float2(0, 0), new float2(5, 5));
            var point = new float2(2.5f, 2.5f);

            Assert.IsTrue(Line2.Contains(line, new float2(0, 0), point));
        }

        [Test]
        public void Contains_WithPointNotOnLine_ReturnsFalse()
        {
            var line = new Line2(new float2(0, 0), new float2(5, 5));
            var point = new float2(3, 2);

            Assert.IsFalse(Line2.Contains(line, new float2(0, 0), point));
        }

        [Test]
        public void Intersects_WithIntersectingLines_ReturnsTrue()
        {
            var line1 = new Line2(new float2(0, 0), new float2(5, 5));
            var line2 = new Line2(new float2(0, 5), new float2(5, 0));

            Assert.IsTrue(Line2.Intersects(line1, line2, new float2(0, 0), new float2(0, 0), false));
        }

        [Test]
        public void Intersects_WithNonIntersectingLines_ReturnsFalse()
        {
            var line1 = new Line2(new float2(0, 0), new float2(5, 5));
            var line2 = new Line2(new float2(6, 6), new float2(10, 10));

            Assert.IsFalse(Line2.Intersects(line1, line2, new float2(0, 0), new float2(0, 0), false));
        }

        [Test]
        public void Distance_WithPointOnLine_ReturnsZero()
        {
            var line = new Line2(new float2(0, 0), new float2(10, 0));
            var point = new float2(5, 0);

            Assert.AreEqual(0, Line2.Distance(line, new float2(0, 0), point));
        }

        [Test]
        public void Distance_WithPointOffLine_ReturnsCorrectDistance()
        {
            var line = new Line2(new float2(0, 0), new float2(10, 0));
            var point = new float2(5, 5);

            Assert.AreEqual(5, Line2.Distance(line, new float2(0, 0), point));
        }
    }
}