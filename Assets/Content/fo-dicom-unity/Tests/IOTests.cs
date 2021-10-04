using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Dicom.Unity.IO.Tests
{
    public class IOTests
    {
        private const string testDataDirectory = @"C:\Development\Reference\DICOM Scans\Multiple Series";
        private const int expectedSeriesCount = 13;

        [Test]
        public void ShouldLoadAllDataIntoMemoryWithoutError()
        {
            DicomStudy.Load(testDataDirectory);
            Assert.True(true);
        }

        [Test]
        public void ShouldSeparateStudyIntoExpectedNumberOfSeries()
        {
            var study = DicomStudy.Load(testDataDirectory);
            Assert.AreEqual(expectedSeriesCount, study.series.Count);
        }
    }
}