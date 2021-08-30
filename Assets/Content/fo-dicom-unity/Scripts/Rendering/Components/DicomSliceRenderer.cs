using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering.Components
{
    using Rendering.Data;

    public class DicomSliceRenderer : DicomRenderer
    {
        public override void Render(DicomSeries series)
        {
            Render(DicomSliceData.Extract(series.dicomFiles[0]));
        }

        public void Render (DicomSliceData sliceData)
        {
            Color[] colors = HoundsfieldScale.ValuesToColors(sliceData.houndsfieldValues);

            Texture2D sliceTexture = new Texture2D(
                sliceData.pixelCount.x,
                sliceData.pixelCount.y,
                TextureFormat.RFloat,
                false);

            sliceTexture.SetPixels(colors);
            sliceTexture.Apply();

            renderer.material.mainTexture = sliceTexture;

            float min = (float)sliceData.houndsfieldValues.Min();
            float max = (float)sliceData.houndsfieldValues.Max();
            SetWindow(min, max);
        }
    }
}