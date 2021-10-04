using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.UI
{
    public class StudyManager : MonoBehaviour
    {
        public EventHandler<DicomStudy> OnStudyLoaded;

        public DicomStudy study { get; private set; }

        public void LoadStudy (string path)
        {
            study = DicomStudy.Load(path);
            OnStudyLoaded?.Invoke(this, study);
        }
    }
}