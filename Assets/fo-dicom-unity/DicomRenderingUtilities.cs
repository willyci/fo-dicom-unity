using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Dicom.Unity
{
    public static class DicomRenderingUtilities
    {
        public static float[] NormaliseValues(double[] values)
        {
            float min = (float)values.Min();
            float max = (float)values.Max();
            return NormaliseValues(values, min, max);
        }

        public static float[] NormaliseValues(double[] values, float min, float max)
        {
            float[] rescaledValues = new float[values.Length];

            Parallel.For(0, rescaledValues.Length, i =>
            {
                float value = (float)values[i];
                value = (value - min) / (max - min);
                value = Mathf.Clamp(value, 0f, 1f);
                rescaledValues[i] = value;
            });

            return rescaledValues;
        }

        public static Color[] ConvertValuesToGrayscale(bool[] values)
        {
            Color[] colors = new Color[values.Length];
            Parallel.For(0, colors.Length, i =>
            {
                colors[i] = values[i] ? Color.white : Color.black;
            });
            return colors;
        }

        public static Color[] ConvertValuesToGrayscale(float[] values)
        {
            Color[] colors = new Color[values.Length];
            Parallel.For(0, colors.Length, i =>
            {
                colors[i] = new Color(values[i], values[i], values[i]);
            });
            return colors;
        }
    }
}