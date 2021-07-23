using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    public class DicomSliceRenderer : DicomRenderer
    {
        public void Render (DicomSliceData sliceData)
        {
            Color[] colors = ConvertValuesToColors(sliceData.houndsfieldValues);

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