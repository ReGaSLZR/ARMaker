﻿using System;
using System.Collections;
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
        private ARTrackedImageManager cachedARManager;

        private ARSession cachedSession;
        private ARSessionOrigin cachedSessionOrigin;
        
        private GameObject cachedARObject;

        private Action<ARStatus> onStatusChange;
        private ARStatus cachedStatus;

        protected override void Awake()
        {
            base.Awake();
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

        public void RegisterSessionOrigin(
            ARSession session, ARSessionOrigin sessionOrigin)
        {
            cachedSession = session;
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
            onStatusChange?.Invoke(ARStatus.UNSET);

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
            Debug.LogWarning($"{GetType().Name}" +
                $".SetUpTracking(): cachedSessionOrigin is null? " +
                $"{cachedSessionOrigin == null}", gameObject);

            Debug.LogWarning($"{GetType().Name}" +
                $".SetUpTracking(): cachedSession is null? " +
                $"{cachedSession == null}", gameObject);

#if PLATFORM_IOS || UNITY_IOS
            cachedSession.enabled = false;
#endif
            cachedARManager = cachedSessionOrigin.gameObject
                .AddComponent<ARTrackedImageManager>();
            cachedARManager.trackedImagePrefab = prefabARBlankObject;
            cachedARManager.requestedMaxNumberOfMovingImages = maxNumberOfMovingImages;
            cachedARManager.trackedImagesChanged += OnChangedTrackedImage;
        }

        private IEnumerator C_StartTracking(Sprite marker)
        {
            DisableActiveTracking();
            SetUpTracking();

            onStatusChange?.Invoke(ARStatus.UNSET);

#if UNITY_EDITOR
            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): Skipping the creation " +
                $"of runtime XR marker library.", gameObject);

            onStatusChange?.Invoke(ARStatus.MarkerDetected);
            yield break;
#endif
            cachedARManager.enabled = true;
            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): ARSession.state is {ARSession.state}", gameObject);

            while (ARSession.state != ARSessionState.SessionTracking)
            {
                yield return null;
            }

            var library = cachedARManager.CreateRuntimeLibrary();
            cachedARManager.referenceLibrary = library;

            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): Created runtime library", gameObject);

            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                var handle = mutableLibrary.ScheduleAddImageWithValidationJob(
                    marker.texture, marker.name, markerWidth);
                handle.jobHandle.Complete();

                yield return null;

#if PLATFORM_IOS || UNITY_IOS
                cachedSession.Reset();
                cachedSession.enabled = true;
#endif

                cachedARManager.enabled = true;

                if (handle.status != AddReferenceImageJobStatus.Success)
                {
                    Debug.LogError($"{GetType().Name}.StartTracking(): " +
                        $"Failed to add image! Status: {handle.status}");
                    yield break;
                }
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

        public void StartTracking(Sprite marker)
        {
            StopAllCoroutines();
            StartCoroutine(C_StartTracking(marker));
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