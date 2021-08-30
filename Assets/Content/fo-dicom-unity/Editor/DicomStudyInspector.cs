using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dicom.Unity.IO.Editors
{
    [CustomEditor(typeof(DicomStudy))]
    public class DicomStudyInspector : Editor
    {
        private DicomStudy study;
        private string path;

        public override void OnInspectorGUI()
        {
            study = target as DicomStudy;
            
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (!Application.isPlaying)
                PromptUserToEnterPlayMode();
            else
                LoadStudyFromPath();
        }

        private void PromptUserToEnterPlayMode()
        {
            EditorGUILayout.LabelField("Enter play mode to load studies");
        }

        private void LoadStudyFromPath()
        {
            EditorGUILayout.LabelField("Load Study From Path");
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(path);
            if (GUILayout.Button("Load"))
                study.LoadStudy(path);
            EditorGUILayout.EndHorizontal();
        }
    }
}