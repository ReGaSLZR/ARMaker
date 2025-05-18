using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class GIFChoiceButton : MonoBehaviour
    {

        [SerializeField]
        private Button button;

        [SerializeField]
        private RawImage rawImage;

        private AnimatorLayerData cachedData;

        public void SetUp(AnimatorLayerData data)
        {
            cachedData = data;

            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"Data is NULL!!!", gameObject);
                return;
            }

            rawImage.texture = data.Sprite.texture;
        }

        public void RegisterOnClick(Action<AnimatorLayerData> listener)
        {
            if (listener == null)
            {
                Debug.LogError($"{GetType().Name}.RegisterOnClick(): " 
                    + $"Listener is NULL!!!", gameObject);
                return;
            }

            button.onClick.AddListener(
                () => listener.Invoke(cachedData));
        }

    }

}