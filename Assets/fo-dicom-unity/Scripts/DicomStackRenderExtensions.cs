using System.Threading.Tasks;
using UnityEngine;

namespace Dicom.Unity
{
    public static class DicomStackRenderExtensions
    {
        public static Texture2D[] ToTexture2DArray (this DicomFile[] dicomFiles)
        {
            return ToTexture2DArray(DicomStackRenderData.Extract(dicomFiles));
        }

        public static Texture2D[] ToTexture2DArray(this DicomStackRenderData stackData)
        {
            Texture2D[] stackTextures = new Texture2D[stackData.sliceData.Length];
            
            Parallel.For(0, stackTextures.Length, i =>
            {
                stackTextures[i] = stackData.sliceData[i].ToTexture2D();
            });
            
            return stackTextures;
        }
    }
}