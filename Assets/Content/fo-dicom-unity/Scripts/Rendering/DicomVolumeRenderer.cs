using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    using Rendering.Data;
    using UnityVolume.Rendering;

    /// <summary>
    /// Renders a DICOM volume into the game world.
    /// </summary>

    public class DicomVolumeRenderer : VolumeRenderer
    {
        public void Render(DicomSeries series)
        {
            Render(DicomVolumeData.Extract(series.dicomFiles));
        }
        
        public void Render (DicomVolumeData volumeData)
        {
            Color[] colors = HoundsfieldScale.ValuesToColors(volumeData.houndsfieldValues);

            Texture3D volumeTexture = new Texture3D(
                volumeData.voxelCount.x,
                volumeData.voxelCount.y,
                volumeData.voxelCount.z,
                TextureFormat.RFloat,
                false);

            volumeTexture.SetPixels(colors);
            volumeTexture.Apply();

            base.Render(volumeTexture, volumeData.physicalSize);
            
            float min = (float)volumeData.houndsfieldValues.Min();
            float max = (float)volumeData.houndsfieldValues.Max();
            
            SetWindow(min, max);
            SetCutoff(min, max);
        }
    }
}