using UnityEngine;

namespace Dicom.Unity.Rendering
{
    public static class DicomMaterials
    {
        public static Material SliceMaterial
        {
            get
            {
                if (sliceMaterial == null)
                    sliceMaterial = Resources.Load<Material>("Dicom Slice Material");

                return sliceMaterial;
            }
            set
            {
                sliceMaterial = value;
            }
        }
        private static Material sliceMaterial;

        public static Material VolumeMaterial
        {
            get
            {
                if (volumeMaterial == null)
                    volumeMaterial = Resources.Load<Material>("Dicom Volume Material");

                return volumeMaterial;
            }
            set
            {
                volumeMaterial = value;
            }
        }
        private static Material volumeMaterial;
    }
}