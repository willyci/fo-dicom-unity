using System.Threading.Tasks;

namespace Dicom.Unity.Factories
{
    public static class DicomStackRenderDataFactory
    {
        public static DicomStackData CreateStackData (DicomFile[] dicomFiles)
        {
            DicomSliceData[] sliceData = new DicomSliceData[dicomFiles.Length];
            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i] = DicomSliceRenderDataFactory.CreateSliceData(dicomFiles[i]);
            });

            return new DicomStackData
            {
                sliceData = sliceData
            };
        }
    }
}