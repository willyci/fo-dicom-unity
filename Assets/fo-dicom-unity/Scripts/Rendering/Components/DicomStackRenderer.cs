using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Rendering.Components
{
    using Rendering.Data;
    using Rendering.Factories;

    public class DicomStackRenderer : DicomRenderer
    {
        public override void Render(DicomSeries series)
        {
            Render(DicomStackDataFactory.CreateStackData(series.dicomFiles));
        }
        
        public void Render (DicomStackData stackData)
        {
            throw new System.NotImplementedException();
        }
    }
}