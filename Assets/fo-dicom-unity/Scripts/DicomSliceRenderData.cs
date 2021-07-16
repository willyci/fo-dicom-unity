using UnityEngine;

namespace Dicom.Unity
{
    /// <summary>
    /// Data required to render a 2D DICOM slice.
    /// </summary>
    public class DicomSliceRenderData
    {
        public Vector2Int pixelCount;
        public Vector2 physicalSize;
        public double[] houndsfieldValues;

        public static DicomSliceRenderData Extract (DicomFile dicomFile)
        {
            return Factories.DicomSliceRenderDataFactory.CreateSliceData(dicomFile);
        }
    }
}