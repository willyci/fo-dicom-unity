using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.UI
{
    public class SeriesManager : MonoBehaviour
    {
        public StudyManager studyManager;
        public SeriesSelectable prefabSelectable;

        private void OnEnable() { studyManager.OnStudyLoaded += OnStudyLoaded; }
        private void OnDisable() { studyManager.OnStudyLoaded -= OnStudyLoaded; }

        private void OnStudyLoaded(object sender, DicomStudy study)
        {
            foreach (var series in study.series)
                Instantiate(prefabSelectable, transform);
        }
    }
}