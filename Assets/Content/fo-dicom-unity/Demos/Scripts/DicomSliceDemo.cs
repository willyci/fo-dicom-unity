using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomSliceDemo : MonoBehaviour
    {
        public DicomStudy study;
        public DicomSliceRenderer sliceRenderer;

        public string dicomPath;

        private void Start()
        {
            study.LoadStudy(dicomPath);

            foreach (var series in study.series)
                sliceRenderer.Render(series.Value);
        }
    }
}