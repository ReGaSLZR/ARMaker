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

        [SerializeField]
        private GameObject prefabARBlankObject;

        [Space]

        [SerializeField]
        private ARObject workCanvas;

        [SerializeField]
        private ARSessionOrigin sessionOrigin;

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootUI;

        [SerializeField]
        private Transform markerButtonsParent;

        private readonly List<MarkerChoiceButton> buttonsSpawned = new();
        private ARTrackedImageManager cachedARManager;

        private ARObject cachedARSpawn;
        private ARTrackedImage cachedTrackedImage;
        private Sprite cachedMarker;

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

        private void SetUpTrackedImageManager()
        {
            if (cachedARManager != null)
            {
                cachedARManager.trackedImagesChanged -= OnChangedTrackedImage;
                Destroy(cachedARManager);
                cachedARManager = null;
            }

            if (cachedARSpawn != null)
            {
                Destroy(cachedARSpawn.gameObject);
                cachedARSpawn = null;
            }

            if (cachedTrackedImage != null)
            {
                Destroy(cachedTrackedImage.gameObject);
                cachedTrackedImage = null;
            }

            cachedARManager = sessionOrigin.gameObject.AddComponent<ARTrackedImageManager>();
            cachedARManager.trackedImagePrefab = prefabARBlankObject;
            cachedARManager.trackedImagesChanged += OnChangedTrackedImage;
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

            workCanvas.SetSprite(cachedMarker);
            SetUpImageButtonsStatus(button);

#if UNITY_EDITOR
            return;
#endif

            workCanvas.gameObject.SetActive(false);
            SetUpTrackedImageManager();

            var library = cachedARManager.CreateRuntimeLibrary();
            cachedARManager.referenceLibrary = library;

            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(
                    cachedMarker.texture, cachedMarker.name, 0.5f);
                cachedARManager.enabled = true;

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

        private void CloneWorkCanvas(ARTrackedImage trackedImage)
        {
            workCanvas.gameObject.SetActive(true);

            var clone = Instantiate(workCanvas);
            clone.transform.SetParent(trackedImage.transform);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.rotation = Quaternion.identity;

            cachedARSpawn = clone;
            workCanvas.gameObject.SetActive(false);
            cachedTrackedImage = trackedImage;
        }    

        private void OnChangedTrackedImage(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // Handle added event
                CloneWorkCanvas(trackedImage);
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