using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.IO;
    using Dicom.Unity.Rendering.Components;

    public class DicomStackDemo : MonoBehaviour
    {
        public DicomStudy study;
        public DicomStackRenderer stackRenderer;

        public string dicomDirPath;

        private void Start()
        {
            study.LoadStudy(dicomDirPath);

            foreach (var series in study.series)
                stackRenderer.Render(series.Value);
        }
    }
}