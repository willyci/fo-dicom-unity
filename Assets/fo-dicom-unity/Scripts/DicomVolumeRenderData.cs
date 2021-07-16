using UnityEngine;

namespace Dicom.Unity
{
    /// <summary>
    /// Data required to render a 3D DICOM volume.
    /// </summary>
    public class DicomVolumeRenderData
    {
        public Vector3Int voxelCount;
        public Vector3 physicalSize;
        public double[] houndsfieldValues;

        public static DicomVolumeRenderData Extract (DicomFile[] dicomFiles)
        {
            return Factories.DicomVolumeRenderDataFactory.CreateVolumeData(dicomFiles);
        }
    }
}