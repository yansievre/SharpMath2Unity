using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// Describes a line that's projected onto a specified axis. This is a useful
    /// mathematical concept. Axis aligned lines *do* have position because they 
    /// are only used as an interim calculation, where position won't change.
    /// </summary>
    [BurstCompile]
    public class AxisAlignedLine2
    {
        /// <summary>
        /// The axis that this projected line is on. Optional.
        /// </summary>
        public readonly float2 Axis;

        /// <summary>
        /// The minimum of this line
        /// </summary>
        public readonly float Min;

        /// <summary>
        /// The maximum of this line
        /// </summary>
        public readonly float Max;

        /// <summary>
        /// Initializes an an axis aligned line. Will autocorrect if min &gt; max
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <param name="min">The min</param>
        /// <param name="max">The max</param>
        public AxisAlignedLine2(float2 axis, float min, float max)
        {
            Axis = axis;

            CorrectMinMax(ref min, ref max);
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Determines if line1 intersects line2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line2">Line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 and line2 intersect</returns>
        /// <exception cref="ArgumentException">if line1.Axis != line2.Axis</exception>
        public static bool Intersects(AxisAlignedLine2 line1, AxisAlignedLine2 line2, bool strict)
        {
            if (!line1.Axis.Equals(line2.Axis))
                throw new ArgumentException($"Lines {line1} and {line2} are not aligned - you will need to convert to Line2 to check intersection.");

            return Intersects(line1.Min, line1.Max, line2.Min, line2.Max, strict, false);
        }
        
        /// <summary>
        /// Determines the best way for line1 to move to prevent intersection with line2
        /// </summary>
        /// <param name="line1">Line1</param>
        /// <param name="line2">Line2</param>
        /// <returns>MTV for line1</returns>
        public static float? IntersectMTV(AxisAlignedLine2 line1, AxisAlignedLine2 line2)
        {
            if (!line1.Axis.Equals(line2.Axis))
                throw new ArgumentException($"Lines {line1} and {line2} are not aligned - you will need to convert to Line2 to check intersection.");

            return IntersectMTV_Internal(line1.Min, line1.Max, line2.Min, line2.Max, true, out var distance) ? distance : null;
        }


        /// <summary>
        /// Determines if axis aligned line (min1, max1) intersects (min2, max2)
        /// </summary>
        /// <param name="min1">Min 1</param>
        /// <param name="max1">Max 1</param>
        /// <param name="min2">Min 2</param>
        /// <param name="max2">Max 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <param name="correctMinMax">If true (default true) mins and maxes will be swapped if in the wrong order</param>
        /// <returns>If (min1, max1) intersects (min2, max2)</returns>
        [BurstCompile]
        public static bool Intersects(float min1, float max1, float min2, float max2, bool strict, bool correctMinMax = true)
        {
            if (correctMinMax)
                CorrectMinMax(ref min1, ref max1, ref min2, ref max2);

            if (strict)
                return (min1 <= min2 && max1 > min2 + Math2.DEFAULT_EPSILON) || (min2 <= min1 && max2 > min1 + Math2.DEFAULT_EPSILON);

            return (min1 <= min2 && max1 > min2 - Math2.DEFAULT_EPSILON) || (min2 <= min1 && max2 > min1 - Math2.DEFAULT_EPSILON);
        }
        
        
        
        /// <summary>
        /// Determines the translation to move line 1 to have line 1 not intersect line 2. Returns
        /// null if line1 does not intersect line1.
        /// </summary>
        /// <param name="min1">Line 1 min</param>
        /// <param name="max1">Line 1 max</param>
        /// <param name="min2">Line 2 min</param>
        /// <param name="max2">Line 2 max</param>
        /// <param name="correctMinMax">If mins and maxs might be reversed</param>
        /// <param name="distance"></param>
        /// <returns>a number to move along the projected axis (positive or negative) or null if no intersection</returns>
        [BurstCompile]
        internal static bool IntersectMTV_Internal(float min1, float max1, float min2, float max2, bool correctMinMax, out float distance)
        {
            if (correctMinMax)
                CorrectMinMax(ref min1, ref max1, ref min2, ref max2);

            if (min1 <= min2 && max1 > min2)
            {
                distance = min2 - max1;

                return true;
            }
            if (min2 <= min1 && max2 > min1)
            {
                distance = max2 - min1;
                return true;
            }

            distance = 0;
            return false;
        }

        
        /// <summary>
        /// Corrects min max
        /// </summary>
        /// <param name="min1">Line 1 min</param>
        /// <param name="max1">Line 1 max</param>
        /// <param name="min2">Line 2 min</param>
        /// <param name="max2">Line 2 max</param>
        [BurstCompile]
        private static void CorrectMinMax(ref float min1, ref float max1, ref float min2, ref float max2)
        {
            if (min1 > max1)
                (min1, max1) = (max1, min1);
            if (min2 > max2)
                (min2, max2) = (max2, min2);
        }
        
        /// <summary>
        /// Corrects min max
        /// </summary>
        /// <param name="min1">Line 1 min</param>
        /// <param name="max1">Line 1 max</param>
        /// <param name="min2">Line 2 min</param>
        /// <param name="max2">Line 2 max</param>
        [BurstCompile]
        private static void CorrectMinMax(ref float min1, ref float max1)
        {
            if (min1 > max1)
                (min1, max1) = (max1, min1);
        }

        /// <summary>
        /// Determines if the specified line contains the specified point.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="point">The point</param>
        /// <param name="strict">If the edges of the line are excluded</param>
        /// <returns>if line contains point</returns>
        public static bool Contains(AxisAlignedLine2 line, float point, bool strict)
        {
            return strict ? ContainsStrict(line.Min, line.Max, point, false) :  Contains(line.Min, line.Max, point, false);
        }

        /// <summary>
        /// Determines if the line from (min, max) contains point
        /// </summary>
        /// <param name="min">Min of line</param>
        /// <param name="max">Max of line</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">If edges are excluded</param>
        /// <param name="correctMinMax">if true (default true) min and max will be swapped if in the wrong order</param>
        /// <returns>if line (min, max) contains point</returns>
        [BurstCompile]
        public static bool Contains(float min, float max, float point, bool strict,  bool correctMinMax)
        {
            return strict ? ContainsStrict(min, max, point, correctMinMax) :  Contains(min, max, point, correctMinMax);
        }
        
        /// <summary>
        /// Determines if the line from (min, max) contains point
        /// </summary>
        /// <param name="min">Min of line</param>
        /// <param name="max">Max of line</param>
        /// <param name="point">Point to check</param>
        /// <param name="correctMinMax">if true (default true) min and max will be swapped if in the wrong order</param>
        /// <returns>if line (min, max) contains point</returns>
        [BurstCompile]
        public static bool Contains(float min, float max, float point, bool correctMinMax)
        {
            if (correctMinMax)
                CorrectMinMax(ref min, ref max);

            return min <= point && max >= point;
        }


        /// <summary>
        /// Determines if the line from (min, max) contains point
        /// </summary>
        /// <param name="min">Min of line</param>
        /// <param name="max">Max of line</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">If edges are excluded</param>
        /// <param name="correctMinMax">if true (default true) min and max will be swapped if in the wrong order</param>
        /// <returns>if line (min, max) contains point</returns>
        [BurstCompile]
        public static bool ContainsStrict(float min, float max, float point, bool correctMinMax)
        {
            if (correctMinMax)
                CorrectMinMax(ref min, ref max);

            return min < point && max > point;
        }
        
        /// <summary>
        /// Detrmines the shortest distance from the line to get to point. Returns
        /// null if the point is on the line (not strict). Always returns a positive value.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="line">Line.</param>
        /// <param name="point">Point.</param>
        public static float? MinDistance(AxisAlignedLine2 line, float point)
        {
            return MinDistance(line.Min, line.Max, point, false, out var distance)? distance : null;
        }

        /// <summary>
        /// Determines the shortest distance for line1 to go to touch line2. Returns
        /// null if line1 and line 2 intersect (not strictly)
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="line1">Line1.</param>
        /// <param name="line2">Line2.</param>
        public static float? MinDistance(AxisAlignedLine2 line1, AxisAlignedLine2 line2)
        {
            return MinDistance_Internal(line1.Min, line1.Max, line2.Min, line2.Max, false, out float distance) ? distance : null;
        }

        /// <summary>
        /// Determines the shortest distance from the line (min, max) to the point. Returns
        /// null if the point is on the line (not strict). Always returns a positive value.
        /// </summary>
        /// <returns>Has value</returns>
        /// <param name="min">Minimum of line.</param>
        /// <param name="max">Maximum of line.</param>
        /// <param name="point">Point to check.</param>
        /// <param name="correctMinMax">If set to <c>true</c> will correct minimum max being reversed if they are</param>
        /// <param name="distance">The distance.</param>
        [BurstCompile]
        public static bool MinDistance(float min, float max, float point, bool correctMinMax, out float distance)
        {
            if (correctMinMax)
                CorrectMinMax(ref min, ref max);

            distance = 0;

            if (point < min)
            {

                distance =  min - point;
                return true;
            }

            if (point > max)
            {

                distance = point - max;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the shortest distance for line1 (min1, max1) to get to line2 (min2, max2).
        /// Returns null if line1 and line2 intersect (not strictly)
        /// </summary>
        /// <returns>The distance along the mutual axis or null.</returns>
        /// <param name="min1">Min1.</param>
        /// <param name="max1">Max1.</param>
        /// <param name="min2">Min2.</param>
        /// <param name="max2">Max2.</param>
        /// <param name="correctMinMax">If set to <c>true</c> correct minimum max being potentially reversed.</param>
        [BurstCompile]
        internal static bool MinDistance_Internal(float min1, float max1, float min2, float max2, bool correctMinMax, out float distance)
        {
            if (correctMinMax)
                CorrectMinMax(ref min1, ref max1, ref min2, ref max2);

            distance = 0;
            if (min1 < min2)
            {
                if (!(max1 < min2)) 
                    return false;
                distance = min2 - max1;

                return true;
            }

            if (!(min2 < min1)) 
                return false;
            if (!(max2 < min1)) 
                return false;
            
            distance = min1 - max2;

            return true;

        }

        /// <summary>
        /// Creates a human-readable representation of this line
        /// </summary>
        /// <returns>string representation of this vector</returns>
        public override string ToString()
        {
            return $"[{Min} -> {Max} along {Axis}]";
        }
    }
}
