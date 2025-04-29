using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMarker
{ 

    public class ARMarkerChooserSingleton : BaseSingleton<ARMarkerChooserSingleton>
    {

        [Header("Data")]

        [SerializeField]
        private ARMarkerChoices choices;

        [SerializeField]
        private MarkerChoiceButton prefabButton;

        [Space]

        [SerializeField]
        private ARObject prefabARObject;

        [SerializeField]
        private ARSessionOrigin sessionOrigin;

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootUI;

        [SerializeField]
        private Transform markerButtonsParent;

        private readonly List<MarkerChoiceButton> buttonsSpawned = new();
        private ARTrackedImageManager manager;
        private ARTrackedImage trackedImage;
        private Texture2D cachedMarker;

        protected override void Awake()
        {
            base.Awake();
            SetUp();
        }

        private void SetUp()
        {
            if (choices == null || choices.Choices.Count == 0)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"Choices are missing!", gameObject);
                return;
            }

            foreach (var choice in choices.Choices)
            {
                if (choice == null)
                {
                    continue;
                }

                var clone = Instantiate(prefabButton, markerButtonsParent);
                clone.SetUp(choice, OnClickChoice);
                buttonsSpawned.Add(clone);
            }

            rootUI.gameObject.SetActive(false);
            //OnClickChoice(buttonsSpawned[0]);
        }

        private void SetUpTrackedImageManager(Texture2D marker)
        {
            if (manager != null)
            {
                manager.trackedImagesChanged -= OnChangedTrackedImage;

                if (trackedImage != null && trackedImage.gameObject != null)
                {
                    DestroyImmediate(trackedImage.gameObject);
                    trackedImage = null;
                }

                Destroy(manager);
                manager = null;
            }

            //prefabARObject.gameObject.SetActive(true);
            //var clone = Instantiate(prefabARObject);
            //clone.SetSprite(marker);
            //prefabARObject.gameObject.SetActive(false);

            manager = sessionOrigin.gameObject.AddComponent<ARTrackedImageManager>();
            manager.trackedImagePrefab = prefabARObject.gameObject;
            //manager.trackedImagePrefab = clone.gameObject;
            manager.trackedImagesChanged += OnChangedTrackedImage;
        }

        private void SetUpImageButtonsStatus(MarkerChoiceButton button)
        {
            foreach (var buttonSpawned in buttonsSpawned)
            {
                buttonSpawned.SetIsSelected(false);
            }

            button.SetIsSelected(true);
            rootUI.gameObject.SetActive(false);
        }

        private void OnClickChoice(MarkerChoiceButton button)
        {
            cachedMarker = button.GetMarker();
            SetUpImageButtonsStatus(button);
            SetUpTrackedImageManager(cachedMarker);

            var library = manager.CreateRuntimeLibrary();
            manager.referenceLibrary = library;

            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(
                    cachedMarker, cachedMarker.name, 0.5f);
                manager.enabled = true;

                Debug.LogWarning($"{GetType().Name}.OnClickChoice(): " +
                    $"Set Marker to: '{cachedMarker.name}'", gameObject);
            }
            else
            {
                Debug.LogError($"{GetType().Name}.OnClickChoice(): " +
                    $"COULD NOT PROCESS NEW MARKER CHOICE " +
                    $"'{cachedMarker.name}'", gameObject);
            }
        }

        private void OnChangedTrackedImage(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var newImage in eventArgs.added)
            {
                trackedImage = newImage;

                //if (trackedImage.gameObject.TryGetComponent<ARObject>(out var aRObject))
                //{
                //    aRObject.SetSprite(cachedMarker);
                //}
                // Handle added event
            }

            foreach (var updatedImage in eventArgs.updated)
            {
                // Handle updated event
            }

            foreach (var removedImage in eventArgs.removed)
            {
                // Handle removed event
            }
        }

        public void ShowChooserUI() => rootUI.gameObject.SetActive(true);

    }

}