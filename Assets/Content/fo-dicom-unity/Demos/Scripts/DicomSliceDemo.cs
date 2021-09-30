using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Demos
{
    using Dicom.Unity.Rendering;

    public class DicomSliceDemo : MonoBehaviour
    {
        public string dicomPath;
        public DicomStudy study;
        public DicomSliceRenderer sliceRenderer;

        private string defaultPath = Application.streamingAssetsPath + @"\Test Data\Foot DICOM Slice\1.3.12.2.1107.5.2.41.69581.2020072113464472647863348.dcm";

        private void Start()
        {
            if (dicomPath.Length == 0)
                dicomPath = defaultPath;

            study = DicomStudy.Load(dicomPath);

            foreach (var series in study.series)
                sliceRenderer.Render(series.Value);
        }
    }
}