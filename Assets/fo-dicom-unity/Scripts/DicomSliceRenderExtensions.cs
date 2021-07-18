using UnityEngine;

namespace Dicom.Unity
{
    public static class DicomSliceRenderExtensions
    {
        public static Texture2D ToTexture2D (this DicomFile dicomFile)
        {
            return ToTexture2D(DicomSliceData.Extract(dicomFile));
        }

        public static Texture2D ToTexture2D (this DicomSliceData renderData)
        {
            float[] normalisedValues = DicomRenderingUtilities.NormaliseValues(renderData.houndsfieldValues);
            Color[] grayscaleColors = DicomRenderingUtilities.ConvertValuesToGrayscale(normalisedValues);

            Texture2D sliceTexture = new Texture2D(renderData.pixelCount.x, renderData.pixelCount.y);
            sliceTexture.SetPixels(grayscaleColors);
            sliceTexture.Apply();

            return sliceTexture;
        }
    }
}