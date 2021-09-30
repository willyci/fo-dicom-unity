using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomStackDemo : MonoBehaviour
    {
        public string dicomPath;
        public DicomStudy study;
        public DicomStackRenderer stackRenderer;

        private string defaultTestData => Application.streamingAssetsPath + @"\Test Data\Foot DICOM Volume";

        private void Start()
        {
            if (dicomPath.Length == 0)
                dicomPath = defaultTestData;

            study = DicomStudy.Load(dicomPath);

            foreach (var series in study.series)
                stackRenderer.Render(series.Value);
        }
    }
}