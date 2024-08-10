using System;
using NUnit.Framework;
using Plugins.SharpMath2Unity.Geometry2;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Tests
{
    [TestFixture]
    public class Polygon2Tests
    {
        [Test]
        public void ActualizePolygon_WithNoRotation_ReturnsOffsetVertices()
        {
            var polygon = new Polygon2(new float2[] {new(0, 0), new(1, 0), new(0.5f, 1)});
            var offset = new float2(2, 2);
            var rotation = new Rotation2(0);

            var actualizedPolygon = Polygon2.ActualizePolygon(polygon, offset, rotation);

            Assert.AreEqual(new float2[] {new(2, 2), new(3, 2), new(2.5f, 3) }, actualizedPolygon);
        }
        
        [Test]
        public void ActualizePolygon_WithRotation_ReturnsRotatedAndOffsetVertices()
        {
            var verts = new float2[] {new(0, 0), new(1,0), new(0.5f, 1)};
            var polygon = new Polygon2(verts);
            var offset = new float2(2, 2);
            var rotation = new Rotation2(math.PI / 2);

            var actualizedPolygon = Polygon2.ActualizePolygon(polygon, offset, rotation);

            Math2.AssertEqual(actualizedPolygon[0].Clip(), new(2.8333f, 1.8333f));
            Math2.AssertEqual(actualizedPolygon[1].Clip(),  new(2.8333f, 2.8333f));
            Math2.AssertEqual(actualizedPolygon[2].Clip(),  new(1.8333f, 2.3333f));
        }

        [Test]
        public void Contains_WithPointInside_ReturnsTrue()
        {
            var polygon = new Polygon2(new float2[] {new(0, 0), new(1, 0), new(0.5f, 1)});
            var pos = new float2(0, 0);
            var rot = new Rotation2(0);
            var point = new float2(0.5f, 0.5f);

            var result = Polygon2.Contains(polygon, pos, rot, point, false);

            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_WithPointOutside_ReturnsFalse()
        {
            var polygon = new Polygon2(new float2[] {new(0, 0), new(1, 0), new(0.5f, 1)});
            var pos = new float2(0, 0);
            var rot = new Rotation2(0);
            var point = new float2(1.5f, 1.5f);

            var result = Polygon2.Contains(polygon, pos, rot, point, false);

            Assert.IsFalse(result);
        }

        [Test]
        public void Intersects_WithIntersectingPolygons_ReturnsTrue()
        {
            var poly1 = new Polygon2(new float2[] {new(0, 0), new(2, 0), new(1, 2)});
            var poly2 = new Polygon2(new float2[] {new(1, 1), new(3, 1), new(2, 3)});
            var origin = new float2(0, 0);
            var rot1 = new Rotation2(0);
            var rot2 = new Rotation2(0);

            var result = Polygon2.Intersects(poly1, poly2, origin, origin, rot1, rot2, false);

            Assert.IsTrue(result);
        }

        [Test]
        public void Intersects_WithNonIntersectingPolygons_ReturnsFalse()
        {
            var poly1 = new Polygon2(new float2[] {new(0, 0), new(2, 0), new(1, 2)});
            var poly2 = new Polygon2(new float2[] {new(3, 3), new(5, 3), new(4, 5)});
            var origin = new float2(0, 0);
            var rot1 = new Rotation2(0);
            var rot2 = new Rotation2(0);

            var result = Polygon2.Intersects(poly1, poly2, origin, origin, rot1, rot2, false);

            Assert.IsFalse(result);
        }

        [Test]
        public void MinDistance_WithPolygonsNotIntersecting_ReturnsCorrectDistanceAndDirection()
        {
            var poly1 = new Polygon2(new float2[] {new(0, 0), new(1, 0), new(0.5f, 1)});
            var poly2 = new Polygon2(new float2[] {new(1, 1), new(2, 1), new(1.5f, 2)});
            var pos = new float2(0, 0);
            var rot1 = new Rotation2(0);
            var rot2 = new Rotation2(0);

            var result = Polygon2.MinDistance(poly1, poly2, pos, pos, rot1, rot2);

            Assert.IsNotNull(result);
            Math2.AssertEqual(result.Item1, new float2(0.8944272f, 0.4472136f));
            Math2.AssertEqual(result.Item2,  0.4472135955f);
        }

        [Test]
        public void MinDistance_WithIntersectingPolygons_ReturnsNull()
        {
            var poly1 = new Polygon2(new float2[] {new(0, 0), new(2, 0), new(1, 2)});
            var poly2 = new Polygon2(new float2[] {new(0, 1), new(2, 1), new(1, 3)});
            var pos1 = new float2(0, 0);
            var pos2 = new float2(1, 0);
            var rot1 = new Rotation2(0);
            var rot2 = new Rotation2(0);

            var result = Polygon2.MinDistance(poly1, poly2, pos1, pos2, rot1, rot2);
            Assert.IsNull(result);
        }
        
        
        [Test]
        public void MinDistance_WithPolygonsIntersectingAndRotated_ReturnsNull()
        {
            var poly1 = new Polygon2(new float2[] {new(1, 0), new(3, 0), new(2, 2)});
            var poly2 = new Polygon2(new float2[] {new(1, -1), new(3, -1), new(2, 1)});
            var pos1 = new float2(-1, 0);
            var pos2 = new float2(0, 2);
            var rot = new Rotation2(math.PI / 4);
            poly1 = Polygon2.GetRotated(poly1, rot);
            poly2 = Polygon2.GetRotated(poly2, rot);

            var result = Polygon2.MinDistance(poly1, poly2, pos1, pos2);
            Assert.IsNull(result);
        }
        
       
        
        [Test]
        public void MinDistance_WithPolygonsIntersectingAndRotatedAndOffset_ReturnsNull()
        {
            var poly1 = new Polygon2(new float2[] {new(0, 0), new(2, 0), new(1, 2)});
            var poly2 = new Polygon2(new float2[] {new(0, 1), new(2, 1), new(1, 3)});
            var pos1 = new float2(0, 0);
            var pos2 = new float2(1, 0);
            var rot = new Rotation2(math.PI / 4);
            poly1 = Polygon2.GetRotated(poly1, rot);
            poly2 = Polygon2.GetRotated(poly2, rot);

            var result = Polygon2.MinDistance(poly1, poly2, pos1, pos2);

            Assert.IsNull(result);
        }
        
        [Test]
        public void MinDistance_WithPolygonsNotIntersectingAndRotated_ReturnsCorrectDistanceAndDirection()
        {
            var poly = new Polygon2(new float2[] {new(0, 0), new(1, 0), new(0.5f, 1)});
            var pos1 = new float2(0, 0);
            var pos2 = new float2(2, 2);
            var rot = new Rotation2(math.PI / 4);
            poly = Polygon2.GetRotated(poly, rot);

            var result = Polygon2.MinDistance(poly, poly, pos1, pos2);

            Assert.IsNotNull(result);
            Math2.AssertEqual( result.Item1, math.normalize(new float2(1, 1)));
            Math2.AssertEqual( result.Item2, 1.82842712475f);
        }
    }
}