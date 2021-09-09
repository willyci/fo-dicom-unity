using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomVolumeDemo : MonoBehaviour
    {
        public DicomStudy study;
        public DicomVolumeRenderer volumeRenderer;

        private string dicomPath = Application.streamingAssetsPath + @"\Test Data\Foot DICOM Volume";

        private void Start()
        {
            study.LoadStudy(dicomPath);

            foreach (var series in study.series)
                volumeRenderer.Render(series.Value);
        }
    }
}