using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Dicom.Unity.UI
{
    using Dicom.Unity.Rendering;

    public class SeriesSelectable : MonoBehaviour
    {
        public DicomSliceRenderer sliceRenderer;

        [SerializeField] private TextMeshProUGUI idLabel;
        [SerializeField] private TextMeshProUGUI countLabel;

        public DicomSeries series { get; private set; }

        private void OnEnable()
        {
            if (series != null)
                sliceRenderer.Render(series);
        }

        public void Inject (Material materialInstance, DicomSeries series)
        {
            sliceRenderer.GetComponent<RawImage>().material = materialInstance;

            sliceRenderer.Render(series);
            this.series = series;

            idLabel.text = "ID: " + series.seriesNumber;
            countLabel.text = series.dicomFiles.Length + " slices";
        }
    }
}