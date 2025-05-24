using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARMarker
{
    public abstract class BaseChoiceButton<T>
        : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
        where T : class
    {

        [SerializeField]
        protected Button button;

        [SerializeField]
        protected RawImage rawImage;

        protected T cachedData;

        [SerializeField]
        protected ScrollRect cachedScrollRect;
        [SerializeField]
        protected RectTransform cachedDropArea;

        protected bool isPrefabSpawned;
        protected bool isDraggable;
        protected WorkLayer cachedWorkLayer;

        protected void SetImage(Texture2D texture)
        {
            rawImage.texture = texture;
        }

        public virtual void SetUp(T data, ScrollRect scrollRect,
            RectTransform dropArea, bool isDraggable)
        {
            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"Data is NULL!!!", gameObject);
                return;
            }

            cachedData = data;
            cachedScrollRect = scrollRect;
            cachedDropArea = dropArea;
            this.isDraggable = isDraggable;

            if (!isDraggable)
            {
                button.onClick.AddListener(() => AddLayer(data));
            }
        }

        protected abstract WorkLayer AddLayer(T data);

        #region EventSystems

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cachedScrollRect == null)
            {
                return;
            }

            // Allow the ScrollRect to handle scrolling.
            cachedScrollRect.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDraggable && cachedScrollRect)
            {
                // Continue dragging the ScrollRect to handle scrolling.
                cachedScrollRect.OnDrag(eventData);
            }
            else if (!isPrefabSpawned)
            {
                if (IsCursorInDropAreaBounds(eventData.position))
                {
                    isPrefabSpawned = true;

                    cachedWorkLayer = AddLayer(cachedData);
                    cachedWorkLayer.SetupInitialDrag(true);

                    // Stop the scroll view from handling drag
                    cachedScrollRect.OnEndDrag(eventData);
                }
                else
                {
                    if (cachedScrollRect == null)
                    {
                        return;
                    }
                    // Continue dragging the ScrollRect to handle scrolling.
                    cachedScrollRect.OnDrag(eventData);
                }
            }
            else
            {
                // Dragging the Spawned object.
                cachedWorkLayer.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isPrefabSpawned)
            {
                isPrefabSpawned = false;
                cachedWorkLayer.OnEndDrag(eventData);
            }
            else
            {
                // We never spawned a prefab.
                if (cachedScrollRect == null)
                {
                    return;
                }

                // Allow the ScrollRect to handle scrolling.
                cachedScrollRect.OnEndDrag(eventData);
            }
        }

        #endregion // EventSystems

        private bool IsCursorInDropAreaBounds(Vector2 screenPoint)
        {
            if (cachedScrollRect == null)
            {
                return false;
            }

            // Check if point is inside the ScrollRect

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cachedDropArea,
                screenPoint,
                null, // For Canvas with Screen Space - Overlay
                out localPoint);

            // Check if the point is inside the dropArea and return it.
            return cachedDropArea.rect.Contains(localPoint);
        }

    }

}