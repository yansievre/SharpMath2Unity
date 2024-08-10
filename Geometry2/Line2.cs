using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    public enum LineInterType
    {
        /// <summary>
        /// Two segments with different slopes which do not intersect
        /// </summary>
        NonParallelNone,
        /// <summary>
        /// Two segments with different slopes which intersect at a 
        /// single point.
        /// </summary>
        NonParallelPoint,
        /// <summary>
        /// Two parallel but not coincident segments. These never intersect
        /// </summary>
        ParallelNone,
        /// <summary>
        /// Two coincident segments which do not intersect
        /// </summary>
        CoincidentNone,
        /// <summary>
        /// Two coincident segments which intersect at a point
        /// </summary>
        CoincidentPoint,
        /// <summary>
        /// Two coincident segments which intersect on infinitely many points
        /// </summary>
        CoincidentLine
    }

    /// <summary>
    /// Describes a line. Does not have position and is meant to be reused.
    /// </summary>
    [BurstCompile]
    public class Line2
    {
        /// <summary>
        /// Where the line begins
        /// </summary>
        public readonly float4 startEnd;

        /// <summary>
        /// Where the line ends
        /// </summary>
        public readonly float2 start;
        /// <summary>
        /// Where the line ends
        /// </summary>
        public readonly float2 end;

        /// <summary>
        /// End - Start
        /// </summary>
        public readonly float2 delta;

        /// <summary>
        /// Normalized Delta
        /// </summary>
        public readonly float2 axis;

        /// <summary>
        /// The normalized normal of axis.
        /// </summary>
        public readonly float2 normal;

        /// <summary>
        /// Square of the magnitude of this line
        /// </summary>
        public readonly float magnitudeSquared;

        /// <summary>
        /// Magnitude of this line
        /// </summary>
        public readonly float magnitude;

        /// <summary>
        /// Min x
        /// </summary>
        public readonly float minX;
        /// <summary>
        /// Min y
        /// </summary>
        public readonly float minY;

        /// <summary>
        /// Max x
        /// </summary>
        public readonly float maxX;

        /// <summary>
        /// Max y
        /// </summary>
        public readonly float maxY;

        /// <summary>
        /// Slope of this line
        /// </summary>
        public readonly float slope;

        /// <summary>
        /// Where this line would hit the y intercept. NaN if vertical line.
        /// </summary>
        public readonly float yIntercept;

        /// <summary>
        /// If this line is horizontal
        /// </summary>
        public readonly bool horizontal;

        /// <summary>
        /// If this line is vertical
        /// </summary>
        public readonly bool vertical;

        /// <summary>
        /// Creates a line from start to end
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        public Line2(float2 start, float2 end)
        {
            if (Math2.Approximately(start, end))
                throw new ArgumentException($"start is approximately end - that's a point, not a line. start={start}, end={end}");
            this.start = start;
            this.end = end;
            InitLine(start, end, 
                out startEnd, out delta, out axis, out normal, 
                out magnitudeSquared, out magnitude,
                out minX, out minY, out maxX, out maxY, 
                out horizontal, out vertical, 
                out slope, out yIntercept);
        }

        [BurstCompile]
        internal static void InitLine(
            in float2 start,
            in float2 end,
            out float4 startEnd,
            out float2 delta,
            out float2 axis,
            out float2 normal,
            out float magnitudeSquared,
            out float mag,
            out float minX,
            out float minY,
            out float maxX,
            out float maxY,
            out bool horizontal,
            out bool vertical,
            out float slope,
            out float yIntercept
            )
        {
            
            startEnd = new float4(start.xy, end.xy);
            delta = end - start;
            GetAxisAndNormal_Internal(delta, out axis, out normal);
            magnitudeSquared = math.lengthsq(delta);
            mag = math.sqrt(magnitudeSquared);

            minX = math.min(start.x, end.x);
            minY = math.min(start.y, end.y);
            maxX = math.max(start.x, end.x);
            maxY = math.max(start.y, end.y);
            horizontal = GetIsHorizontal_Internal(startEnd);
            vertical = GetIsVertical_Internal(startEnd);

            if (vertical)
                slope = float.PositiveInfinity;
            else
                slope = (end.y - start.y) / (end.x - start.x);

            if (vertical)
                yIntercept = float.NaN;
            else
            {
                // y = mx + b
                // Start.y = Slope * Start.x + b
                // b = Start.y - Slope * Start.x
                yIntercept = start.y - slope * start.x;
            }
        }

        [BurstCompile]
        internal static void GetAxisAndNormal_Internal(in float4 line, out float2 axis, out float2 normal)
        {
            GetAxisAndNormal_Internal(line.zw - line.xy, out axis, out normal);
        }
        
        [BurstCompile]
        internal static void GetAxisAndNormal_Internal(in float2 delta, out float2 axis, out float2 normal)
        {
           GetAxis_Internal(delta, out axis);
           Normalize_Internal(delta, out normal);
        }
        
        [BurstCompile]
        internal static void GetAxis_Internal(in float4 line, out float2 result)
        {
            result = math.normalize(line.zw-line.xy);
        }

        [BurstCompile]
        internal static void Normalize_Internal(in float4 line, out float2 result)
        {
            result = math.normalize(Math2.Perpendicular(line.zw-line.xy));
        }

        
        [BurstCompile]
        private static void GetAxis_Internal(in float2 delta, out float2 result)
        {
            result = math.normalize(delta);
        }

        [BurstCompile]
        private static void Normalize_Internal(in float2 delta, out float2 result)
        {
            result = math.normalize(Math2.Perpendicular(delta));
        }
        [BurstCompile]
        internal static bool GetIsVertical_Internal(in float4 startEnd)
        {
            return math.abs(startEnd.z - startEnd.x) <= Math2.DEFAULT_EPSILON;
        }

        [BurstCompile]
        internal static bool GetIsHorizontal_Internal(in float4 startEnd)
        {
            return math.abs(startEnd.w - startEnd.y) <= Math2.DEFAULT_EPSILON;
        }

        /// <summary>
        /// Determines if the two lines are parallel. Shifting lines will not
        /// effect the result.
        /// </summary>
        /// <param name="line1">The first line</param>
        /// <param name="line2">The second line</param>
        /// <returns>True if the lines are parallel, false otherwise</returns>
        public static bool Parallel(Line2 line1, Line2 line2)
        {
            return Parallel_Internal(line1.axis, line2.axis);
        }

        [BurstCompile]
        private static bool Parallel_Internal(in float2 axis1, in float2 axis2)
        {
            return (
                Math2.Approximately(axis1, axis2)
                || Math2.Approximately(axis1, -axis2)
            );
        }
        /// <summary>
        /// Determines if the given point is along the infinite line described
        /// by the given line shifted the given amount.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="pos">The shift for the line</param>
        /// <param name="pt">The point</param>
        /// <returns>True if pt is on the infinite line extension of the segment</returns>
        public static bool AlongInfiniteLine(Line2 line, float2 pos, float2 pt)
        {
            float normalPart = math.dot(pt - pos - line.start, line.normal);
            return Math2.Approximately(normalPart, 0);
        }

        /// <summary>
        /// Determines if the given line contains the given point.
        /// </summary>
        /// <param name="line">The line to check</param>
        /// <param name="pos">The offset for the line</param>
        /// <param name="pt">The point to check</param>
        /// <returns>True if pt is on the line, false otherwise</returns>
        public static bool Contains(Line2 line, float2 pos, float2 pt)
        {
            // The horizontal/vertical checks are not required but are
            // very fast to calculate and short-circuit the common case
            // (false) very quickly
            if(line.horizontal)
            {
                return Math2.Approximately(line.start.y + pos.y, pt.y)
                    && AxisAlignedLine2.Contains(line.minX, line.maxX, pt.x - pos.x, false);
            }
            if(line.vertical)
            {
                return Math2.Approximately(line.start.x + pos.x, pt.x)
                    && AxisAlignedLine2.Contains(line.minY, line.maxY, pt.y - pos.y, false);
            }

            // Our line is not necessarily a linear space, but if we shift
            // our line to the origin and adjust the point correspondingly
            // then we have a linear space and the problem remains the same.

            // Our line at the origin is just the infinite line with slope
            // Axis. We can form an orthonormal basis of R2 as (Axis, Normal).
            // Hence we can write pt = line_part * Axis + normal_part * Normal. 
            // where line_part and normal_part are floats. If the normal_part
            // is 0, then pt = line_part * Axis, hence the point is on the
            // infinite line.

            // Since we are working with an orthonormal basis, we can find
            // components with dot products.

            // To check the finite line, we consider the start of the line
            // the origin. Then the end of the line is line.Magnitude * line.Axis.

            float2 lineStart = pos + line.start;

            float normalPart = Math2.Dot(pt - lineStart, line.normal);
            if (!Math2.Approximately(normalPart, 0))
                return false;

            float axisPart = Math2.Dot(pt - lineStart, line.axis);
            return axisPart > -Math2.DEFAULT_EPSILON 
                && axisPart < line.magnitude + Math2.DEFAULT_EPSILON;
        }

        private static unsafe void FindSortedOverlap(float* projs, bool* isFromLine1)
        {
            // ascending insertion sort while simultaneously updating 
            // isFromLine1
            for (int i = 0; i < 3; i++)
            {
                int best = i;
                for (int j = i + 1; j < 4; j++)
                {
                    if (projs[j] < projs[best])
                    {
                        best = j;
                    }
                }
                if (best != i)
                {
                    (projs[i], projs[best]) = (projs[best], projs[i]);
                    (isFromLine1[i], isFromLine1[best]) = (isFromLine1[best], isFromLine1[i]);
                }
            }
        }

        /// <summary>
        /// Checks the type of intersection between the two coincident lines.
        /// </summary>
        /// <param name="a">The first line</param>
        /// <param name="b">The second line</param>
        /// <param name="pos1">The offset for the first line</param>
        /// <param name="pos2">The offset for the second line</param>
        /// <returns>The type of intersection</returns>
        public static unsafe LineInterType CheckCoincidentIntersectionType(Line2 a, Line2 b, float2 pos1, float2 pos2)
        {
            float2 relOrigin = a.start + pos1;

            float* projs = stackalloc float[4] {
                0,
                a.magnitude,
                Math2.Dot((b.start + pos2) - relOrigin, a.axis),
                Math2.Dot((b.end + pos2) - relOrigin, a.axis)
            };

            bool* isFromLine1 = stackalloc bool[4] {
                true,
                true,
                false,
                false
            };

            FindSortedOverlap(projs, isFromLine1);

            if (Math2.Approximately(projs[1], projs[2]))
                return LineInterType.CoincidentPoint;
            if (isFromLine1[0] == isFromLine1[1])
                return LineInterType.CoincidentNone;
            return LineInterType.CoincidentLine;
        }

        /// <summary>
        /// Determines if line1 intersects line2, when line1 is offset by pos1 and line2 
        /// is offset by pos2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line2">Line 2</param>
        /// <param name="pos1">Origin of line 1</param>
        /// <param name="pos2">Origin of line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 intersects line2</returns>
        public static bool Intersects(Line2 line1, Line2 line2, float2 pos1, float2 pos2, bool strict)
        {
            if (!Parallel(line1, line2)) 
                return HasIntersection(line1, line2, pos1, pos2, strict);
            if (!AlongInfiniteLine(line1, pos1, line2.start + pos2))
                return false;
            LineInterType iType = CheckCoincidentIntersectionType(line1, line2, pos1, pos2);
            if (iType == LineInterType.CoincidentNone)
                return false;
            if (iType == LineInterType.CoincidentPoint)
                return !strict;
            return true;

        }

        public static bool HasIntersection(Line2 line1,
            Line2 line2,
            float2 pos1,
            float2 pos2,
            bool strict)
        {
            return HasIntersection_Internal(line1.startEnd, line2.startEnd, pos1, pos2, strict);
        }

        [BurstCompile]
        internal static bool HasIntersection_Internal(in float4 line1,
            in float4 line2,
            in float2 pos1,
            in float2 pos2,
            bool strict)
        {
            // The infinite lines intersect at exactly one point. The segments intersect
            // if they both contain that point. We will treat the lines as first-degree
            // Bezier lines to skip the vertical case
            // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
            
            var d1 = line2.x + pos2.x - line2.z - pos2.x;
            var d2 = line2.y + pos2.y - line2.w - pos2.y;

            var det = ( line1.x + pos1.x - line1.z - pos1.x) * d2 - (line1.y + pos1.y - line1.w - pos1.y) * d1;
            // we assume det != 0 (lines not parallel)
            if(Math2.Approximately(det, 0))
                return false;

            var t = ((line1.x + pos1.x - line2.x - pos2.x) * d2 - (line1.y + pos1.y - line2.y - pos2.y) * d1) / det;


            var min = strict ? Math2.DEFAULT_EPSILON : -Math2.DEFAULT_EPSILON;
            var max = 1 - min;
            return !(t < min) && !(t > max);
        }

        /// <summary>
        /// Finds the intersection of two non-parallel lines a and b. Returns
        /// true if the point of intersection is on both line segments, returns
        /// false if the point of intersection is not on at least one line
        /// segment. In either case, pt is set to where the intersection is
        /// on the infinite lines.
        /// </summary>
        /// <param name="line1">First line</param>
        /// <param name="line2">Second line</param>
        /// <param name="pos1">The shift of the first line</param>
        /// <param name="pos2">The shift of the second line</param>
        /// <param name="strict">True if we should return true if pt is on an edge of a line as well
        /// as in the middle of the line. False to return true only if pt is really within the lines</param>
        /// <param name="pt"></param>
        /// <returns>True if both segments contain the pt, false otherwise</returns>
        public static bool GetIntersection(Line2 line1, Line2 line2, float2 pos1, float2 pos2, bool strict, out float2 pt)
        {
            // The infinite lines intersect at exactly one point. The segments intersect
            // if they both contain that point. We will treat the lines as first-degree
            // Bezier lines to skip the vertical case
            // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection

            float x1 = line1.start.x + pos1.x;
            float x2 = line1.end.x + pos1.x;
            float x3 = line2.start.x + pos2.x;
            float x4 = line2.end.x + pos2.x;
            float y1 = line1.start.y + pos1.y;
            float y2 = line1.end.y + pos1.y;
            float y3 = line2.start.y + pos2.y;
            float y4 = line2.end.y + pos2.y;

            float det = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            // we assume det != 0 (lines not parallel)
            if(Math2.Approximately(det, 0))
            {
                pt = float2.zero;
                return false;
            }

            var t = (
                ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / det
            );

            pt = new float2(x1 + (x2 - x1) * t, y1 + (y2 - y1) * t);

            float min = strict ? Math2.DEFAULT_EPSILON : -Math2.DEFAULT_EPSILON;
            float max = 1 - min;

            if (t < min || t > max)
                return false;

            float u = -(
                ((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / det
            );
            return u >= min && u <= max;
        }

        /// <summary>
        /// Finds the line of overlap between the the two lines if there is
        /// one. If the two lines are not coincident (i.e., if the infinite
        /// lines are not the same) then they don't share a line of points.
        /// If they are coincident, they may still share no points (two
        /// seperate but coincident line segments), one point (they share
        /// an edge), or infinitely many points (the share a coincident
        /// line segment). In all but the last case, this returns false
        /// and overlap is set to null. In the last case this returns true
        /// and overlap is set to the line of overlap.
        /// </summary>
        /// <param name="a">The first line</param>
        /// <param name="b">The second line</param>
        /// <param name="pos1">The position of the first line</param>
        /// <param name="pos2">the position of the second line</param>
        /// <param name="overlap">Set to null or the line of overlap</param>
        /// <returns>True if a and b overlap at infinitely many points,
        /// false otherwise</returns>
        public static unsafe bool LineOverlap(Line2 a, Line2 b, float2 pos1, float2 pos2, out Line2 overlap)
        {
            if (!Parallel(a, b))
            {
                overlap = null;
                return false;
            }
            if (!AlongInfiniteLine(a, pos1, b.start + pos2))
            {
                overlap = null;
                return false;
            }

            float2 relOrigin = a.start + pos1;

            float* projs = stackalloc float[4] {
                0,
                a.magnitude,
                Math2.Dot((b.start + pos2) - relOrigin, a.axis),
                Math2.Dot((b.end + pos2) - relOrigin, a.axis)
            };

            bool* isFromLine1 = stackalloc bool[4] {
                true,
                true,
                false,
                false
            };

            FindSortedOverlap(projs, isFromLine1);

            if (isFromLine1[0] == isFromLine1[1])
            {
                // at best we overlap at one point, most likely no overlap
                overlap = null;
                return false;
            }

            if (Math2.Approximately(projs[1], projs[2]))
            {
                // Overlap at one point
                overlap = null;
                return false;
            }

            overlap = new Line2(
                relOrigin + projs[1] * a.axis,
                relOrigin + projs[2] * a.axis
            );
            return true;
        }

        /// <summary>
        /// Calculates the distance that the given point is from this line.
        /// Will be nearly 0 if the point is on the line.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="pos">The shift for the line</param>
        /// <param name="pt">The point that you want the distance from the line</param>
        /// <returns>The distance the point is from the line</returns>
        public static float Distance(Line2 line, float2 pos, float2 pt)
        {
            // As is typical for this type of question, we will solve along
            // the line treated as a linear space (which requires shifting 
            // so the line goes through the origin). We will use that to find
            // the nearest point on the line to the given pt, then just
            // calculate the distance normally.

            float2 relPt = pt - line.start - pos;

            float axisPart = Math2.Dot(relPt, line.axis);
            float nearestAxisPart;
            if (axisPart < 0)
                nearestAxisPart = 0;
            else if (axisPart > line.magnitude)
                nearestAxisPart = line.magnitude;
            else
                nearestAxisPart = axisPart;

            float2 nearestOnLine = line.start + pos + nearestAxisPart * line.axis;
            return math.length(pt - nearestOnLine);
        }

        /// <summary>
        /// Create a human-readable representation of this line
        /// </summary>
        /// <returns>human-readable string</returns>
        public override string ToString()
        {
            return $"[{start} to {end}]";
        }
    }
}
