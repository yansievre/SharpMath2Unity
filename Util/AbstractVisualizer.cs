using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.SharpMath2Unity.Util
{ [Toggle("isEnabled")]
   [Serializable]
   public abstract class AbstractVisualizer
    {
        [SerializeField]
        public bool isEnabled = false;

        public abstract void DrawHandles(float4[] handleValues);

        public abstract void DrawGizmos( float4[] handleValues);

    }
}