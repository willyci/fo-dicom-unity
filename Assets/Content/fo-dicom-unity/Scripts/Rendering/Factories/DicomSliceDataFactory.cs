using Dicom.Imaging;
using Dicom.Imaging.Render;
using UnityEngine;

namespace Dicom.Unity.Rendering.Factories
{
    using Rendering.Data;

    public static class DicomSliceDataFactory
    {
        public static DicomSliceData CreateSliceData (DicomFile dicomFile)
        {
            Vector2Int pixelCount = GetPixelCount(dicomFile);
            Vector2 physicalSize = GetPhysicalSize(dicomFile, pixelCount);
            double[] values = GetValues(dicomFile, pixelCount);

            return new DicomSliceData
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

            bool containsPixelSpacing = dicomFile.Dataset.Contains(DicomTag.PixelSpacing);
            
            string[] pixelSpacing = containsPixelSpacing ? dicomFile.Dataset.GetValue<string>(DicomTag.PixelSpacing, 0).Split('\\') : new string[] { "1.00", "1.00" };

            float rowSpacing = ParseSpacing(pixelSpacing[0]);
            float columnSpacing = pixelSpacing.Length > 1 ? ParseSpacing(pixelSpacing[0]) : rowSpacing;

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

            bool containsRescaleSlopeTag = dicomFile.Dataset.Contains(DicomTag.RescaleSlope);
            bool containsInterceptTab = dicomFile.Dataset.Contains(DicomTag.RescaleIntercept);

            double slope = containsRescaleSlopeTag ? dicomFile.Dataset.GetValue<double>(DicomTag.RescaleSlope, 0) : 1;
            double intercept = containsInterceptTab ? dicomFile.Dataset.GetValue<double>(DicomTag.RescaleIntercept, 0) : -1024;

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

        private static float ParseSpacing (string input)
        {
            bool decimalSuccess = decimal.TryParse(input, out decimal decimalValue);
            if (decimalSuccess) 
                return (float)decimalValue;

            bool floatSuccess = float.TryParse(input, out float floatValue);
            if (floatSuccess)
                return floatValue;

            throw new System.ArgumentException("Unparseable string: " + input);
        }
    }
}