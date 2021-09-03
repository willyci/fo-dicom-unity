using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomVolumeDemo : MonoBehaviour
    {
        public DicomStudy study;
        public new DicomVolumeRenderer renderer;

        public string dicomDirPath;

        private void Start()
        {
            study.LoadStudy(dicomDirPath);

            DicomSeries activeSeries = null;

            int seriesLength = -1;
            foreach (var series in study.series)
            {
                if (series.Value.dicomFiles.Length > seriesLength)
                    activeSeries = series.Value;
            }

            renderer.Render(activeSeries);
        }
    }
}