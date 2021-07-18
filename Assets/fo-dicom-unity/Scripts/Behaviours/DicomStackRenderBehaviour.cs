using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.Behaviours
{
    public class DicomStackRenderBehaviour : DicomRenderBehaviourBase
    {
        public int SliceIndex { 
            get { return sliceIndex; } 
            set { SetSliceIndex(sliceIndex); }
        }

        public int SliceCount
        {
            get { return sliceTextures != null ? sliceTextures.Length : -1; }
        }

        private int sliceIndex = -1;
        private Texture2D[] sliceTextures;

        public void Render (DicomStackRenderData stackData)
        {
            throw new System.NotImplementedException();
        }

        public void Render (Texture2D[] stackTextures)
        {
            throw new System.NotImplementedException();
        }

        private void SetSliceIndex (int sliceIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}