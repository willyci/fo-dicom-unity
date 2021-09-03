using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using Dicom;
using Dicom.Unity;

public class SeriesSplitter : MonoBehaviour
{
    public string rootDir;

    private void Start()
    {
        var filePaths = GetDicomFilePathsInDirectory(rootDir);
        foreach (var file in filePaths)
        {
            DicomFile dicom = DicomFile.Open(file);
            Debug.Log(dicom.File.Directory);
            Debug.Log(dicom.File.Name);
            break;

            /*
            int series = dicom.Dataset.GetValue<int>(DicomTag.SeriesNumber, 0);

            if (!Directory.Exists(rootDir + @"\" + series))
                Directory.CreateDirectory(rootDir + @"\" + series);

            string fileName = Path.GetFileName(file);
            string destination = rootDir + @"\" + series + @"\" + fileName;
            File.Copy(file, destination);
            */
        }
    }

    private static IEnumerable<string> GetDicomFilePathsInDirectory(string directoryPath)
    {
        // Gets all files in the directory that end with one of the specified DICOM file types
        IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(p => p.EndsWith(".dcm") || p.EndsWith(".dicom") || p.EndsWith(".dicm"));

        if (filePaths.Count() == 0)
            throw new System.IndexOutOfRangeException("Directory contains no files with a recognised DICOM extension (.dcm, .dicom, .dicm).");

        return filePaths;
    }

    private static DicomFile[] ReadDicomFiles(IEnumerable<string> filePaths)
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
