using System;
using Unity.Mathematics;

namespace Plugins.SharpMath2Unity.Geometry2
{
    /// <summary>
    /// Describes a rotation about the z axis, with sin and cos of theta
    /// cached.
    /// </summary>
    public struct Rotation2
    {
        /// <summary>
        /// Rotation Theta=0
        /// </summary>
        public static readonly Rotation2 Zero = new Rotation2(0, 1, 0);

        /// <summary>
        /// Theta in radians.
        /// </summary>
        public readonly float theta;

        /// <summary>
        /// Math.Cos(Theta)
        /// </summary>
        public readonly float cosTheta;

        /// <summary>
        /// Math.Sin(Theta)
        /// </summary>
        public readonly float sinTheta;

        /// <summary>
        /// Create a new rotation by specifying the theta, its cosin, and its sin.
        /// 
        /// Theta will be normalized to 0 &lt;= theta &lt;= 2pi
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="cosTheta"></param>
        /// <param name="sinTheta"></param>
        public Rotation2(float theta, float cosTheta, float sinTheta)
        {
            if (float.IsInfinity(theta) || float.IsNaN(theta))
                throw new ArgumentException($"Invalid theta: {theta}", nameof(theta));

            this.theta = Standardize(theta);
            this.cosTheta = cosTheta;
            this.sinTheta = sinTheta;
        }

        /// <summary>
        /// Create a new rotation at the specified theta, calculating the cos and sin.
        /// 
        /// Theta will be normalized to 0 &lt;= theta &lt;= 2pi
        /// </summary>
        /// <param name="theta"></param>
        public Rotation2(float theta) : this(theta, math.cos(theta), math.sin(theta))
        {
        }

        /// <summary>
        /// Determine if the two rotations have the same theta
        /// </summary>
        /// <param name="r1">First rotation</param>
        /// <param name="r2">Second rotation</param>
        /// <returns>if r1 and r2 are the same logical rotation</returns>
        public static bool operator ==(Rotation2 r1, Rotation2 r2)
        {
            return Math2.Approximately(r1.theta, r2.theta);
        }

        /// <summary>
        /// Determine if the two rotations are not the same
        /// </summary>
        /// <param name="r1">first rotation</param>
        /// <param name="r2">second rotation</param>
        /// <returns>if r1 and r2 are not the same logical rotation</returns>
        public static bool operator !=(Rotation2 r1, Rotation2 r2)
        {
            return !Math2.Approximately(r1.theta, r2.theta);
        }

        /// <summary>
        /// Determine if obj is a rotation that is logically equal to this one
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>if it is logically equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Rotation2))
                return false;

            return this == ((Rotation2)obj);
        }

        /// <summary>
        /// The hashcode of this rotation based on just Theta
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return theta.GetHashCode();
        }

        /// <summary>
        /// Create a human-readable representation of this rotation
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return $"{theta} rads";
        }

        /// <summary>
        /// Standardizes the given angle to fall between 0 &lt;= theta &lt; 2 * PI
        /// </summary>
        /// <param name="theta">The radian angle to standardize</param>
        /// <returns>The standardized theta</returns>
        public static float Standardize(float theta)
        {
            if (theta < 0)
            {
                int numToAdd = (int)math.ceil((-theta) / (math.PI * 2));
                return theta + math.PI * 2 * numToAdd;
            }
            if (theta >= math.PI * 2)
            {
                int numToReduce = (int)math.floor(theta / (math.PI * 2));
                return theta - math.PI * 2 * numToReduce;
            }
            return theta;
        }
    }
}
