namespace Dicom.Unity
{
    /// <summary>
    /// A related collection of DICOM files.
    /// </summary>
    [System.Serializable]
    public class DicomSeries
    {
        public readonly int seriesNumber;
        public readonly DicomFile[] dicomFiles;

        public DicomSeries(int seriesNumber, DicomFile[] dicomFiles)
        {
            this.seriesNumber = seriesNumber;
            this.dicomFiles = dicomFiles;
        }
    }
}