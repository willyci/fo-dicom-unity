using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering
{
    using Rendering.Data;

    public class DicomStackRenderer : DicomRenderer
    {
        public int index { get; private set; }
        public int length { get; private set; }

        private Texture2D[] stack;

        public override void Render(DicomSeries series)
        {
            Render(DicomStackData.Extract(series.dicomFiles));
        }
        
        public void Render (DicomStackData stackData)
        {
            length = stackData.sliceData.Length;
            stack = new Texture2D[length];

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

            // Set the display variables
            SetWindow(min, max);
            SetStackIndex(0);
        }

        public void SetStackIndex (int index)
        {
            // Clamp index to stack length
            if (index < 0) index = 0;
            if (index >= length) index = length - 1;

            this.index = index;
            renderer.material.mainTexture = stack[this.index];
        }
    }
}