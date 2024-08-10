using System.Collections;
using NUnit.Framework;
using Plugins.SharpMath2Unity.Geometry2;
using Unity.Mathematics;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Plugins.SharpMath2Unity.Tests
{
    using Math2 = Math2;
    [TestFixture]
    public class Math2Tests
    {
        #region Approximately
        
        [Test]
        public void Approximately_ValuesWithinEpsilon_ReturnsTrue()
        {
            float f1 = 1.0000f;
            float f2 = 1.00001f;

            bool result = Math2.Approximately(f1, f2);

            Assert.IsTrue(result);
        }
        
        #endregion


        #region IsOnLine

        [Test]
        public void IsOnLine_WithCollinearPoints_ReturnsTrue()
        {
            float2 v1 = new float2(0, 0);
            float2 v2 = new float2(10, 10);
            float2 v3 = new float2(5, 5); // Collinear point

            bool result = Math2.IsOnLine(v1, v2, v3);

            Assert.IsTrue(result);
        }

        #endregion


        #region IsBetweenLine

        [Test]
        public void IsBetweenLine_WithPointOnLine_ReturnsTrue()
        {
            float2 v1 = new float2(0, 0);
            float2 v2 = new float2(10, 0);
            float2 pt = new float2(5, 0); // Point on the line

            bool result = Math2.IsBetweenLine(v1, v2, pt);

            Assert.IsTrue(result);
        }

        #endregion
        
        #region AreaOfTriangle

        [Test]
        public void AreaOfTriangle_CalculatesCorrectArea()
        {
            float2 v1 = new float2(0, 0);
            float2 v2 = new float2(4, 0);
            float2 v3 = new float2(0, 3);

            float expectedArea = 6.0f; // Half of base times height
            float area = Math2.AreaOfTriangle(v1, v2, v3);

            Assert.IsTrue(Math2.Approximately(expectedArea, area));
        }
        
        [Test]
        public void AreaOfTriangle_CalculatesCorrectArea2()
        {
            float2 v1 = new float2(0, 0);
            float2 v2 = new float2(4, 0);
            float2 v3 = new float2(0, 4);

            float expectedArea = 8.0f; // Half of base times height
            float area = Math2.AreaOfTriangle(v1, v2, v3);

            Assert.IsTrue(Math2.Approximately(expectedArea, area));
        }

        #endregion
        
        //triple cross tests

        #region Triple Cross

        [Test]
        public void TripleCross_CalculatesCorrectResult()
        {
            float2 a = new float2(1, 2);
            float2 b = new float2(3, 4);

            float2 expected = new float2(4, -2);
            Math2.TripleCross(a, b, out float2 result);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        [Test]
        public void TripleCross_CalculatesCorrectResult2()
        {
            float2 a = new float2(1, 2);
            float2 b = new float2(2, 1);

            float2 expected = new float2(6, -3);
            Math2.TripleCross(a, b, out float2 result);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }

        #endregion

        #region Perpendicular

        [Test]
        public void Perpendicular_CalculatesCorrectResult()
        {
            float2 v = new float2(1, 2);

            float2 expected = new float2(-2, 1);
            float2 result = Math2.Perpendicular(v);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        [Test]
        public void Perpendicular_CalculatesCorrectResult2()
        {
            float2 v = new float2(2, 1);

            float2 expected = new float2(-1, 2);
            float2 result = Math2.Perpendicular(v);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        #endregion

        #region Dot

        [Test]
        public void Dot_CalculatesCorrectResult()
        {
            float x1 = 1;
            float y1 = 2;
            float x2 = 3;
            float y2 = 4;

            float expected = 11;
            float result = Math2.Dot(x1, y1, x2, y2);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        [Test]
        public void Dot_CalculatesCorrectResult2()
        {
            float x1 = 2;
            float y1 = 1;
            float x2 = 1;
            float y2 = 2;

            float expected = 4;
            float result = Math2.Dot(x1, y1, x2, y2);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        [Test]
        public void Dot_CalculatesCorrectResult3()
        {
            float2 v1 = new float2(1, 2);
            float2 v2 = new float2(3, 4);

            float expected = 11;
            float result = Math2.Dot(v1, v2);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        [Test]
        public void Dot_CalculatesCorrectResult4()
        {
            float2 v = new float2(1, 2);
            float x2 = 3;
            float y2 = 4;

            float expected = 11;
            float result = Math2.Dot(v, x2, y2);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        //edge cases
        [Test]
        public void Dot_CalculatesCorrectResult5()
        {
            float2 v = new float2(1, 2);
            float x2 = 0;
            float y2 = 0;

            float expected = 0;
            float result = Math2.Dot(v, x2, y2);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }
        
        [Test]
        public void Dot_CalculatesCorrectResult6()
        {
            float2 v = new float2(0, 0);
            float x2 = 3;
            float y2 = 4;

            float expected = 0;
            float result = Math2.Dot(v, x2, y2);

            Assert.IsTrue(Math2.Approximately(expected, result));
        }

        #endregion

        #region Rotate
        public static IEnumerable RotateCases
        {
            get
            {
                yield return new TestCaseData(1, 0, 0, 0, 90).Returns(new float2(0, 1));
                yield return new TestCaseData( 0, 1, 0, 0, 180).Returns(new float2(0, -1));
                yield return new TestCaseData(0, 0, 1, 1, 90).Returns(new float2(2, 0));
                yield return new TestCaseData(1, 1, 0, 0, 0).Returns(new float2(1, 1));
                yield return new TestCaseData(1, 0, 0, 0, 360).Returns(new float2(1 ,0));
            }
        }
        
        [TestCaseSource(nameof(RotateCases))]
        public float2 Rotate_VectorAroundPointWithGivenAngle_ReturnsExpectedResult(float vecX, float vecY, float aboutX, float aboutY, float angle)
        {
            var vec = new float2(vecX, vecY);
            var about = new float2(aboutX, aboutY);
            var rotation = new Rotation2(math.radians(angle));

            var res = Math2.Rotate(vec, about, rotation);
            Math2.Clip(ref res);
            return res;
        }

        #endregion

        #region MakeStandardNormal

        [TestCase(-1, 0, ExpectedResult = new float[] {1, 0})]
        [TestCase(0, -1, ExpectedResult = new float[] {0, 1})]
        [TestCase(-1, -1, ExpectedResult = new float[] {1, 1})]
        [TestCase(0, 0, ExpectedResult = new float[] {0, 0})] // Edge case: zero vector remains unchanged
        [TestCase(1, 1, ExpectedResult = new float[] {1, 1})] // Positive vector remains unchanged
        public float[] MakeStandardNormal_PositiveXOrY_ReturnsExpectedResult(float x, float y)
        {
            var vec = new float2(x, y);
            var result = Math2.MakeStandardNormal(vec);
            return new [] { result.x, result.y };
        }

        #endregion
    }
}