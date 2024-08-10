using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Plugins.SharpMath2Unity.Tests
{
    [CustomEditor(typeof(GeometryVisualizer))]
    public class GeometryVisualizerSceneGUI : OdinEditor
    {
        private void OnSceneGUI()
        {
            var trg = (GeometryVisualizer) target;
            trg.DrawHandles();
        }
    }

}