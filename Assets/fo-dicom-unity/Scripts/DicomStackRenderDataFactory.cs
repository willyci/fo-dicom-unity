using System.Threading.Tasks;

namespace Dicom.Unity.Factories
{
    public static class DicomStackRenderDataFactory
    {
        public static DicomStackRenderData CreateStackData (DicomFile[] dicomFiles)
        {
            DicomSliceRenderData[] sliceData = new DicomSliceRenderData[dicomFiles.Length];
            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i] = DicomSliceRenderDataFactory.CreateSliceData(dicomFiles[i]);
            });

            return new DicomStackRenderData
            {
                sliceData = sliceData
            };
        }
    }
}