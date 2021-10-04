using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    using Rendering.Data;
    using UnityVolume.Rendering;

    /// <summary>
    /// Renders a DICOM volume.
    /// </summary>

    [RequireComponent(typeof(VolumeRenderer))]
    public class DicomVolumeRenderer : MonoBehaviour
    {
        public VolumeRenderer volumeRenderer { get; private set; }

        private void Awake ()
        {
            volumeRenderer = GetComponent<VolumeRenderer>();
        }

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

            volumeRenderer.Render(volumeTexture, volumeData.physicalSize);
            
            float min = (float)volumeData.houndsfieldValues.Min();
            float max = (float)volumeData.houndsfieldValues.Max();

            volumeRenderer.SetWindow(min, max);
            volumeRenderer.SetCutoff(min, max);
        }
    }
}