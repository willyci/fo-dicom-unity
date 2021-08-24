using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Rendering.Components
{
    using Rendering.Data;
    using Rendering.Factories;

    public class DicomVolumeRenderer : DicomRenderer
    {
        public override void Render(DicomSeries series)
        {
            Render(DicomVolumeDataFactory.CreateVolumeData(series.dicomFiles));
        }
        
        public void Render (DicomVolumeData volumeData)
        {
            throw new System.NotImplementedException();
        }
    }
}