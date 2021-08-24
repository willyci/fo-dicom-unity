using System.Threading.Tasks;

namespace Dicom.Unity.Rendering.Factories
{
    using Rendering.Data;

    public static class DicomStackDataFactory
    {
        public static DicomStackData CreateStackData (DicomFile[] dicomFiles)
        {
            DicomSliceData[] sliceData = new DicomSliceData[dicomFiles.Length];
            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i] = DicomSliceDataFactory.CreateSliceData(dicomFiles[i]);
            });

            return new DicomStackData
            {
                sliceData = sliceData
            };
        }
    }
}