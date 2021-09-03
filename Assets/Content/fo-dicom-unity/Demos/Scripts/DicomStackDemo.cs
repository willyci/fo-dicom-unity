using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomStackDemo : MonoBehaviour
    {
        public DicomStudy study;
        public DicomStackRenderer stackRenderer;

        public string dicomPath;

        private void Start()
        {
            study.LoadStudy(dicomPath);

            foreach (var series in study.series)
                stackRenderer.Render(series.Value);
        }
    }
}