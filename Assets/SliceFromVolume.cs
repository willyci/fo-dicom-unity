using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Dicom.Unity;
using Dicom.Unity.Rendering;
using Dicom.Unity.Rendering.Data;
using Dicom.Unity.Rendering.Factories;

public class SliceFromVolume : MonoBehaviour
{
    public DicomStudy study;
    public DicomSliceRenderer sliceRenderer;

    public string dicomDirPath;
    public int sliceTarget;
    private int prevSliceTarget = -1;

    private DicomSeries series;
    private DicomVolumeData volumeData;
    private Texture3D texture;
    private Color[] voxels;

    private void Start()
    {
        study.LoadStudy(dicomDirPath);

        foreach (var series in study.series)
        {
            this.series = series.Value;
            break;
        }

        volumeData = DicomVolumeData.Extract(series.dicomFiles);
        texture = ConvertDataToTexture(volumeData);
        voxels = texture.GetPixels();

        float min = (float)volumeData.houndsfieldValues.Min();
        float max = (float)volumeData.houndsfieldValues.Max();
        sliceRenderer.SetWindow(min, max);
    }

    private void Update()
    {
        if (sliceTarget == prevSliceTarget || texture == null)
            return;

        if (sliceTarget >= volumeData.voxelCount.z - 1)
            sliceTarget = volumeData.voxelCount.z - 2;

        int length = volumeData.voxelCount.x * volumeData.voxelCount.y;
        int start = sliceTarget * length;

        Color[] subset = voxels.Skip(start).Take(length).ToArray();
        Texture2D sliceTexture = new Texture2D(volumeData.voxelCount.x, volumeData.voxelCount.y, TextureFormat.RFloat, false);
        sliceTexture.SetPixels(subset);
        sliceTexture.Apply();

        sliceRenderer.GetComponent<MeshRenderer>().material.mainTexture = sliceTexture;
    }

    private Texture3D ConvertDataToTexture(DicomVolumeData volumeData)
    {
        Color[] colors = HoundsfieldScale.ValuesToColors(volumeData.houndsfieldValues);

        Texture3D volumeTexture = new Texture3D(
            volumeData.voxelCount.x,
            volumeData.voxelCount.y,
            volumeData.voxelCount.z,
            TextureFormat.RFloat,
            false);

        volumeTexture.SetPixels(colors);
        volumeTexture.Apply();

        return volumeTexture;
    }
}
