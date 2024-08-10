using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.SharpMath2Unity.Util;
using Plugins.SharpMath2Unity.Util.Visualizers;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.SharpMath2Unity
{
    public class GeometryVisualizer : MonoBehaviour
    {
        [SerializeField]
        private CircleCircleIntersection _circleCircleIntersection = new CircleCircleIntersection();
        [SerializeField]
        private CircleProjection _circleProjection = new CircleProjection();
        [SerializeField]
        private CircleContains _circleContains = new CircleContains();


        private float4[] _handleValues = new float4[10];
        
        private IEnumerable<AbstractVisualizer> Visualizers {
            get
            {
                yield return _circleCircleIntersection;
                yield return _circleProjection;
                yield return _circleContains;
            }
        }

        private float4[] HandleValues
        {
            get
            {
                if(_handleValues == null || _handleValues.Length < 10)
                    _handleValues = new float4[10];
                return _handleValues;
            }
            set => _handleValues = value;
        }
        
        private void OnDrawGizmos()
        {
            foreach (var visualizer in Visualizers)
            {
                DrawGizmos(visualizer);
            }
        }

        public void DrawHandles()
        {
            foreach (var visualizer in Visualizers)
            {
                DrawHandles(visualizer);
            }
        }

        private void DrawGizmos(AbstractVisualizer abstractVisualizer)
        {
            if (abstractVisualizer.isEnabled)
            {
                abstractVisualizer.DrawGizmos(HandleValues);
            }
        }

        private void DrawHandles(AbstractVisualizer abstractVisualizer)
        {
            if (abstractVisualizer.isEnabled)
            {
                abstractVisualizer.DrawHandles(HandleValues);
            }
        }
    }
}