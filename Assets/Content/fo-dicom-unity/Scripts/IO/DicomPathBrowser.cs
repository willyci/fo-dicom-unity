using UnityEngine;

namespace Dicom.Unity.IO
{
    public static class DicomPathBrowser
    {
        public static string OpenFile()
        {
            string[] results = SFB.StandaloneFileBrowser.OpenFilePanel("Open File", Application.dataPath, "", false);

            return results.Length > 0 ? results[0] : null;
        }

        public static string OpenFolder()
        {
            string[] results = SFB.StandaloneFileBrowser.OpenFolderPanel("Open Folder", Application.dataPath, false);

            return results.Length > 0 ? results[0] : null;
        }
    }
}