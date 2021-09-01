using UnityEngine;

namespace Dicom.Unity.Rendering.Volume
{
    public static class Noise
    {
        public static Texture2D GenerateMonochromatic2D(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            Color[] colors = new Color[width * height];
            int i; float value;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    i = x + (y * width);
                    value = Random.Range(0f, 1f);
                    colors[i] = new Color(value, value, value);
                }
            }

            texture.SetPixels(colors);
            texture.Apply();

            return texture;
        }
    }
}