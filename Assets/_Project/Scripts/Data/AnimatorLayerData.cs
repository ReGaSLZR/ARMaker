using UnityEditor.Animations;
using UnityEngine;

namespace ARMarker
{

    [System.Serializable]
    public class AnimatorLayerData
    {

        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private AnimatorController controller;

        public Sprite Sprite => sprite;
        public AnimatorController Controller => controller;

    }

}