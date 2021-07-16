using UnityEngine;

namespace Dicom.Unity
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class DicomRenderBehaviourBase : MonoBehaviour
    {
        protected MeshFilter meshFilter => GetComponent<MeshFilter>();
        protected MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    }
}