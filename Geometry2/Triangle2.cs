using System;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// Describes a triangle, which is a collection of three points. This is
    /// used for the implementation of the Polygon2.
    /// </summary>
    public class Triangle2
    {
        /// <summary>
        /// The 3 vertices of this triangle.
        /// </summary>
        public float2[] Vertices;

        /// <summary>
        /// This is used to determine if points are inside the triangle.
        /// This has 4 values where the first 2 correspond to row 1 and
        /// the second 2 to row 2 of a 2x2 matrix. When that matrix is
        /// matrix-multiplied by a point, if the result has a sum less
        /// than 1 and each component is positive, the point is in the
        /// triangle.
        /// </summary>
        private float[] InvContainsBasis;

        /// <summary>
        /// The centroid of the triangle
        /// </summary>
        public readonly float2 Center;

        /// <summary>
        /// The edges of the triangle, where the first edge is from 
        /// Vertices[0] to Vertices[1], etc.
        /// </summary>
        public readonly Line2[] Edges;

        /// <summary>
        /// The area of the triangle.
        /// </summary>
        public readonly float Area;

        /// <summary>
        /// Constructs a triangle with the given vertices, assuming that
        /// the vertices define a triangle (i.e., are not collinear)
        /// </summary>
        /// <param name="vertices">The vertices of the triangle</param>
        public Triangle2(float2[] vertices)
        {
            Vertices = vertices;

            float2 vertSum = float2.zero;
            for(int i = 0; i < 3; i++)
            {
                vertSum += vertices[i];
            }

            Center = vertSum / 3.0f;
            float a = vertices[1].x - vertices[0].x;
            float b = vertices[2].x - vertices[0].x;
            float c = vertices[1].y - vertices[0].y;
            float d = vertices[2].y - vertices[0].y;

            float det = a * d - b * c;
            Area = Math.Abs(0.5f * det);

            float invDet = 1 / det;
            InvContainsBasis = new float[4]
            {
                invDet * d, -invDet * b, 
                -invDet * c, invDet * a
            };

            Edges = new Line2[]
            {
                new Line2(Vertices[0], Vertices[1]),
                new Line2(Vertices[1], Vertices[2]),
                new Line2(Vertices[2], Vertices[0])
            };
        }

        /// <summary>
        /// Checks if this triangle contains the given point. This is
        /// never strict.
        /// </summary>
        /// <param name="tri">The triangle</param>
        /// <param name="pos">The position of the triangle</param>
        /// <param name="pt">The point to check</param>
        /// <returns>true if this triangle contains the point or the point
        /// is along an edge of this polygon</returns>
        public static bool Contains(Triangle2 tri, float2 pos, float2 pt)
        {
            float2 relPt = pt - pos - tri.Vertices[0];
            float r = tri.InvContainsBasis[0] * relPt.x + tri.InvContainsBasis[1] * relPt.y;
            if (r < -Math2.DEFAULT_EPSILON)
                return false;

            float t = tri.InvContainsBasis[2] * relPt.x + tri.InvContainsBasis[3] * relPt.y;
            if (t < -Math2.DEFAULT_EPSILON)
                return false;

            return (r + t) < 1 + Math2.DEFAULT_EPSILON;
        }

        /// <summary>
        /// An optimized check to determine if a triangle made up of the given
        /// points strictly contains the origin. This is generally slower than reusing
        /// a triangle, but much faster than creating a triangle and then doing
        /// a single contains check. There are aspects of the constructor which
        /// do not speed up the Contains check, which this skips.
        /// </summary>
        /// <param name="vertices">The 3 points making up the triangle</param>
        /// <returns>True if the given triangle contains the origin, false otherwise</returns>
        public static bool ContainsOrigin(float2[] vertices)
        {
            float a = vertices[1].x - vertices[0].x;
            float b = vertices[2].x - vertices[0].x;
            float c = vertices[1].y - vertices[0].y;
            float d = vertices[2].y - vertices[0].y;
            float det = a * d - b * c;
            float invDet = 1 / det;
            /*{
                invDet * d, -invDet * b,
                -invDet * c, invDet * a
            };*/

            // relPt = -vertices[0]
            float r = (invDet * d) * (-(vertices[0].x)) + (-invDet * b) * (-(vertices[0].y));
            if (r < -Math2.DEFAULT_EPSILON)
                return false;

            float t = (-invDet * c) * (-(vertices[0].x)) + (invDet * a) * (-(vertices[0].y));
            if (t < -Math2.DEFAULT_EPSILON)
                return false;

            return (r + t) < 1 + Math2.DEFAULT_EPSILON;
        }
    }
}
