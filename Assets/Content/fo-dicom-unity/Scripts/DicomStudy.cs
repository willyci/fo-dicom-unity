using System.Collections.Generic;

namespace Dicom.Unity
{
    /// <summary>
    /// The record of an imaging procedure containing one or more series.
    /// </summary>

    public class DicomStudy
    {
        public readonly Dictionary<int, DicomSeries> series;

        public DicomStudy(Dictionary<int, DicomSeries> series)
        {
            this.series = series;
        }

        public static DicomStudy Load (string directoryPath)
        {
            return IO.DicomReader.ReadDirectory(directoryPath);
        }
    }
}