using System.Collections.Generic;
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
        /// <summary>
        /// Returns a DicomFile representation of a DICOM file.
        /// </summary>
        
        /// <remarks>
        /// Simple wrapper around DicomFile.Open for consistency 
        /// with the more complex ReadDirectory functionality.
        /// </remarks>
        public static DicomFile ReadFile (string filePath)
        {
            return DicomFile.Open(filePath);
        }

        /// <summary>
        /// Returns an ordered array of DicomFiles representing a DICOM series.
        /// </summary>
        public static DicomFile[] ReadDirectory (string directoryPath)
        {
            IEnumerable<string> filePaths = GetDicomFilePathsInDirectory(directoryPath);
            DicomFile[] dicomFiles = ReadDicomFiles(filePaths);
            return dicomFiles;
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
    }
}