using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dicom.Unity.Rendering.Components.Editors
{
    [CustomEditor(typeof(DicomStackRenderer))]
    public class DicomStackInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var stack = target as DicomStackRenderer;

            if (stack.length > 0)
            {
                int targetIndex = EditorGUILayout.IntSlider(stack.index, 0, stack.length - 1);
                if (targetIndex != stack.index)
                    stack.ViewSliceAtIndex(targetIndex);
            }
        }
    }
}