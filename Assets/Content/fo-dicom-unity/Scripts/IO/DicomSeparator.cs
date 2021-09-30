using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Dicom.Unity.IO
{
    public class DicomSeparator
    {
        public static Dictionary<int, DicomSeries> SeparateIntoSeries(DicomFile[] dicomFiles)
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
                DicomSorter.SortByInstanceNumber(sortedArray);

                var series = new DicomSeries(bag.Key, sortedArray);
                seriesDictionary.Add(series.seriesNumber, series);
            }

            return seriesDictionary;
        }
    }
}