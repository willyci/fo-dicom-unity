using UnityEngine;

namespace Dicom.Unity
{
    /// <summary>
    /// Data associated with a 3D DICOM volume.
    /// </summary>
    public class DicomVolumeData
    {
        public Vector3Int voxelCount;
        public Vector3 physicalSize;
        public double[] houndsfieldValues;

        public static DicomVolumeData Extract (DicomFile[] dicomFiles)
        {
            return Factories.DicomVolumeRenderDataFactory.CreateVolumeData(dicomFiles);
        }
    }
}