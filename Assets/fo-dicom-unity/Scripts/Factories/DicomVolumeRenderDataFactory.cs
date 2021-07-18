using System.Threading.Tasks;
using UnityEngine;

namespace Dicom.Unity.Factories
{
    public static class DicomVolumeRenderDataFactory
    {
        public static DicomVolumeData CreateVolumeData (DicomFile[] dicomFiles)
        {
            DicomSliceData[] sliceData = CreateSliceData(dicomFiles);

            if (SlicesHaveInconsistentDimensions(sliceData))
                Debug.LogError("Warning: Volume slices have inconsistent dimensions");

            Vector3Int voxelCount = GetVoxelCount(sliceData);
            Vector3 physicalSize = GetPhysicalSize(sliceData, dicomFiles);
            double[] houndsfieldValues = ConcatenateSliceValues(sliceData, voxelCount);

            return new DicomVolumeData
            {
                voxelCount = voxelCount,
                physicalSize = physicalSize,
                houndsfieldValues = houndsfieldValues
            };
        }

        private static DicomSliceData[] CreateSliceData (DicomFile[] dicomFiles)
        {
            DicomSliceData[] sliceData = new DicomSliceData[dicomFiles.Length];

            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i] = DicomSliceRenderDataFactory.CreateSliceData(dicomFiles[i]);
            });

            return sliceData;
        }

        private static bool SlicesHaveInconsistentDimensions (DicomSliceData[] sliceData)
        {
            for (int i = 1; i < sliceData.Length; i++)
                if (sliceData[i].pixelCount != sliceData[0].pixelCount)
                    return true;

            return false;
        }

        private static Vector3Int GetVoxelCount (DicomSliceData[] sliceData)
        {
            return new Vector3Int
            {
                x = sliceData[0].pixelCount.x,
                y = sliceData[0].pixelCount.y,
                z = sliceData.Length
            };
        }

        private static Vector3 GetPhysicalSize (DicomSliceData[] sliceData, DicomFile[] dicomFiles)
        {
            decimal spacingBetweenSlices = dicomFiles[0].Dataset.GetValue<decimal>(DicomTag.SpacingBetweenSlices, 0);

            return new Vector3
            {
                x = sliceData[0].physicalSize.x,
                y = sliceData[0].physicalSize.y,
                z = Mathf.Abs((float)(spacingBetweenSlices * dicomFiles.Length))
            };
        }

        private static double[] ConcatenateSliceValues (DicomSliceData[] sliceData, Vector3Int voxelCount)
        {
            double[] houndsfieldValues = new double[voxelCount.x * voxelCount.y * voxelCount.z];

            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i].houndsfieldValues.CopyTo(houndsfieldValues, i * voxelCount.x * voxelCount.y);
            });

            return houndsfieldValues;
        }
    }
}