using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// A class containing utilities that help creating shapes.
    /// </summary>
    public class ShapeUtils
    {
        /// <summary>
        /// A dictionary containing the circle shapes.
        /// </summary>
        private static Dictionary<Tuple<float, float, float, float>, Polygon2> CircleCache = new Dictionary<Tuple<float, float, float, float>, Polygon2>();

        /// <summary>
        /// A dictionary containing the rectangle shapes.
        /// </summary>
        private static Dictionary<Tuple<float, float, float, float>, Polygon2> RectangleCache = new Dictionary<Tuple<float, float, float, float>, Polygon2>();

        /// <summary>
        /// A dictionary containing the convex polygon shapes.
        /// </summary>
        private static Dictionary<int, Polygon2> ConvexPolygonCache = new Dictionary<int, Polygon2>();

        /// <summary>
        /// Returns the cross product of the given three vectors.
        /// </summary>
        /// <param name="v1">Vector 1.</param>
        /// <param name="v2">Vector 2.</param>
        /// <param name="v3">Vector 3.</param>
        /// <returns></returns>
        private static double Cross(float2 v1, float2 v2, float2 v3)
        {
            return (v2.x - v1.x) * (v3.y - v1.y) - (v2.y - v1.y) * (v3.x - v1.x);
        }

        /// <summary>
        /// Fetches a rectangle shape with the given width, height, x and y center.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="x">The X center of the rectangle.</param>
        /// <param name="y">The Y center of the rectangle.</param>
        /// <returns>A rectangle shape with the given width, height, x and y center.</returns>
        public static Polygon2 CreateRectangle(float width, float height, float x = 0, float y = 0)
        {
            width = width.Clip();
            height = height.Clip();
            x = x.Clip();
            y = y.Clip();
            var Key = new Tuple<float, float, float, float>(width, height, x, y);

            if (RectangleCache.ContainsKey(Key))
                return RectangleCache[Key];

            return RectangleCache[Key] = new Polygon2(new[] {
                 new float2(x, y),
                 new float2(x + width, y),
                 new float2(x + width, y + height),
                 new float2(x, y + height)
            });
        }

        /// <summary>
        /// Fetches a circle shape with the given radius, center, and segments. Because of the discretization
        /// of the circle, it is not possible to perfectly get the AABB to match both the radius and the position.
        /// This will match the position.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="x">The X center of the circle.</param>
        /// <param name="y">The Y center of the circle.</param>
        /// <param name="segments">The amount of segments (more segments equals higher detailed circle)</param>
        /// <returns>A circle with the given radius, center, and segments, as a polygon2 shape.</returns>
        public static Polygon2 CreateCircle(float radius, float x = 0, float y = 0, int segments = 32)
        {
            radius = radius.Clip();
            x = x.Clip();
            y = y.Clip();
            var Key = new Tuple<float, float, float, float>(radius, x, y, segments);

            if (CircleCache.ContainsKey(Key))
                return CircleCache[Key];

            var Center = new float2(radius + x, radius + y);
            var increment = (Math.PI * 2.0) / segments;
            var theta = 0.0;
            var verts = new List<float2>(segments);

            float2 correction = new float2(radius, radius);
            for (var i = 0; i < segments; i++)
            {
                float2 vert = radius * new float2(
                        (float)Math.Cos(theta),
                        (float)Math.Sin(theta)
                    );

                if (vert.x < correction.x)
                    correction.x = vert.x;
                if (vert.y < correction.y)
                    correction.y = vert.y;

                verts.Add(
                    Center + vert
                );
                theta += increment;
            }

            correction.x += radius;
            correction.y += radius;

            for(var i = 0; i < segments; i++)
            {
                verts[i] -= correction;
            }

            return CircleCache[Key] = new Polygon2(verts.ToArray());
        }
    }
}