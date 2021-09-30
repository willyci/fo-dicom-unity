using System.Threading.Tasks;
using UnityEngine;

namespace Dicom.Unity
{
    public static class HoundsfieldScale
    {
        [System.Serializable]
        public struct Range
        {
            public double min;
            public double max;

            public Range(double min, double max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public static Range air = new Range(-10000, -800);
        public static Range lung = new Range(-800, -200);

        public static Color[] ValuesToColors (double[] values)
        {
            Color[] colors = new Color[values.Length];
            Parallel.For(0, colors.Length, i =>
            {
                float value = (float)values[i];
                colors[i] = new Color(value, value, value);
            });
            return colors;
        }
    }
}