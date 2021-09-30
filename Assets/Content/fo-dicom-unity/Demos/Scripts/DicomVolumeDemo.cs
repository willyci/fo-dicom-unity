using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomVolumeDemo : MonoBehaviour
    {
        public string dicomPath;
        public DicomStudy study;
        public DicomVolumeRenderer volumeRenderer;

        private string defaultPath = Application.streamingAssetsPath + @"\Test Data\Foot DICOM Volume";

        private void Start()
        {
            if (dicomPath.Length == 0)
                dicomPath = defaultPath;

            study = DicomStudy.Load(dicomPath);

            foreach (var series in study.series)
                volumeRenderer.Render(series.Value);
        }
    }
}