using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dicom.Unity.Rendering.Factories
{
    using Rendering.Data;

    public static class DicomStackDataFactory
    {
        public static DicomStackData CreateStackData (DicomFile[] dicomFiles)
        {
            DicomFile[] orderedDicomFiles = GetOrderedDicomImages(dicomFiles);

            DicomSliceData[] sliceData = new DicomSliceData[orderedDicomFiles.Length];
         
            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i] = DicomSliceDataFactory.CreateSliceData(orderedDicomFiles[i]);
            });

            return new DicomStackData
            {
                sliceData = sliceData
            };
        }

        private static DicomFile[] GetOrderedDicomImages (DicomFile[] dicomFiles)
        {
            DicomFile[] imageBearingFiles = GetImageBearingDicomFiles(dicomFiles);
            
            if (DatasetsAllContainsImagePositionTags(imageBearingFiles))
            {
                // Sort by single-axis slice location
                Array.Sort(imageBearingFiles, (DicomFile a, DicomFile b) =>
                {
                    // TODO: A comparer for XYZ decimal strings

                    return a.Dataset.GetValue<float>(DicomTag.ImagePositionPatient, 0)
                    .CompareTo(b.Dataset.GetValue<float>(DicomTag.ImagePositionPatient, 0));
                });
            }
            else if (DatasetsAllContainSliceLocationTags(imageBearingFiles))
            {
                // Sort by slice location
                Array.Sort(imageBearingFiles, (DicomFile a, DicomFile b) =>
                {
                    return a.Dataset.GetValue<decimal>(DicomTag.SliceLocation, 0)
                    .CompareTo(b.Dataset.GetValue<decimal>(DicomTag.SliceLocation, 0));
                });
            }
            else
            {
                // Sort alphabetically by file name
                Array.Sort(imageBearingFiles, (DicomFile a, DicomFile b) =>
                {
                    return a.File.Name.CompareTo(b.File.Name);
                });
            }

            return imageBearingFiles;
        }

        private static DicomFile[] GetImageBearingDicomFiles (DicomFile[] dicomFiles)
        {
            // DICOM series can sometimes start with a header file containing no image data
            // This file, containing only text, will usually only be a few kB in size,
            // compared to the hundreds of kB for a file containing image data.
            // Filtering out 

            long minimumSize = 25000; // 25kB - large enough to include text, small enough to exclude images
            
            var imageFiles = new List<DicomFile>();
            foreach (var file in dicomFiles)
                if (new FileInfo(file.File.Name).Length > minimumSize)
                    imageFiles.Add(file);

            return imageFiles.ToArray();
        }

        private static bool DatasetsAllContainsImagePositionTags(DicomFile[] dicomFiles)
        {
            bool containsImagePositionTag = true;

            foreach (var file in dicomFiles)
            {
                if (!file.Dataset.Contains(DicomTag.ImagePositionPatient))
                {
                    containsImagePositionTag = false;
                    break;
                }
            }

            return containsImagePositionTag;
        }

        private static bool DatasetsAllContainSliceLocationTags (DicomFile[] dicomFiles)
        {
            bool containsSlicePositionTag = true;

            foreach (var file in dicomFiles)
            {
                if (!file.Dataset.Contains(DicomTag.SliceLocation))
                {
                    containsSlicePositionTag = false;
                    break;
                }
            }

            return containsSlicePositionTag;
        }
    }
}