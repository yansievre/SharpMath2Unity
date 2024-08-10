using NUnit.Framework;
using Plugins.SharpMath2Unity.Geometry2;
using Unity.Mathematics;
using UnityEngine.Analytics;

namespace Plugins.SharpMath2Unity.Tests
{
    [TestFixture]
    public class AxisAlignedLine2Tests
    {
        [Test]
        public void Constructor_WithMinLessThanMax_SetsPropertiesCorrectly()
        {
            var axis = new float2(1, 0);
            var line = new AxisAlignedLine2(axis, -5, 5);

            Assert.AreEqual(axis, line.Axis);
            Assert.AreEqual(-5, line.Min);
            Assert.AreEqual(5, line.Max);
        }

        [Test]
        public void Constructor_WithMinGreaterThanMax_AutoCorrectsAndSetsProperties()
        {
            var axis = new float2(0, 1);
            var line = new AxisAlignedLine2(axis, 10, -10);

            Assert.AreEqual(axis, line.Axis);
            Assert.AreEqual(-10, line.Min);
            Assert.AreEqual(10, line.Max);
        }

        [Test]
        public void Intersects_WithOverlappingLines_ReturnsTrue()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 5);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 3, 8);

            Assert.IsTrue(AxisAlignedLine2.Intersects(line1, line2, false));
        }

        [Test]
        public void Intersects_WithNonOverlappingLines_ReturnsFalse()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 5);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 6, 10);

            Assert.IsFalse(AxisAlignedLine2.Intersects(line1, line2, false));
        }

        [Test]
        public void Intersects_WithAdjacentLinesStrict_ReturnsFalse()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 5);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 5, 10);

            Assert.IsFalse(AxisAlignedLine2.Intersects(line1, line2, true));
        }

        [Test]
        public void IntersectMTV_WithOverlappingLines_ReturnsCorrectDistance()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 5);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 3, 8);

            var mtv = AxisAlignedLine2.IntersectMTV(line1, line2);

            Assert.IsTrue(mtv.HasValue);
            Math2.AssertEqual(mtv.Value, -2);
        }
        
        [Test]
        public void IntersectMTV_WithOverlappingLines_ReturnsCorrectDistance2()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 7, 15);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 9, 11);

            var mtv = AxisAlignedLine2.IntersectMTV(line1, line2);

            Assert.IsTrue(mtv.HasValue);
            Math2.AssertEqual(mtv.Value, -6);
        }
        
        [Test]
        public void IntersectMTV_WithOverlappingLines_ReturnsCorrectDistance3()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 9, 11);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 7, 15);

            var mtv = AxisAlignedLine2.IntersectMTV(line1, line2);

            Assert.IsTrue(mtv.HasValue);
            Math2.AssertEqual(mtv.Value, 6);
        }
        
        [Test]
        public void IntersectMTV_WithOverlappingLines_ReturnsCorrectDistance4()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 9, 11);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 9, 11);

            var mtv = AxisAlignedLine2.IntersectMTV(line1, line2);

            Assert.IsTrue(mtv.HasValue);
            Math2.AssertEqual(mtv.Value, -2f);
        }
        

        [Test]
        public void IntersectMTV_WithNonOverlappingLines_ReturnsNull()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 2);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 3, 5);

            var mtv = AxisAlignedLine2.IntersectMTV(line1, line2);

            Assert.IsNull(mtv);
        }

        [Test]
        public void Contains_WithPointOnLine_ReturnsTrue()
        {
            var line = new AxisAlignedLine2(new float2(1, 0), 0, 10);

            Assert.IsTrue(AxisAlignedLine2.Contains(line, 5, false));
        }

        [Test]
        public void Contains_WithPointOutsideLine_ReturnsFalse()
        {
            var line = new AxisAlignedLine2(new float2(1, 0), 0, 10);

            Assert.IsFalse(AxisAlignedLine2.Contains(line, -1, false));
        }

        [Test]
        public void Contains_WithPointOnEdgeAndStrict_ReturnsFalse()
        {
            var line = new AxisAlignedLine2(new float2(1, 0), 0, 10);

            Assert.IsFalse(AxisAlignedLine2.Contains(line, 10, true));
        }

        [Test]
        public void MinDistance_WithPointOutsideLine_ReturnsCorrectDistance()
        {
            var line = new AxisAlignedLine2(new float2(1, 0), 0, 10);

            var distance = AxisAlignedLine2.MinDistance(line, 15);

            Assert.AreEqual(5, distance);
        }

        [Test]
        public void MinDistance_WithPointInsideLine_ReturnsNull()
        {
            var line = new AxisAlignedLine2(new float2(1, 0), 0, 10);

            var distance = AxisAlignedLine2.MinDistance(line, 5);

            Assert.IsNull(distance);
        }

        [Test]
        public void MinDistance_WithOverlappingLines_ReturnsNull()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 5);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 3, 8);

            var distance = AxisAlignedLine2.MinDistance(line1, line2);

            Assert.IsNull(distance);
        }

        [Test]
        public void MinDistance_WithNonOverlappingLines_ReturnsCorrectDistance()
        {
            var line1 = new AxisAlignedLine2(new float2(1, 0), 0, 2);
            var line2 = new AxisAlignedLine2(new float2(1, 0), 3, 5);

            var distance = AxisAlignedLine2.MinDistance(line1, line2);

            Assert.True(distance.HasValue);
            Math2.AssertEqual(distance.Value, 1);
        }
    }
}