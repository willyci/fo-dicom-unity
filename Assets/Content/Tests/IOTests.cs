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
        private static string testDataDirectory = @"C:\Development\Reference\DICOM Scans\Multiple Series";

        [Test]
        public void ShouldLoadAllDataIntoMemoryWithoutError()
        {
            DicomStudy.Load(testDataDirectory);
            Assert.True(true);
        }
    }
}