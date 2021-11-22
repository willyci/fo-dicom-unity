using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dicom.Unity.UI
{
    public class SeriesManager : MonoBehaviour
    {
        public StudyManager studyManager;
        public Transform contents;
        public SeriesSelectable prefabSelectable;
        public Material prefabMaterial;

        private List<SeriesSelectable> activeSeries = new List<SeriesSelectable>();

        private void OnEnable() { studyManager.OnStudyLoaded += OnStudyLoaded; }
        private void OnDisable() { studyManager.OnStudyLoaded -= OnStudyLoaded; }

        private void OnStudyLoaded(object sender, DicomStudy study)
        {
            ClearExistingSeries();
            AddNewSeries(study);
        }
        
        private void ClearExistingSeries()
        {
            for (int i = 0; i < activeSeries.Count; i++)
                Destroy(activeSeries[i].gameObject);

            activeSeries.Clear();
        }

        private void AddNewSeries(DicomStudy study)
        {
            foreach (var series in study.series)
            {
                // Materials need to be manually instanced to work with Unity's Canvas rendering
                Material instancedMaterial = new Material(prefabMaterial);

                SeriesSelectable selectable = Instantiate(prefabSelectable, contents);
                selectable.Inject(instancedMaterial, series.Value);
                activeSeries.Add(selectable);
            }
        }
    }
}