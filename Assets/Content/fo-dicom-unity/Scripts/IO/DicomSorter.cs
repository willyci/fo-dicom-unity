using System;
using System.Collections.Generic;
using System.Linq;

namespace Dicom.Unity.IO
{
    public class DicomSorter
    {
        public static void SortByInstanceNumber(DicomFile[] files)
        {
            Array.Sort(files, (DicomFile a, DicomFile b) =>
            {
                return a.Dataset.GetValue<int>(DicomTag.InstanceNumber, 0)
                .CompareTo(b.Dataset.GetValue<int>(DicomTag.InstanceNumber, 0));
            });
        }

        public static void SortByInstanceNumber (ref List<DicomFile> files)
        {
            files = files.OrderBy(file => file.Dataset.GetValue<int>(DicomTag.InstanceNumber, 0)) as List<DicomFile>;
        }
    }
}