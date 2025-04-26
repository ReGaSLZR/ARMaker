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
        ARTrackedImageManager trackedImageManager;

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootUI;

        [SerializeField]
        private Transform markerButtonsParent;

        private readonly List<MarkerChoiceButton> buttonsSpawned = new();

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
            OnClickChoice(buttonsSpawned[0]);
        }

        private void OnClickChoice(MarkerChoiceButton button)
        {
            foreach (var buttonSpawned in buttonsSpawned)
            {
                buttonSpawned.SetIsSelected(false);
            }

            button.SetIsSelected(true);

            var library = trackedImageManager.CreateRuntimeLibrary();
            var marker = button.GetMarker();

            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(
                    marker, marker.name, 0.5f);
                trackedImageManager.referenceLibrary = library;
                trackedImageManager.enabled = true;
            }
            else
            {
                Debug.LogError($"{GetType().Name}.OnClickChoice(): " +
                    $"COULD NOT PROCESS NEW MARKER CHOICE " +
                    $"'{marker.name}'", gameObject);
            }
        }

        public void ShowChooserUI() => rootUI.gameObject.SetActive(true);

    }

}