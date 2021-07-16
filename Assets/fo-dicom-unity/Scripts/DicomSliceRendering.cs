using UnityEngine;

namespace Dicom.Unity
{
    public static class DicomSliceRendering
    {
        public static Texture2D ToTexture2D (this DicomFile dicomFile)
        {
            return ToTexture2D(DicomSliceRenderData.Extract(dicomFile));
        }

        public static Texture2D ToTexture2D (this DicomSliceRenderData renderData)
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