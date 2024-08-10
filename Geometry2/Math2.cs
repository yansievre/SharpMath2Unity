using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR

[assembly: BurstCompile(OptimizeFor = OptimizeFor.FastCompilation)]

#else

[assembly: BurstCompile(OptimizeFor = OptimizeFor.Performance)]

#endif

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// Contains utility functions for doing math in two-dimensions that
    /// don't fit elsewhere. Also contains any necessary constants.
    /// </summary>
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
    public static class Math2
    {
        /// <summary>
        /// Default epsilon
        /// </summary>
        public const float DEFAULT_EPSILON = 0.0001f;


        public static float Clip(this float value)
        {
            Clip(ref value);
            return value;
        }
        

        public static float2 Clip(this float2 value)
        {
            Clip(ref value);
            return value;
        }
        
        public static int2 ClipHash(this float2 value)
        {
            int2 res;
            
            res.x = (int) (math.floor(value.x / DEFAULT_EPSILON) * DEFAULT_EPSILON);
            res.y = (int)  (math.floor(value.y / DEFAULT_EPSILON) * DEFAULT_EPSILON);

            return res;
        }

        public static Vector3 ToWorld(this float2 value)
        {
            return new Vector3(value.x, 0, value.y);
        }

        public static Vector3 ToWorld(this float2 value, float y)
        {
            return new Vector3(value.x, y, value.y);
        }
        public static float3 AsMath(this Vector3 value)
        {
            return value;
        }

        
        /// <summary>
        /// Implemented for equality check in testing, avoid using this as you should never really need it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [BurstCompile]
        public static void Clip(ref float value)
        {
            //Remove unnecessary decimals depending on epsilon
            value = math.round(value / DEFAULT_EPSILON) * DEFAULT_EPSILON;
        }
        
        /// <summary>
        /// Implemented for equality check in testing, avoid using this as you should never really need it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [BurstCompile]
        public static void Clip(ref float2 value)
        {
            //Remove unnecessary decimals depending on epsilon
            value.x = math.round(value.x / DEFAULT_EPSILON) * DEFAULT_EPSILON;
            value.y = math.round(value.y / DEFAULT_EPSILON) * DEFAULT_EPSILON;
        }
        
        /// <summary>
        /// Determines if v1, v2, and v3 are collinear
        /// </summary>
        /// <param name="v1">Vector 1</param>
        /// <param name="v2">Vector 2</param>
        /// <param name="v3">Vector 3</param>
        /// <param name="epsilon">How close is close enough</param>
        /// <returns>If v1, v2, v3 is collinear</returns>
        [BurstCompile]
        public static bool IsOnLine(in float2 v1, in float2 v2, in float2 v3, in float epsilon = DEFAULT_EPSILON)
        {
            var fromV1ToV2 = v2 - v1;
            var axis = math.normalize(fromV1ToV2);
            var normal = Perpendicular(axis);

            var fromV1ToV3 = v3 - v1;
            var normalPortion = Dot(fromV1ToV3, normal);

            return Approximately(normalPortion, 0, epsilon);
        }

        /// <summary>
        /// Determines if the given pt is between the line between v1 and v2.
        /// </summary>
        /// <param name="v1">The first edge of the line</param>
        /// <param name="v2">The second edge of the line</param>
        /// <param name="pt">The point to test</param>
        /// <param name="epsilon">How close is close enough (not exactly distance)</param>
        /// <returns>True if pt is on the line between v1 and v2, false otherwise</returns>
        [BurstCompile]
        public static bool IsBetweenLine(in float2 v1,in float2 v2,in float2 pt, float epsilon = DEFAULT_EPSILON)
        {
            var fromV1ToV2 = v2 - v1;
            var axis = math.normalize(fromV1ToV2);
            var normal = Perpendicular(axis);

            var fromV1ToPt = pt - v1;
            var normalPortion = Dot(fromV1ToPt, normal);

            if (!Approximately(normalPortion, 0, epsilon))
                return false; // not on the infinite line

            var axisPortion = Dot(fromV1ToPt, axis);

            if (axisPortion < -epsilon)
                return false; // left of the first point

            if (axisPortion > math.length(fromV1ToV2) + epsilon)
                return false; // right of second point

            return true;
        }

        /// <summary>
        /// Computes the triple cross product (A x B) x A
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Result of projecting to 3 dimensions, performing the
        /// triple cross product, and then projecting back down to 2 dimensions.</param>
        [BurstCompile]
        public static void TripleCross(in float2 a, in float2 b, out float2 result)
        {
            result = new float2(
                -a.x * a.y * b.y + a.y * a.y * b.x,
                a.x * a.x * b.y - a.x * a.y * b.x
            );
        }

        /// <summary>
        /// Calculates the square of the area of the triangle made up of the specified points.
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <param name="v3">Third point</param>
        /// <returns>Area of the triangle made up of the given 3 points</returns>
        [BurstCompile]
        public static float AreaOfTriangle(in float2 v1,in  float2 v2,in  float2 v3)
        {
            return 0.5f * math.abs((v2.x - v1.x) * (v3.y - v1.y) - (v3.x - v1.x) * (v2.y - v1.y));
        }

        /// <summary>
        /// Finds a vector that is perpendicular to the specified vector.
        /// </summary>
        /// <returns>A vector perpendicular to v</returns>
        /// <param name="v">Vector</param>
        public static float2 Perpendicular(float2 v)
        {
            Perpendicular_Internal(v, out v);
            return v;
        }
        
        [BurstCompile]
        public static void Perpendicular_Internal(in float2 v, out float2 result)
        {
            result = new float2(-v.y, v.x);
        }


        /// <summary>
        /// Finds the dot product of (x1, y1) and (x2, y2)
        /// </summary>
        /// <returns>The dot.</returns>
        /// <param name="x1">The first x value.</param>
        /// <param name="y1">The first y value.</param>
        /// <param name="x2">The second x value.</param>
        /// <param name="y2">The second y value.</param>
        [BurstCompile]
        public static float Dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        /// <summary>
        /// Finds the dot product of the vector with itself
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float SelfDot(float x1, float y1)
        {
            return x1 * x1 + y1 * y1;
        }
        /// <summary>
        /// Finds the dot product of the two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>The dot product between v1 and v2</returns>
        [BurstCompile]
        public static float Dot(in float2 v1,in  float2 v2)
        {
            return Dot(v1.x, v1.y, v2.x, v2.y);
        }

        /// <summary>
        /// Finds the dot product of the vector with itself
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <returns>The dot product between v1 and v1</returns>
        [BurstCompile]
        public static float SelfDot(in float2 v1)
        {
            return SelfDot(v1.x, v1.y);
        }

        /// <summary>
        /// Finds the dot product of two vectors, where one is specified
        /// by its components
        /// </summary>
        /// <param name="v">The first vector</param>
        /// <param name="x2">The x-value of the second vector</param>
        /// <param name="y2">The y-value of the second vector</param>
        /// <returns>The dot product of v and (x2, y2)</returns>
        [BurstCompile]
        public static float Dot(in float2 v, float x2, float y2)
        {
            return Dot(v.x, v.y, x2, y2);
        }

        /// <summary>
        /// Determines if f1 and f2 are approximately the same.
        /// </summary>
        /// <returns>The approximately.</returns>
        /// <param name="f1">F1.</param>
        /// <param name="f2">F2.</param>
        /// <param name="epsilon">Epsilon.</param>
        [BurstCompile]
        public static bool Approximately(float f1, float f2, float epsilon = DEFAULT_EPSILON)
        {
            return math.abs(f1 - f2) <= epsilon;
        }

        /// <summary>
        /// Determines if vectors v1 and v2 are approximately equal, such that
        /// both coordinates are within epsilon.
        /// </summary>
        /// <returns>If v1 and v2 are approximately equal.</returns>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <param name="epsilon">Epsilon.</param>
        [BurstCompile]
        public static bool Approximately(in float2 v1,in  float2 v2, float epsilon = DEFAULT_EPSILON)
        {
            return Approximately(v1.x, v2.x, epsilon) && Approximately(v1.y, v2.y, epsilon);
        }


        /// <summary>
        /// Rotates the specified vector about the specified vector a rotation of the
        /// specified amount.
        /// </summary>
        /// <param name="vec">The vector to rotate</param>
        /// <param name="about">The point to rotate vec around</param>
        /// <param name="rotation">The rotation</param>
        /// <returns>The vector vec rotated about about rotation.Theta radians.</returns>
        public static float2 Rotate(float2 vec, float2 about, Rotation2 rotation)
        {
            Rotate_Internal(vec, about, rotation, out var result);
            return result;
        }

        [BurstCompile]
        internal static void Rotate_Internal(in float2 vec,in  float2 about,in Rotation2 rotation, out float2 result)
        {
            if (rotation.theta == 0)
            {
                result = vec;
                return;
            }
            var tmp = vec - about;
            result = new float2(
                tmp.x * rotation.cosTheta - tmp.y * rotation.sinTheta + about.x,
                tmp.x * rotation.sinTheta + tmp.y * rotation.cosTheta + about.y);
        }

        /// <summary>
        /// Returns either the vector or -vector such that MakeStandardNormal(vec) == MakeStandardNormal(-vec)
        /// </summary>
        /// <param name="vec">The vector</param>
        /// <returns>Normal such that vec.x is positive (unless vec.x is 0, in which such that vec.y is positive)</returns>
        public static float2 MakeStandardNormal(float2 vec)
        {
            MakeStandardNormal_Internal(vec, out var result);
            return result;
        }

        [BurstCompile]
        internal static void MakeStandardNormal_Internal(in float2 vec, out float2 result)
        { 
            if (vec.x < -DEFAULT_EPSILON)
            {
                result = -vec;
                return;
            }

            if (Approximately(vec.x, 0) && vec.y < 0)
            {
                result = -vec;
                return;
            }

            result = vec;
        }

        public static void AssertEqual(float value, float expected)
        {
            Assert.IsTrue(Approximately(value, expected), $"Expected {expected}, but it was {value}");
        }

        public static void AssertEqual(float2 value, float2 expected)
        {
            Assert.IsTrue(Approximately(value, expected), $"Expected {expected}, but it was {value}");
        }
    }
}
