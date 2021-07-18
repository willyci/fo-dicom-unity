using UnityEngine;

namespace Dicom.Unity
{
    /// <summary>
    /// Data associated with a 2D DICOM slice.
    /// </summary>
    public class DicomSliceData
    {
        public Vector2Int pixelCount;
        public Vector2 physicalSize;
        public double[] houndsfieldValues;

        public static DicomSliceData Extract (DicomFile dicomFile)
        {
            return Factories.DicomSliceRenderDataFactory.CreateSliceData(dicomFile);
        }
    }
}