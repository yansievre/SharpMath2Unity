using System;
using Plugins.SharpMath2Unity.Geometry2;
using Sirenix.OdinInspector;
using UGizmo;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Plugins.SharpMath2Unity.Util.Visualizers
{
    [Serializable]
    public class CircleCircleIntersection : AbstractVisualizer
    {
        public float circle1Radius = 1f;
        public float circle2Radius = 1f;
        public Color circle1Color = Color.blue;
        public Color circle2Color = Color.red;
        public Color pushedColor = Color.yellow;

        public override void DrawHandles(float4[] handleValues)
        {
            var circle1Pos = handleValues[0].xy + new float2(circle1Radius, circle1Radius);
            Handles.color = circle1Color;
            circle1Pos.xy = Handles.DoPositionHandle(circle1Pos.ToWorld(), Quaternion.identity).AsMath().xz;
            circle1Radius = Handles.RadiusHandle(Quaternion.identity, circle1Pos.ToWorld(), circle1Radius, false);
            handleValues[0] = (circle1Pos.xy - new float2(circle1Radius, circle1Radius)).xyxy;
            var circle2Pos = handleValues[1].xy + new float2(circle2Radius, circle2Radius);
            ;

            Handles.color = circle2Color;
            circle2Pos.xy = Handles.DoPositionHandle(circle2Pos.ToWorld(), Quaternion.identity).AsMath().xz;
            circle2Radius = Handles.RadiusHandle(Quaternion.identity, circle2Pos.ToWorld(), circle2Radius, false);

            handleValues[1] = (circle2Pos.xy - new float2(circle2Radius, circle2Radius)).xyxy;
        }

        public override void DrawGizmos(float4[] handleValues)
        {
            var p1 = handleValues[0].xy;
            var p1Visual = p1 + new float2(circle1Radius, circle1Radius);
            var c1 = new Circle2(circle1Radius);
            UGizmos.DrawCircle2D(p1Visual.ToWorld(), Quaternion.LookRotation(Vector3.up, Vector3.forward),
                circle1Radius, circle1Color);
            var p2 = handleValues[1].xy;
            var p2Visual = p2 + new float2(circle2Radius, circle2Radius);
            var c2 = new Circle2(circle2Radius);
            UGizmos.DrawCircle2D(p2Visual.ToWorld(), Quaternion.LookRotation(Vector3.up, Vector3.forward),
                circle2Radius, circle2Color);
            var result = Circle2.IntersectMTV(c1, c2, p1, p2);

            if (result != null)
            {
                var direction = result.Item1;
                var distance = result.Item2;
                UGizmos.DrawCircle2D((p1Visual + direction * distance).ToWorld(),
                    Quaternion.LookRotation(Vector3.up, Vector3.forward), circle1Radius, pushedColor);
            }
        }
    }
}