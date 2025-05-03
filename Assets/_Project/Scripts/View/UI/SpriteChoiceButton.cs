using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class SpriteChoiceButton : MonoBehaviour
    {

        [SerializeField]
        private Button button;

        [SerializeField]
        private RawImage rawImage;

        private Sprite cachedSprite;

        public void SetUp(Sprite sprite)
        {
            cachedSprite = sprite;
            rawImage.texture = sprite.texture;
        }

        public void RegisterOnClick(Action<Sprite> listener)
        {
            if (listener == null)
            {
                return;
            }

            button.onClick.AddListener(
                () => listener.Invoke(cachedSprite));
        }

    }

}