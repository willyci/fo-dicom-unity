using System;

namespace Dicom.Unity.IO
{
    public static class DicomSorter
    {
        /// <summary>
        /// Automatically detect and apply the appropriate sort.
        /// </summary>
        /// <param name="files"></param>
        public static void Sort (DicomFile[] files)
        {
            if (files.ContainsTag(DicomTag.InstanceNumber))
                SortByInstanceNumber(files);
        }

        /// <summary>
        /// Extension for DicomFile[] to verify that all files contain a tag.
        /// </summary>
        public static bool ContainsTag (this DicomFile[] files, DicomTag tag)
        {
            bool allFilesContainTag = true;

            foreach (var file in files)
            {
                bool containsTag = file.Dataset.TryGetValue(tag, 0, out int value);
                if (!containsTag)
                {
                    allFilesContainTag = false;
                    break;
                }
            }

            return allFilesContainTag;
        }

        /// <summary>
        /// Recommended.
        /// </summary>
        public static void SortByInstanceNumber(DicomFile[] files)
        {
            Array.Sort(files, (DicomFile a, DicomFile b) =>
            {
                return a.Dataset.GetValue<int>(DicomTag.InstanceNumber, 0)
                .CompareTo(b.Dataset.GetValue<int>(DicomTag.InstanceNumber, 0));
            });
        }
    }
}