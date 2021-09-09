using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    using Rendering.Data;
    using UnityVolume.Rendering;

    /// <summary>
    /// Renders a DICOM slice into the game world.
    /// </summary>

    public class DicomSliceRenderer : VolumeRenderer
    {
        public void Render(DicomSeries series)
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

            Vector3 size = new Vector3()
            {
                x = sliceData.physicalSize.x,
                y = sliceData.physicalSize.y,
                z = 1f
            };

            base.Render(sliceTexture, size);

            float min = (float)sliceData.houndsfieldValues.Min();
            float max = (float)sliceData.houndsfieldValues.Max();
            
            SetWindow(min, max);
            SetCutoff(min, max);
        }
    }
}