using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{
    public abstract class BaseChoiceButton<T> 
        : MonoBehaviour
        where T : class
    {

        [SerializeField]
        protected Button button;

        [SerializeField]
        protected RawImage rawImage;

        protected T cachedData;

        protected void SetImage(Texture2D texture)
        {
            rawImage.texture = texture;
        }

        public virtual void SetUp(T data)
        {
            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"Data is NULL!!!", gameObject);
                return;
            }

            cachedData = data;
        }

        public void RegisterOnClick(Action<T> listener)
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