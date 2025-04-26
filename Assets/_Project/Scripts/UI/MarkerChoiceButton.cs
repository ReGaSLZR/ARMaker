using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class MarkerChoiceButton : MonoBehaviour
    {

        [SerializeField]
        private Button button;

        [SerializeField]
        private RawImage rawImage;

        [Space]

        [SerializeField]
        private Color colorNormal = Color.white;

        [SerializeField]
        private Color colorOnSelect = Color.white;

        public void SetUp(Texture2D marker, Action<MarkerChoiceButton> listener)
        {
            if (listener != null)
            {
                button.onClick.AddListener(
                    () => listener.Invoke(this));
            }
            
            rawImage.texture = marker;
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected) 
            => button.image.color = isSelected 
                ? colorOnSelect : colorNormal;

        public Texture2D GetMarker() => (Texture2D)rawImage.texture;

    }

}