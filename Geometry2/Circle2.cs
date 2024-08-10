using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// Describes a circle in the x-y plane.
    /// </summary>
    public struct Circle2
    {
        /// <summary>
        /// The radius of the circle
        /// </summary>
        public readonly float Radius;

        /// <summary>
        /// Constructs a circle with the specified radius
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        public Circle2(float radius)
        {
            Radius = radius;
        }

        /// <summary>
        /// Determines if the first circle is equal to the second circle
        /// </summary>
        /// <param name="c1">The first circle</param>
        /// <param name="c2">The second circle</param>
        /// <returns>If c1 is equal to c2</returns>
        public static bool operator ==(Circle2 c1, Circle2 c2)
        {
            return CircleMethods.Equals(c1.Radius, c2.Radius);
        }

        /// <summary>
        /// Determines if the first circle is not equal to the second circle
        /// </summary>
        /// <param name="c1">The first circle</param>
        /// <param name="c2">The second circle</param>
        /// <returns>If c1 is not equal to c2</returns>
        public static bool operator !=(Circle2 c1, Circle2 c2)
        {
            return CircleMethods.NotEquals(c1.Radius, c2.Radius);
        }
        
        /// <summary>
        /// Determines if this circle is logically the same as the 
        /// specified object.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>if it is a circle with the same radius</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) || obj.GetType() != typeof(Circle2))
                return false;

            var other = (Circle2)obj;
            return this == other;
        }

        /// <summary>
        /// Calculate a hashcode based solely on the radius of this circle.
        /// </summary>
        /// <returns>hashcode</returns>
        public override int GetHashCode()
        {
            return Radius.GetHashCode();
        }

        /// <summary>
        /// Determines if the circle at the specified position contains the point
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="pos">The top-left of the circles bounding box</param>
        /// <param name="point">The point to check if is in the circle at pos</param>
        /// <param name="strict">If the edges do not count</param>
        /// <returns>If the circle at pos contains point</returns>
        public static bool Contains(Circle2 circle, float2 pos, float2 point, bool strict)
        {
            return strict ? CircleMethods.ContainsStrict(circle.Radius, pos, point) : CircleMethods.Contains(circle.Radius, pos, point);
        }
        
        /// <summary>
        /// Determines if the first circle at the specified position intersects the second circle
        /// at the specified position.
        /// </summary>
        /// <param name="circle1">First circle</param>
        /// <param name="circle2">Second circle</param>
        /// <param name="pos1">Top-left of the bounding box of the first circle</param>
        /// <param name="pos2">Top-left of the bounding box of the second circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle1 at pos1 intersects circle2 at pos2</returns>
        public static bool Intersects(Circle2 circle1, Circle2 circle2, float2 pos1, float2 pos2)
        {
            return Intersects(circle1.Radius, circle2.Radius, pos1, pos2);
        }

        /// <summary>
        /// Determines if the first circle of specified radius and (bounding box top left) intersects
        /// the second circle of specified radius and (bounding box top left)
        /// </summary>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="radius2">Radius of the second circle</param>
        /// <param name="pos1">Top-left of the bounding box of the first circle</param>
        /// <param name="pos2">Top-left of the bounding box of the second circle</param>
        /// <returns>If circle1 of radius=radius1, topleft=pos1 intersects circle2 of radius=radius2, topleft=pos2</returns>
        public static bool Intersects(float radius1, float radius2, float2 pos1, float2 pos2)
        {
            return CircleMethods.Intersects(radius1, radius2, pos1, pos2);
        }

        /// <summary>
        /// Determines the shortest axis and overlap for which the first circle at the specified position
        /// overlaps the second circle at the specified position. If the circles do not overlap, returns null.
        /// </summary>
        /// <param name="circle1">First circle</param>
        /// <param name="circle2">Second circle</param>
        /// <param name="pos1">Top-left of the first circles bounding box</param>
        /// <param name="pos2">Top-left of the second circles bounding box</param>
        /// <returns></returns>
        public static Tuple<float2, float> IntersectMTV(Circle2 circle1, Circle2 circle2, float2 pos1, float2 pos2)
        {
            return IntersectMTV(circle1.Radius, circle2.Radius, pos1, pos2);
        }

        /// <summary>
        /// Determines the shortest axis and overlap for which the first circle, specified by its radius and its bounding
        /// box's top-left, intersects the second circle specified by its radius and bounding box top-left. Returns null if
        /// the circles do not overlap.
        /// </summary>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="radius2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns>The direction and magnitude to move pos1 to prevent intersection</returns>
        public static Tuple<float2, float> IntersectMTV(float radius1, float radius2, float2 pos1, float2 pos2)
        {
            return CircleMethods.IntersectMTV(radius1, radius2, pos1, pos2, out var result) ? new Tuple<float2, float>(result.xy, result.z) : null;
        }

        /// <summary>
        /// Projects the specified circle with the upper-left at the specified position onto
        /// the specified axis. 
        /// </summary>
        /// <param name="circle">The circle</param>
        /// <param name="pos">The position of the circle</param>
        /// <param name="axis">the axis to project along</param>
        /// <returns>Projects circle at pos along axis</returns>
        public static AxisAlignedLine2 ProjectAlongAxis(Circle2 circle, float2 pos, float2 axis)
        {
            return ProjectAlongAxis(circle.Radius, pos, axis);
        }

        /// <summary>
        /// Projects a circle defined by its radius and the top-left of its bounding box along
        /// the specified axis.
        /// </summary>
        /// <param name="radius">Radius of the circle to project</param>
        /// <param name="pos">Position of the circle</param>
        /// <param name="axis">Axis to project on</param>
        /// <returns></returns>
        public static AxisAlignedLine2 ProjectAlongAxis(float radius, float2 pos, float2 axis)
        {
            CircleMethods.ProjectAlongAxis(radius, pos, axis, out float2 minMax);

            return new AxisAlignedLine2(axis, minMax.x, minMax.y);
        }
    }

    [BurstCompile]
    public static class CircleMethods
    {
        [BurstCompile]
        public static float ProjectCircleCenter(in float2 pos, in float2 axis)
        {
            return math.dot(pos, axis);
        }

        /// <summary>
        /// Projects a circle defined by its radius and the top-left of its bounding box along
        /// the specified axis. This overload returns the min and max values of the projection and the axis.
        /// </summary>
        /// <param name="radius">Radius of the circle to project</param>
        /// <param name="pos">Position of the circle</param>
        /// <param name="axis">Axis to project on</param>
        /// <param name="line">xy: axis, z: min, w: max</param>
        /// <returns></returns>
        [BurstCompile]
        public static void ProjectAlongAxis(float radius, in float2 pos, in float2 axis, out float4 line)
        {
            var projectedCenter = ProjectCircleCenter(pos, axis);

            line = new float4(axis, projectedCenter - radius, projectedCenter + radius);
        }

        /// <summary>
        /// Projects a circle defined by its radius and the top-left of its bounding box along
        /// the specified axis. This overload returns the min and max values as a float2.
        /// </summary>
        /// <param name="radius">Radius of the circle to project</param>
        /// <param name="pos">Position of the circle</param>
        /// <param name="axis">Axis to project on</param>
        /// <param name="line">x: min, y: max</param>
        /// <returns></returns>
        [BurstCompile]
        public static void ProjectAlongAxis(float radius, in float2 pos, in float2 axis, out float2 line)
        {
            var projectedCenter = ProjectCircleCenter( pos, axis);

            line = new float2(projectedCenter - radius, projectedCenter + radius);
        }
        
        
        /// <summary>
        /// Intersect and get minimum translation value
        /// </summary>
        /// <param name="radius1"></param>
        /// <param name="radius2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="result">xy: movement direction, z: movement magnitude</param>
        /// <returns></returns>
        [BurstCompile]
        public static bool IntersectMTV(float radius1, in float2 pos1, float radius2, in float2 pos2, out float3 result)
        {
            var betweenVec = pos1 - pos2;
            var len = math.length(betweenVec);
            
            if(len * len < (radius1 + radius2) * (radius1 + radius2))
            {
                betweenVec *= (1 / len);

                result = new float3(betweenVec, radius1 + radius2 - len);

                return true;
            }
            result = float3.zero;
            return false;
        }
        
        /// <summary>
        /// Intersect and get minimum translation value
        /// </summary>
        /// <param name="radiusSum"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="result">xy: movement direction, z: movement magnitude</param>
        /// <returns></returns>
        [BurstCompile]
        public static bool IntersectMTV(float radiusSum, in float2 pos1, in float2 pos2, out float3 result)
        {
            var betweenVec = pos1 - pos2;
            var len = math.length(betweenVec);
            
            if(len * len < (radiusSum) * (radiusSum))
            {
                betweenVec *= (1 / len);

                result = new float3(betweenVec, radiusSum - len);

                return true;
            }
            result = float3.zero;
            return false;
        }
        
        /// <summary>
        /// Intersect and get minimum translation value
        /// </summary>
        /// <param name="radiusSum"></param>
        /// <param name="radiusSumSq"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="result">xy: movement direction, z: movement magnitude</param>
        /// <returns></returns>
        [BurstCompile]
        public static bool IntersectMTV(float radiusSum, float radiusSumSq, in float2 pos1, in float2 pos2, out float3 result)
        {
            var betweenVec = pos1 - pos2;
            var len = math.length(betweenVec);
            
            if(len * len < radiusSumSq)
            {
                betweenVec *= (1 / len);

                result = new float3(betweenVec, radiusSum - len);

                return true;
            }
            result = float3.zero;
            return false;
        }
        
        [BurstCompile]
        public static bool Intersects(float radius1, float radius2, in float2 pos1, in float2 pos2)
        {
            return  math.lengthsq(pos1 - pos2) < (radius1 + radius2) * (radius1 + radius2);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radiusSumSq">The squared sum of the radii</param>
        /// <param name="pos1">Circle 1 position</param>
        /// <param name="pos2">Circle 2 position</param>
        /// <returns></returns>
        [BurstCompile]
        public static bool IntersectsSq(float radiusSumSq, in float2 pos1, in float2 pos2)
        {
            return math.lengthsq(pos1 - pos2) < radiusSumSq;
        }
        
        [BurstCompile]
        public static void ClosestPointOnSurface(float radius, in float2 pos, in float2 point, out float2 closestPoint)
        {
            closestPoint = pos + math.normalize(point - pos) * radius;
        }
        
        [BurstCompile]
        public static void ClosestPointOnSurface(float radius, in float2 pos, in float2 point, out float2 closestPoint, out float distance)
        {
            var toPoint = point - pos;
            distance = math.length(toPoint);
            closestPoint = pos + toPoint * (1/distance) * radius;
            distance -= radius;
        }
        
        [BurstCompile]
        public static void ClosestPointOnSurface(float radius, in float2 pos, in float2 point, out float2 closestPoint, out float2 normal, out float distance)
        {
            var toPoint = point - pos;
            distance = math.length(toPoint);
            normal = toPoint * (1 / distance);
            closestPoint = pos + toPoint * (1/distance) * radius;
            distance -= radius;
        }
        
        [BurstCompile]
        public static void ClosestPointOnSurface(float radius, in float2 pos, in float2 point, out float2 closestPoint, out float2 normal)
        {
            normal = math.normalize(point - pos);
            closestPoint = pos + normal * radius;
        }

        /// <summary>
        /// Only checks if line cast hits a circle with radius sq at circlePos
        /// </summary>
        /// <param name="radiusSq"></param>
        /// <param name="circlePos"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        [BurstCompile]
        public static bool LineCast(float radiusSq, in float2 circlePos, in float2 lineStart, in float2 lineEnd)
        {
            return LineCast(radiusSq, lineStart - circlePos, lineEnd - circlePos);
        }
        
        /// <summary>
        /// Only checks if line cast hits a circle with radius sq at origin
        /// </summary>
        /// <param name="radiusSq"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        [BurstCompile]
        public static bool LineCast(float radiusSq, in float2 lineStart, in float2 lineEnd)
        {
            return  radiusSq * Math2.SelfDot(lineEnd - lineStart) - (lineStart.x * lineEnd.y - lineEnd.x * lineStart.y) > -Math2.DEFAULT_EPSILON;
        }
        

        [BurstCompile]
        public static bool Contains(float radius, in float2 pos, in float2 point)
        {
            return math.lengthsq(point - pos) <= radius * radius;
        }
        
        [BurstCompile]
        public static bool ContainsStrict(float radius, in float2 pos, in float2 point)
        {
            return math.lengthsq(point - pos) < radius * radius;
        }
        
        [BurstCompile]
        public static bool Contains_Sq(float radiusSquared, in float2 pos, in float2 point)
        {
            return math.lengthsq(point - pos) <= radiusSquared;
        }
        
        [BurstCompile]
        public static bool ContainsStrict_Sq(float radiusSquared, in float2 pos, in float2 point)
        {
            return math.lengthsq(point - pos) < radiusSquared;
        }
        
        [BurstCompile]
        public static bool Equals(float c1, float c2)
        {
            return math.abs(c1 - c2) < math.EPSILON;
        }
        
        [BurstCompile]
        public static bool NotEquals(float c1, float c2)
        {
            return math.abs(c1 - c2) > math.EPSILON;
        }
    }
}
