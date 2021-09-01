using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dicom.Unity.Rendering.Volume
{
    /// <summary>
    /// Maps normalised input scalars in the range (0f, 1f) to RGBA colors.
    /// </summary>

    [System.Serializable]
    public class TransferFunction
    {
        public Texture2D texture { get; private set; }

        public List<ControlPoint> controlPoints = new List<ControlPoint>();

        private const int width = 512;
        private const int height = 1;

        public TransferFunction()
        {
            controlPoints.Add(new ControlPoint(0, new Color(0f, 0f, 0f, 0f)));
            controlPoints.Add(new ControlPoint(1, new Color(1f, 1f, 1f, 1f)));
        }

        /// <summary>
        /// Converts the control points into a one-dimensional texture.
        /// </summary>
        public Texture2D GenerateTexture()
        {
            if (texture == null)
            {
                TextureFormat format = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf) ? TextureFormat.RGBAHalf : TextureFormat.RGBAFloat;
                texture = new Texture2D(width, height, format, false);
            }

            Color[] colors = new Color[width * height];

            controlPoints.Sort((a, b) => (a.value.CompareTo(b.value)));

            // Make sure the upper bound is covered
            if (controlPoints.Count == 0 || controlPoints.Last().value < 1f)
                controlPoints.Add(new ControlPoint(1f, new Color(1f, 1f, 1f, 1f)));

            // Make sure the lower bound is covered
            if (controlPoints[0].value > 0f)
                controlPoints.Insert(0, new ControlPoint(0f, new Color(0f, 0f, 0f, 0f)));

            // Assign the pixel colors
            for (int x = 0; x < width; x++)
            {
                // Normalise the pixel index to the texture width
                float pixelValue = x / (float)(width - 1);

                GetBoundingControlPoints(pixelValue, out ControlPoint lowerControlPoint, out ControlPoint upperControlPoint);

                // Normalise the pixel value to the control point range
                pixelValue = Mathf.Clamp(pixelValue, lowerControlPoint.value, upperControlPoint.value);
                float controlInterpolationValue = (pixelValue - lowerControlPoint.value) / (upperControlPoint.value - lowerControlPoint.value);

                Color color = Color.Lerp(lowerControlPoint.color, upperControlPoint.color, controlInterpolationValue);

                for (int y = 0; y < height; y++)
                    colors[x + y * width] = color;
            }

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colors);
            texture.Apply();

            return texture;
        }

        private void GetBoundingControlPoints(float pixelValue, out ControlPoint lowerControlPoint, out ControlPoint upperControlPoint)
        {
            int currentControl = 0;

            while (PixelValueBelowLowerControlValue(pixelValue, currentControl) && NextControlIndexWithinRange(currentControl))
                currentControl++;

            lowerControlPoint = controlPoints[currentControl];
            upperControlPoint = controlPoints[currentControl + 1];
        }

        private bool PixelValueBelowLowerControlValue(float pixelValue, int currentControl)
        {
            return pixelValue < controlPoints[currentControl].value;
        }

        private bool NextControlIndexWithinRange(int currentControl)
        {
            return (currentControl + 1) < (controlPoints.Count - 1);
        }

        [System.Serializable]
        public struct ControlPoint
        {
            public float value;
            public Color color;

            public ControlPoint(float value, Color color)
            {
                this.value = value;
                this.color = color;
            }
        }
    }
}