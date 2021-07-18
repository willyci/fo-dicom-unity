using System;
using System.IO;
using UnityEngine;

namespace Dicom.Unity.Behaviours
{
    /// <summary>
    /// Basic Unity MonoBehaviour hook for load and render operations.
    /// </summary>
    public class DicomSliceLoadBehaviour : MonoBehaviour
    {
        public string dicomSliceFilePath;
        public DicomSliceRenderBehaviour sliceRenderer;

        private void Start()
        {
            LoadSlice(dicomSliceFilePath);
        }

        /// <summary>
        /// Loads (and optionally renders) a DICOM file from drive as DicomSliceRenderData.
        /// </summary>
        public DicomSliceRenderData LoadSlice (string dicomSliceFilePath)
        {
            if (!File.Exists(dicomSliceFilePath))
                throw new ArgumentNullException("Specified path is not a file or does not exist");

            DicomFile dicomFile = DicomReader.ReadFile(dicomSliceFilePath);
            DicomSliceRenderData sliceData = DicomSliceRenderData.Extract(dicomFile);
            
            if (sliceRenderer != null)
            {
                Texture2D sliceTexture = sliceData.ToTexture2D();
                sliceRenderer.Render(sliceTexture);
            }

            return sliceData;
        }
    }
}