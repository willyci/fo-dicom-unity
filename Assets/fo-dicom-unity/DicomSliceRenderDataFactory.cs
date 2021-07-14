using Dicom.Imaging;
using Dicom.Imaging.Render;
using UnityEngine;

namespace Dicom.Unity.Factories
{
    public static class DicomSliceRenderDataFactory
    {
        public static DicomSliceRenderData CreateSliceData (DicomFile dicomFile)
        {
            Vector2Int pixelCount = GetPixelCount(dicomFile);
            Vector2 physicalSize = GetPhysicalSize(dicomFile, pixelCount);
            double[] values = GetValues(dicomFile, pixelCount);

            return new DicomSliceRenderData
            {
                pixelCount = pixelCount,
                physicalSize = physicalSize,
                houndsfieldValues = values
            };
        }

        private static Vector2Int GetPixelCount (DicomFile dicomFile)
        {
            DicomImage image = new DicomImage(dicomFile.Dataset);

            return new Vector2Int
            {
                x = image.Width,
                y = image.Height
            };
        }

        private static Vector2 GetPhysicalSize (DicomFile dicomFile, Vector2Int pixelCount)
        {
            // Data format: ==> {Pixel Spacing Value} = {Row Spacing Value} \ {Column Spacing Value}
            // [0]: Row Spacing Value
            // [1]: Column Spacing Value
            string[] pixelSpacing = dicomFile.Dataset.GetValue<string>(DicomTag.PixelSpacing, 0).Split('\\');

            decimal rowSpacing = decimal.Parse(pixelSpacing[0]);
            decimal columnSpacing = pixelSpacing.Length > 1 ? decimal.Parse(pixelSpacing[1]) : decimal.Parse(pixelSpacing[0]);

            return new Vector2()
            {
                x = Mathf.Abs((float)(rowSpacing * pixelCount.x)),
                y = Mathf.Abs((float)(columnSpacing * pixelCount.y)),
            };
        }

        private static double[] GetValues (DicomFile dicomFile, Vector2Int pixelCount)
        {
            double[] houndsfieldValues = new double[pixelCount.x * pixelCount.y];

            DicomPixelData header = DicomPixelData.Create(dicomFile.Dataset);
            IPixelData pixelData = PixelDataFactory.Create(header, 0);

            double slope = dicomFile.Dataset.GetValue<double>(DicomTag.RescaleSlope, 0);
            double intercept = dicomFile.Dataset.GetValue<double>(DicomTag.RescaleIntercept, 0);

            for (int x = 0; x < pixelData.Width; x++)
            {
                for (int yDicom = 0; yDicom < pixelData.Height; yDicom++)
                {
                    // DICOM is ordered top to bottom but Unity is ordered bottom to top,
                    // so we need to flip the y-index so the data is later processed right way up
                    int yUnity = pixelCount.y - yDicom - 1;
                    int index = x + (yUnity * pixelCount.x);

                    // HoundsfieldValue = (RescaleSlope * PixelData) + RescaleIntercept
                    houndsfieldValues[index] = (slope * pixelData.GetPixel(x, yDicom)) + intercept;
                }
            }

            return houndsfieldValues;
        }
    }
}