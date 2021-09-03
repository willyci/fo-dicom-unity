using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.IO;
    using Dicom.Unity.Rendering;

    public class DicomStackDemo : MonoBehaviour
    {
        public DicomStudy study;
        public new DicomRenderer renderer;

        public string dicomPath;

        private void Start()
        {
            study.LoadStudy(dicomPath);

            foreach (var series in study.series)
                renderer.Render(series.Value);
        }
    }
}