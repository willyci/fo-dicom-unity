namespace Dicom.Unity
{
    /// <summary>
    /// Convenience aggregate of a stack of DicomSliceData slices.
    /// </summary>
    public class DicomStackData
    {
        public DicomSliceData[] sliceData;

        public static DicomStackData Extract (DicomFile[] dicomFiles)
        {
            return Factories.DicomStackDataFactory.CreateStackData(dicomFiles);
        }
    }
}