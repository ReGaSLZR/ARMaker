using UnityEngine;

namespace ARMarker
{
    [System.Serializable]
    public class WorkLayerData
    {

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public Sprite sprite;

        public void ResetTransform()
        {
            position = Vector3.zero;
            scale = Vector3.one;
            rotation = Quaternion.identity;
        }

    }

}