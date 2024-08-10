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
    public class CircleContains : AbstractVisualizer
    {
        public bool strict = false;
        public float circle1Radius = 1f;
        public Color circle1Color = Color.blue;
        public Color containsColor = Color.green;
        public Color notContainsColor = Color.red;
        private float _pointRadius = 0.6f;

        public override void DrawHandles(float4[] handleValues)
        {
            var circle1Pos = handleValues[0].xy + new float2(circle1Radius, circle1Radius);
            Handles.color = circle1Color;
            circle1Pos.xy = Handles.FreeMoveHandle(circle1Pos.ToWorld(), 0.25f,Vector3.zero, Handles.CubeHandleCap).AsMath().xz;
            circle1Radius = Handles.RadiusHandle(Quaternion.identity, circle1Pos.ToWorld(), circle1Radius, false);
            handleValues[0] = (circle1Pos.xy - new float2(circle1Radius, circle1Radius)).xyxy;
            
            
            
            var pointPos = handleValues[1].xy;
            pointPos.xy = Handles.FreeMoveHandle(pointPos.ToWorld(), 0.5f,Vector3.zero, Handles.CubeHandleCap).AsMath().xz;

            handleValues[1] = pointPos.xyxy;
        }

        public override void DrawGizmos(float4[] handleValues)
        {
            var p1 = handleValues[0].xy;
            var p1Visual = p1 + new float2(circle1Radius, circle1Radius);
            UGizmos.DrawCircle2D(p1Visual.ToWorld(), Quaternion.LookRotation(Vector3.up, Vector3.forward),
                circle1Radius, circle1Color);
            
            
            var p2 = handleValues[1].xy;
            var color = Circle2.Contains(new Circle2(circle1Radius), p1, p2, strict) ? containsColor : notContainsColor;
            UGizmos.DrawCircle2D(p2.ToWorld(), Quaternion.LookRotation(Vector3.up, Vector3.forward),
                _pointRadius, color);
        }
    }
}