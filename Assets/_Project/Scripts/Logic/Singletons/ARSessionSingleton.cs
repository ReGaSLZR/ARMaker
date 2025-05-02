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

        private ARSessionOrigin cachedSessionOrigin;
        private ARTrackedImageManager cachedARManager;
        private GameObject cachedARObject;

        private Action onSessionOriginAvailable;
        private Action<ARTrackedImage> onTrackedMarker;

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
            this.cachedSessionOrigin = sessionOrigin;
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

            cachedARObject = Instantiate(prefabARBlankObject,
                cachedSessionOrigin.transform);
            return cachedARObject.transform;
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
            if (cachedARObject != null)
            {
                Destroy(cachedARObject);
                cachedARObject = null;
            }
        }

        private void SetUpTracking()
        {
            cachedARManager = cachedSessionOrigin.gameObject
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
                cachedARObject = trackedImage.gameObject;

                onTrackedMarker?.Invoke(trackedImage);
            }

            //foreach (var updatedImage in eventArgs.updated) { }
            //foreach (var removedImage in eventArgs.removed) { }
        }

    }

}