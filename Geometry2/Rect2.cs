using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// Describes a rectangle. Meant to be reused.
    /// </summary>
    public class Rect2 : Shape2
    {
        /// <summary>
        /// The vertices of this rectangle as a clockwise array.
        /// </summary>
        public readonly float2[] Vertices;

        /// <summary>
        /// The corner with the smallest x and y coordinates on this
        /// rectangle.
        /// </summary>
        public float2 Min => Vertices[0];

        /// <summary>
        /// The corner with the largest x and y coordinates on this
        /// rectangle
        /// </summary>
        public float2 Max => Vertices[2];

        /// <summary>
        /// The corner with the largest x and smallest y coordinates on
        /// this rectangle
        /// </summary>
        public float2 UpperRight => Vertices[1];

        /// <summary>
        /// The corner with the smallest x and largest y coordinates on this
        /// rectangle
        /// </summary>
        public float2 LowerLeft => Vertices[3];

        /// <summary>
        /// The center of this rectangle
        /// </summary>
        public readonly float2 Center;

        /// <summary>
        /// The width of this rectangle
        /// </summary>
        public readonly float Width;

        /// <summary>
        /// The height of this rectangle
        /// </summary>
        public readonly float Height;

        /// <summary>
        /// Creates a bounding box with the specified upper-left and bottom-right.
        /// Will autocorrect if min.x > max.x or min.y > max.y
        /// </summary>
        /// <param name="min">Min x, min y</param>
        /// <param name="max">Max x, max y</param>
        /// <exception cref="ArgumentException">If min and max do not make a box</exception>
        public Rect2(float2 min, float2 max)
        {
            float area = (max.x - min.x) * (max.y - min.y);
            if(area > -Math2.DEFAULT_EPSILON && area < Math2.DEFAULT_EPSILON)
                throw new ArgumentException($"min={min}, max={max} - that's a line or a point, not a box (area below epsilon {Math2.DEFAULT_EPSILON} (got {area}))");

            float tmpX1 = min.x, tmpX2 = max.x;
            float tmpY1 = min.y, tmpY2 = max.y;

            min.x = Math.Min(tmpX1, tmpX2);
            min.y = Math.Min(tmpY1, tmpY2);
            max.x = Math.Max(tmpX1, tmpX2);
            max.y = Math.Max(tmpY1, tmpY2);

            Vertices = new float2[]
            {
                min, new float2(max.x, min.y), max, new float2(min.x, max.y)
            };

            Center = new float2((Min.x + Max.x) / 2, (Min.y + Max.y) / 2);

            Width = Max.x - Min.x;
            Height = Max.y - Min.y;
        }

        /// <summary>
        /// Creates a bounding box from the specified points. Will correct if minX > maxX or minY > maxY.
        /// </summary>
        /// <param name="minX">Min or max x (different from maxX)</param>
        /// <param name="minY">Min or max y (different from maxY)</param>
        /// <param name="maxX">Min or max x (different from minX)</param>
        /// <param name="maxY">Min or max y (different from minY)</param>
        public Rect2(float minX, float minY, float maxX, float maxY) : this(new float2(minX, minY), new float2(maxX, maxY))
        {
        }

        /// <summary>
        /// Determines if box1 with origin pos1 intersects box2 with origin pos2.
        /// </summary>
        /// <param name="box1">Box 1</param>
        /// <param name="box2">Box 2</param>
        /// <param name="pos1">Origin of box 1</param>
        /// <param name="pos2">Origin of box 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If box1 intersects box2 when box1 is at pos1 and box2 is at pos2</returns>
        public static bool Intersects(Rect2 box1, Rect2 box2, float2 pos1, float2 pos2, bool strict)
        {
            return AxisAlignedLine2.Intersects(box1.Min.x + pos1.x, box1.Max.x + pos1.x, box2.Min.x + pos2.x, box2.Max.x + pos2.x, strict, false)
                && AxisAlignedLine2.Intersects(box1.Min.y + pos1.y, box1.Max.y + pos1.y, box2.Min.y + pos2.y, box2.Max.y + pos2.y, strict, false);
        }

        /// <summary>
        /// Determines if the box when at pos contains point.
        /// </summary>
        /// <param name="box">The box</param>
        /// <param name="pos">Origin of box</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">true if the edges do not count</param>
        /// <returns>If the box at pos contains point</returns>
        public static bool Contains(Rect2 box, float2 pos, float2 point, bool strict)
        {
            return AxisAlignedLine2.Contains(box.Min.x + pos.x, box.Max.x + pos.x, point.x, strict, false)
                && AxisAlignedLine2.Contains(box.Min.y + pos.y, box.Max.y + pos.y, point.y, strict, false);
        }

        /// <summary>
        /// Determines if innerBox is contained entirely in outerBox
        /// </summary>
        /// <param name="outerBox">the (bigger) box that you want to check contains the inner box</param>
        /// <param name="innerBox">the (smaller) box that you want to check is contained in the outer box</param>
        /// <param name="posOuter">where the outer box is located</param>
        /// <param name="posInner">where the inner box is located</param>
        /// <param name="strict">true to return false if innerBox touches an edge of outerBox, false otherwise</param>
        /// <returns>true if innerBox is contained in outerBox, false otherwise</returns>
        public static bool Contains(Rect2 outerBox, Rect2 innerBox, float2 posOuter, float2 posInner, bool strict)
        {
            return Contains(outerBox, posOuter, innerBox.Min + posInner, strict) && Contains(outerBox, posOuter, innerBox.Max + posInner, strict);
        }

        /// <summary>
        /// Deterimines in the box contains the specified polygon
        /// </summary>
        /// <param name="box">The box</param>
        /// <param name="poly">The polygon</param>
        /// <param name="boxPos">Where the box is located</param>
        /// <param name="polyPos">Where the polygon is located</param>
        /// <param name="strict">true if we return false if the any part of the polygon is on the edge, false otherwise</param>
        /// <returns>true if the poly is contained in box, false otherwise</returns>
        public static bool Contains(Rect2 box, Polygon2 poly, float2 boxPos, float2 polyPos, bool strict)
        {
            return Contains(box, poly.AABB, boxPos, polyPos, strict);
        }

        /// <summary>
        /// Projects the rectangle at pos along axis.
        /// </summary>
        /// <param name="rect">The rectangle to project</param>
        /// <param name="pos">The origin of the rectangle</param>
        /// <param name="axis">The axis to project on</param>
        /// <returns>The projection of rect at pos along axis</returns>
        public static unsafe AxisAlignedLine2 ProjectAlongAxis(Rect2 rect, float2 pos, float2 axis)
        {
            //return ProjectAlongAxis(axis, pos, Rotation2.Zero, rect.Center, rect.Min, rect.UpperRight, rect.LowerLeft, rect.Max);
            fixed (float2* p = rect.Vertices)
            {
                ProjectAlongAxis_Internal(pos, axis, p, rect.Vertices.Length, out var minMax);

                return new AxisAlignedLine2(axis, minMax.x, minMax.y);
            }
        }
    }

    [BurstCompile]
    public static class RectMethods
    {
        
        [BurstCompile]
        public static bool Contains(float radiusSquared, in float2 pos, in float2 point)
        {
            return math.lengthsq(point - pos) <= radiusSquared;
        }
    }
}
