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

        private ARTrackedImage cachedTrackedImage;
        private ARSessionOrigin cachedSessionOrigin;
        private ARTrackedImageManager cachedARManager;
        private GameObject cachedARObject;

        private Action<ARStatus> onStatusChange;
        private ARStatus cachedStatus;

        private void Start()
        {
            cachedStatus = ARStatus.UNSET;
            RegisterOnStatusChange(OnARStatusChange);
        }

        private void OnARStatusChange(ARStatus status)
        {
            cachedStatus = status;
        }

        public ARStatus GetStatus() => cachedStatus;

        public void RegisterOnStatusChange(Action<ARStatus> listener, 
            bool deRegisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }


            if(deRegisterInstead)
            {
                onStatusChange -= listener;
            }
            else
            {
                onStatusChange += listener;
            }
        }

        public ARTrackedImage GetTrackedImage() => cachedTrackedImage;

        public void RegisterSessionOrigin(ARSessionOrigin sessionOrigin)
        {
            cachedSessionOrigin = sessionOrigin;
            onStatusChange?.Invoke(ARStatus.SessionOriginCreated);
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

            if (cachedTrackedImage != null)
            {
                Destroy(cachedTrackedImage);
                cachedTrackedImage = null;
            }

            onStatusChange?.Invoke(ARStatus.UNSET);
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

            onStatusChange?.Invoke(ARStatus.UNSET);

#if UNITY_EDITOR
            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): Skipping the creation " +
                $"of runtime XR marker library.", gameObject);

            onStatusChange?.Invoke(ARStatus.MarkerDetected);
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

                onStatusChange?.Invoke(ARStatus.ScanningMarker);
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

                cachedTrackedImage = trackedImage;
                onStatusChange?.Invoke(ARStatus.MarkerDetected);
            }

            foreach (var updatedImage in eventArgs.updated) 
            {
                cachedTrackedImage = updatedImage;
                onStatusChange?.Invoke(ARStatus.ActivelyTrackingMarker);
            }
            
            foreach (var removedImage in eventArgs.removed) 
            {
                onStatusChange?.Invoke(ARStatus.LostMarker);
            }
        }

    }

}