using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    using Rendering.Data;
    using Rendering.Volume;

    public class DicomVolumeRenderer : DicomRenderer
    {
        public enum VolumeRenderMode { Direct, Surface };
        
        public VolumeRenderMode RenderMode
        {
            get { return renderMode; }
            set { SetRenderMode(value); }
        }
        private VolumeRenderMode renderMode = VolumeRenderMode.Direct;

        public TransferFunction transferFunction;

        public void SetRenderMode (VolumeRenderMode renderMode)
        {
            Material material = renderer.material;

            switch (renderMode)
            {
                case VolumeRenderMode.Direct:
                    material.EnableKeyword("MODE_DVR");
                    material.DisableKeyword("MODE_SURF");
                    break;
                case VolumeRenderMode.Surface:
                    material.DisableKeyword("MODE_DVR");
                    material.EnableKeyword("MODE_SURF");
                    break;
            }

            this.renderMode = renderMode;
        }

        public override void Render(DicomSeries series)
        {
            Render(DicomVolumeData.Extract(series.dicomFiles));
        }
        
        public void Render (DicomVolumeData volumeData)
        {
            Texture3D volumeTexture = ConvertDataToTexture(volumeData);
            // calculate gradient texture
            Texture2D noiseTexture = Noise.GenerateMonochromatic2D(512, 512);
            Texture2D transferTexture = transferFunction.GenerateTexture();

            Material material = renderer.material;

            material.SetTexture("_DataTex", volumeTexture);
            //material.SetTexture("_GradientTex", renderData.gradient); TODO
            material.SetTexture("_NoiseTex", noiseTexture);
            material.SetTexture("_TFTex", transferTexture);

            SetRenderMode(renderMode);

            //transform.localScale = volumeData.physicalSize;

            float min = (float)volumeData.houndsfieldValues.Min();
            float max = (float)volumeData.houndsfieldValues.Max();
            SetWindow(min, max);
        }

        private Texture3D ConvertDataToTexture (DicomVolumeData volumeData)
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

            return volumeTexture;
        }
    }
}