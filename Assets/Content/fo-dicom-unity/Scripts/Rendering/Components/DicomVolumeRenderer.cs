using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Rendering.Components
{
    using Rendering.Data;

    public class DicomVolumeRenderer : DicomRenderer
    {
        public override void Render(DicomSeries series)
        {
            Render(DicomVolumeData.Extract(series.dicomFiles));
        }
        
        public void Render (DicomVolumeData volumeData)
        {
            throw new System.NotImplementedException();
        }
    }
}