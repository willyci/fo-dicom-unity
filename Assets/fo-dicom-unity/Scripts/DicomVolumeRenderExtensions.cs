using UnityEngine;

namespace Dicom.Unity
{
    public static class DicomVolumeRenderExtensions
    {
        public static Texture3D ToTexture3D (this DicomFile[] dicomFiles)
        {
            return ToTexture3D(DicomVolumeData.Extract(dicomFiles));
        }

        public static Texture3D ToTexture3D (this DicomVolumeData renderData)
        {
            float[] normalisedValues = DicomRenderingUtilities.NormaliseValues(renderData.houndsfieldValues);
            Color[] grayscaleColors = DicomRenderingUtilities.ConvertValuesToGrayscale(normalisedValues);

            Texture3D volumeTexture = new Texture3D(renderData.voxelCount.x, renderData.voxelCount.y, renderData.voxelCount.z, TextureFormat.RFloat, false);
            volumeTexture.SetPixels(grayscaleColors);
            volumeTexture.Apply();

            return volumeTexture;
        }
    }
}