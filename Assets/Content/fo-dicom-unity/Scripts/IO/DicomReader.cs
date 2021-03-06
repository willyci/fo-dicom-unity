using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dicom.Unity.IO
{
    /// <summary>
    /// Loads DICOM files synchronously from disk into memory in fo-dicom's DicomFile format.
    /// </summary>
    public static class DicomReader
    {
        public static DicomStudy Read (string path)
        {
            DicomStudy study = null;

            if (File.Exists(path))
                study = ReadFile(path);
            else if (Directory.Exists(path))
                study = ReadDirectory(path);

            return study;
        }

        /// <summary>
        /// Returns a study consisting of just one series containing the one file.
        /// </summary>
        public static DicomStudy ReadFile (string filePath)
        {
            DicomFile dicomFile = DicomFile.Open(filePath);
            int seriesNumber = dicomFile.Dataset.GetValue<int>(DicomTag.SeriesNumber, 0);

            return new DicomStudy(new Dictionary<int, DicomSeries>()
            {
                { seriesNumber, new DicomSeries(seriesNumber, new DicomFile[]{ dicomFile }) }
            });
        }

        /// <summary>
        /// Returns an ordered array of DicomFiles representing a DICOM series.
        /// </summary>
        public static DicomStudy ReadDirectory (string directoryPath)
        {
            IEnumerable<string> filePaths = GetDicomFilePathsInDirectory(directoryPath);
            DicomFile[] dicomFiles = ReadDicomFiles(filePaths);
            Dictionary<int, DicomSeries> dicomSeries = SeparateIntoSeries(dicomFiles);

            return new DicomStudy(dicomSeries);
        }

        private static IEnumerable<string> GetDicomFilePathsInDirectory (string directoryPath)
        {
            // Gets all files in the directory that end with one of the specified DICOM file types
            IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(p => p.EndsWith(".dcm") || p.EndsWith(".dicom") || p.EndsWith(".dicm"));

            if (filePaths.Count() == 0)
                throw new System.IndexOutOfRangeException("Directory contains no files with a recognised DICOM extension (.dcm, .dicom, .dicm).");

            return filePaths;
        }

        private static DicomFile[] ReadDicomFiles (IEnumerable<string> filePaths)
        {
            // Initialise an empty array to allow for easy parallel access
            DicomFile[] fileArray = new DicomFile[filePaths.Count()];

            // Read the files in parallel
            Parallel.For(0, fileArray.Length, i =>
            {
                fileArray[i] = DicomFile.Open(filePaths.ElementAt(i));
            });

            return fileArray;
        }

        private static Dictionary<int, DicomSeries> SeparateIntoSeries(DicomFile[] dicomFiles)
        {
            // Separate out all the files into different bags based on their series number
            var seriesBags = new ConcurrentDictionary<int, ConcurrentBag<DicomFile>>();
            Parallel.For(0, dicomFiles.Length, i =>
            {
                int seriesNumber = dicomFiles[i].Dataset.GetValue<int>(DicomTag.SeriesNumber, 0);

                if (!seriesBags.ContainsKey(seriesNumber))
                    seriesBags.TryAdd(seriesNumber, new ConcurrentBag<DicomFile>());

                seriesBags[seriesNumber].Add(dicomFiles[i]);
            });

            // Sort and wrap each pair as a DicomSeries
            var seriesDictionary = new Dictionary<int, DicomSeries>();
            foreach (var bag in seriesBags)
            {
                List<DicomFile> sortedFiles = new List<DicomFile>(bag.Value.Count);
                foreach (var file in bag.Value)
                    sortedFiles.Add(file);

                DicomFile[] sortedArray = sortedFiles.ToArray();
                DicomSorter.Sort(sortedArray);

                var series = new DicomSeries(bag.Key, sortedArray);
                seriesDictionary.Add(series.seriesNumber, series);
            }

            return seriesDictionary;
        }
    }
}