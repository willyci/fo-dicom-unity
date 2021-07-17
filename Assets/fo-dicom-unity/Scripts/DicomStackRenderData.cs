namespace Dicom.Unity
{
    /// <summary>
    /// Convenience aggregate of a stack of DicomSliceRenderData slices.
    /// </summary>
    public class DicomStackRenderData
    {
        public DicomSliceRenderData[] sliceData;

        public static DicomStackRenderData Extract (DicomFile[] dicomFiles)
        {
            return Factories.DicomStackRenderDataFactory.CreateStackData(dicomFiles);
        }
    }
}