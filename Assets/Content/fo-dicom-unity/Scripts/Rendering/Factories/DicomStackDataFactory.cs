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
            DicomFile[] imageBearingFiles = GetImageBearingDicomFiles(dicomFiles);

            DicomSliceData[] sliceData = new DicomSliceData[imageBearingFiles.Length];
         
            Parallel.For(0, sliceData.Length, i =>
            {
                sliceData[i] = DicomSliceDataFactory.CreateSliceData(imageBearingFiles[i]);
            });

            return new DicomStackData
            {
                sliceData = sliceData
            };
        }

        private static DicomFile[] GetImageBearingDicomFiles (DicomFile[] dicomFiles)
        {
            // DICOM series can sometimes start with a header file containing no image data
            // This file, containing only text, will usually only be a few kB in size,
            // compared to the hundreds of kB for a file containing image data.
            
            // This function filterings out these header files and returns only those files bearing images.

            long minimumSize = 25000; // 25kB - large enough to include text, small enough to exclude images
            
            var imageFiles = new List<DicomFile>();
            foreach (var file in dicomFiles)
                if (new FileInfo(file.File.Name).Length > minimumSize)
                    imageFiles.Add(file);

            return imageFiles.ToArray();
        }
    }
}