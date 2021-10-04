using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dicom.Unity.UI
{
    using Dicom.Unity.IO;

    public class StudyPathInput : MonoBehaviour
    {
        public StudyManager manager;
        
        public Button loadFile;
        public Button loadFolder;

        private void OnEnable() 
        { 
            loadFile.onClick.AddListener(LoadFile); 
            loadFolder.onClick.AddListener(LoadFolder); 
        }

        private void OnDisable() 
        {
            loadFile.onClick.RemoveListener(LoadFile);
            loadFolder.onClick.RemoveListener(LoadFolder);
        }

        private void LoadFile()
        {
            string path = DicomPathBrowser.OpenFile();

            if (path != null)
                manager.LoadStudy(path);
        }

        private void LoadFolder()
        {
            string path = DicomPathBrowser.OpenFolder();

            if (path != null)
                manager.LoadStudy(path);
        }
    }
}