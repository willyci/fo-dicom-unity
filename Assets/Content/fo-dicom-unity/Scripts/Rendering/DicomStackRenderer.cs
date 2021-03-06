using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    using Rendering.Data;
    using UnityVolume.Rendering;

    /// <summary>
    /// Renders a DICOM stack.
    /// </summary>

    [RequireComponent(typeof(VolumeRenderer))]
    public class DicomStackRenderer : MonoBehaviour
    {
        public VolumeRenderer volumeRenderer { get; private set; }

        public int index { get; private set; }
        public int length { get; private set; }

        private Texture2D[] stack;
        private Vector3 physicalSize;

        private void Awake()
        {
            volumeRenderer = GetComponent<VolumeRenderer>();
        }

        public void Render(DicomSeries series)
        {
            Render(DicomStackData.Extract(series.dicomFiles));
        }
        
        public void Render (DicomStackData stackData)
        {
            length = stackData.sliceData.Length;
            stack = new Texture2D[length];
            physicalSize = new Vector3()
            {
                x = stackData.sliceData[0].physicalSize.x,
                y = stackData.sliceData[0].physicalSize.y,
                z = 1f
            };

            // Convert stack into textures
            for (int i = 0; i < stack.Length; i++)
            {
                Color[] colors = HoundsfieldScale.ValuesToColors(stackData.sliceData[i].houndsfieldValues);

                Texture2D sliceTexture = new Texture2D(
                stackData.sliceData[i].pixelCount.x,
                stackData.sliceData[i].pixelCount.y,
                TextureFormat.RFloat,
                false);

                sliceTexture.SetPixels(colors);
                sliceTexture.Apply();

                stack[i] = sliceTexture;
            }

            // Find the min and max values in the stack
            float min = Mathf.Infinity, max = Mathf.NegativeInfinity;
            foreach(var sliceData in stackData.sliceData)
            {
                float localMin = (float)sliceData.houndsfieldValues.Min();
                float localMax = (float)sliceData.houndsfieldValues.Max();

                if (localMin < min) min = localMin;
                if (localMax > max) max = localMax;
            }


            volumeRenderer.SetWindow(min, max);
            volumeRenderer.SetCutoff(min, max);

            ViewSliceAtIndex(stack.Length / 2);
        }

        public void ViewSliceAtIndex (int index)
        {
            // Clamp index to stack length
            if (index < 0) index = 0;
            if (index >= length) index = length - 1;

            this.index = index;

            volumeRenderer.Render(stack[index], physicalSize);
        }
    }
}