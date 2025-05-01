using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMarker
{
    public class ARSessionSingleton : BaseSingleton<ARSessionSingleton>
    {

        [Header("AR Settings")]

        [SerializeField]
        private GameObject prefabARBlankObject;

        [SerializeField]
        private int maxNumberOfMovingImages;

        [SerializeField]
        private float markerWidth = 0.5f;

        [Header("Runtime Data")]

        [SerializeField]
        private ARSessionOrigin sessionOrigin;

        [SerializeField]
        private ARTrackedImageManager cachedARManager;

        private Action onSessionOriginAvailable;
        private Action<ARTrackedImage> onTrackedMarker;

        private GameObject tempARObject;

        public void RegisterOnTrackedMarker(Action<ARTrackedImage> listener)
        {
            if (listener == null)
            {
                return;
            }

            onTrackedMarker += listener;
        }

        public void RegisterSessionOrigin(ARSessionOrigin sessionOrigin)
        {
            this.sessionOrigin = sessionOrigin;
            onSessionOriginAvailable?.Invoke();
        }

        public void RegisterOnSessionOriginAvailable(Action listener)
        {
            if (listener == null)
            {
                return;
            }

            onSessionOriginAvailable += listener;
        }

        public Transform InstantiateARObjectToSessionOrigin()
        {
            SafelyDeleteSpawnedARObject();

            tempARObject = Instantiate(prefabARBlankObject,
                sessionOrigin.transform);
            return tempARObject.transform;
        }

        public void DisableActiveTracking()
        {
            SafelyDeleteSpawnedARObject();

            if (cachedARManager != null)
            {
                cachedARManager.trackedImagesChanged -= OnChangedTrackedImage;
                Destroy(cachedARManager);
                cachedARManager = null;
            }
        }

        private void SafelyDeleteSpawnedARObject()
        {
            if (tempARObject != null)
            {
                Destroy(tempARObject);
                tempARObject = null;
            }
        }

        private void SetUpTracking()
        {
            cachedARManager = sessionOrigin.gameObject
                .AddComponent<ARTrackedImageManager>();
            cachedARManager.trackedImagePrefab = prefabARBlankObject;
            cachedARManager.requestedMaxNumberOfMovingImages = maxNumberOfMovingImages;
            cachedARManager.trackedImagesChanged += OnChangedTrackedImage;
        }

        public void StartTracking(Sprite marker)
        {
            DisableActiveTracking();
            SetUpTracking();

#if UNITY_EDITOR
            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): Skipping the creation " +
                $"of runtime XR marker library.", gameObject);

            onTrackedMarker?.Invoke(null);
            return;
#endif

            var library = cachedARManager.CreateRuntimeLibrary();
            cachedARManager.referenceLibrary = library;

            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(
                    marker.texture, marker.name, markerWidth);
                cachedARManager.enabled = true;

                Debug.Log($"{GetType().Name}.StartTracking(): " +
                    $"Set Marker to: '{marker.name}'", gameObject);
            }
            else
            {
                Debug.LogError($"{GetType().Name}.StartTracking(): " +
                    $"COULD NOT PROCESS NEW MARKER CHOICE " +
                    $"'{marker.name}'", gameObject);
            }
        }

        private void OnChangedTrackedImage(
            ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                SafelyDeleteSpawnedARObject();
                tempARObject = trackedImage.gameObject;

                onTrackedMarker?.Invoke(trackedImage);
            }

            //foreach (var updatedImage in eventArgs.updated) { }
            //foreach (var removedImage in eventArgs.removed) { }
        }

    }

}