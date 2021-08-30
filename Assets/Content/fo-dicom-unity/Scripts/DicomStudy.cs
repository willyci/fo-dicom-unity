using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace Dicom.Unity
{
    public class DicomStudy : MonoBehaviour
    {
        public Dictionary<int, DicomSeries> series;

        /// <summary>
        /// Attempts to load the DICOM study at the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// Returns true if successful, false otherwise.
        /// </returns>
        public bool LoadStudy(string path)
        {
            ResetStudy();
            
            DicomFile[] dicomFiles = ReadStudy(path);

            if (dicomFiles != null)
                series = SeparateStudyIntoSeries(dicomFiles);

            return series != null;
        }

        private void ResetStudy()
        {
            series = null;
        }

        private DicomFile[] ReadStudy (string path)
        {
            DicomFile[] dicomFiles = null;

            try
            {
                if (File.Exists(path))
                    dicomFiles = new DicomFile[] { IO.DicomReader.ReadFile(path) };
                else if (Directory.Exists(path))
                    dicomFiles = IO.DicomReader.ReadDirectory(path);
                else
                    Debug.LogError("DICOM IO Error: No file or path found at " + path);
            }
            catch
            {
                Debug.LogError("Error in reading DICOM study");
            }

            return dicomFiles;
        }

        private Dictionary<int, DicomSeries> SeparateStudyIntoSeries(DicomFile[] dicomFiles)
        {
            // Separate out all the files into different bags based on their series number
            var unsortedBags = new ConcurrentDictionary<int, ConcurrentBag<DicomFile>>();
            Parallel.For(0, dicomFiles.Length, i =>
            {
                int seriesNumber = dicomFiles[i].Dataset.GetValue<int>(DicomTag.SeriesNumber, 0);
                
                if (!unsortedBags.ContainsKey(seriesNumber))
                    unsortedBags.TryAdd(seriesNumber, new ConcurrentBag<DicomFile>());

                unsortedBags[seriesNumber].Add(dicomFiles[i]);
            });

            // Convert each bag to an array sorted by slice location
            var sortedArrays = new ConcurrentDictionary<int, DicomFile[]>();
            Parallel.ForEach(unsortedBags, fileBag =>
            {
                // Convert bag to a list and sort by slice location
                var fileList = fileBag.Value.ToList();
                fileList.Sort((DicomFile a, DicomFile b) => {
                    return a.Dataset.GetValue<decimal>(DicomTag.SliceLocation, 0)
                    .CompareTo(b.Dataset.GetValue<decimal>(DicomTag.SliceLocation, 0));
                });

                // Convert to an array to lock the order
                var fileArray = fileList.ToArray();

                // Add to sorted arrays dictionary
                sortedArrays.TryAdd(fileBag.Key, fileArray);
            });

            // Wrap each pair as a DicomSeries
            var seriesDictionary = new Dictionary<int, DicomSeries>();
            Parallel.ForEach(sortedArrays, sortedArray =>
            {
                var series = new DicomSeries(sortedArray.Key, sortedArray.Value);
                seriesDictionary.Add(series.seriesNumber, series);
            });

            return seriesDictionary;
        }
    }
}