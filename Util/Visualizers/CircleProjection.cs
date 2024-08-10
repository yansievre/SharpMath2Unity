using System;
using System.Collections.Generic;
using Plugins.SharpMath2Unity.Geometry2;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UGizmo;

namespace Plugins.SharpMath2Unity.Util.Visualizers
{
    [Serializable]
    public class CircleProjection : AbstractVisualizer
    {
        public float circle1Radius = 1f;
        public Color circle1Color = Color.blue;
        public Color circle2Color = Color.red;
        public Color projectionColor = Color.yellow;

        public override void DrawHandles(float4[] handleValues)
        {
            var circle1Pos = handleValues[0].xy + new float2(circle1Radius, circle1Radius);
            Handles.color = circle1Color;
            circle1Pos.xy = Handles.DoPositionHandle(circle1Pos.ToWorld(), Quaternion.identity).AsMath().xz;
            circle1Radius = Handles.RadiusHandle(Quaternion.identity, circle1Pos.ToWorld(), circle1Radius, false);
            handleValues[0] = (circle1Pos.xy - new float2(circle1Radius, circle1Radius)).xyxy;

            var axis = handleValues[1].xy;
            Handles.color = circle2Color;

            var rotPos = axis.ToWorld() * 15f;
            Handles.DrawDottedLine(Vector3.zero, rotPos, 3f);
            axis = math.normalize(
                Handles.FreeMoveHandle(rotPos, 0.5f, Vector3.one, Handles.SphereHandleCap).AsMath().xz);

            handleValues[1] = axis.xyxy;
        }

        public override void DrawGizmos(float4[] handleValues)
        {
            var p1 = handleValues[0].xy;
            var p1Visual = p1 + new float2(circle1Radius, circle1Radius);
            var c1 = new Circle2(circle1Radius);
            UGizmos.DrawCircle2D(p1Visual.ToWorld(), Quaternion.LookRotation(Vector3.up, Vector3.forward),
                circle1Radius, circle1Color);

            var axis = handleValues[1].xy;
            var line = Circle2.ProjectAlongAxis(c1, p1, axis);
            UGizmos.DrawLine((line.Axis * line.Min).ToWorld(), (line.Axis * line.Max).ToWorld(), projectionColor);
        }
    }
}