using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Dicom.Unity.UI
{
    using Dicom.Unity.IO;

    public class LoadDicomUI : MonoBehaviour
    {
        public DicomStudy study;
        public TMP_InputField dicomPath;
        public Button loadButton;

        private void Start()
        {
            loadButton.onClick.AddListener(LoadDicomFromPath);
        }

        private void LoadDicomFromPath()
        {
            string path = dicomPath.text;
        }
    }
}