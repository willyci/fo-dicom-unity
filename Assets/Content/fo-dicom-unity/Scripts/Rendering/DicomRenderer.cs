using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Dicom.Unity.Rendering
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

        protected Vector3 ConvertPhysicalSizeToNormalisedScale (Vector2 physicalSize)
        {
            throw new System.NotImplementedException();
        }

        protected Vector3 ConvertPhysicalSizeToNormalisedScale (Vector3 physicalSize)
        {
            throw new System.NotImplementedException();
        }
    }
}