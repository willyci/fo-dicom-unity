using UnityEngine;

namespace Dicom.Unity.Behaviours
{
    /// <summary>
    /// Renders a DICOM slice through Unity's default rendering pipeline.
    /// </summary>
    public class DicomSliceRenderBehaviour : DicomRenderBehaviourBase
    {
        public void Render (DicomSliceRenderData sliceData)
        {
            Render(sliceData.ToTexture2D());
        }

        public void Render(Texture2D sliceTexture)
        {
            meshRenderer.material.mainTexture = sliceTexture;
        }
    }
}