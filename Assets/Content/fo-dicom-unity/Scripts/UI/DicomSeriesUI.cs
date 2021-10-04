using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.UI
{
    using Dicom.Unity.Rendering;

    public class DicomSeriesUI : MonoBehaviour
    {
        public DicomStackRenderer stackRenderer;

        private string defaultTestData => Application.streamingAssetsPath + @"\Test Data\Foot DICOM Volume";

        private void Start()
        {
            var study = DicomStudy.Load(defaultTestData);
            foreach (var series in study.series)
            {
                stackRenderer.Render(series.Value);
                break;
            }
        }
    }
}