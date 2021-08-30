using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Dicom.Unity.Rendering.Components
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public abstract class DicomRenderer : MonoBehaviour
    {
        public abstract void Render(DicomSeries series);

        protected new MeshRenderer renderer => GetComponent<MeshRenderer>();
        protected MeshFilter filter => GetComponent<MeshFilter>();

        public void SetWindow (float min, float max)
        {
            renderer.material.SetFloat("_WindowMin", min);
            renderer.material.SetFloat("_WindowMax", max);
        }

        protected Color[] ConvertValuesToColors(double[] values)
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