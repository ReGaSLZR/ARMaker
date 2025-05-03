using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{ 

    public class ARMarkerChooser : MonoBehaviour
    {

        [Header("Data")]

        [SerializeField]
        private ARMarkerChoices choices;

        [SerializeField]
        private MarkerChoiceButton prefabButton;

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootUI;

        [SerializeField]
        private Transform markerButtonsParent;

        private readonly List<MarkerChoiceButton> cachedSpawnedButtons = new();
        private Action<Sprite> onChooseMarker;

        private void Start()
        {
            GameManager.Instance.RegisterMarkerChooser(this);
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
                cachedSpawnedButtons.Add(clone);
            }

            rootUI.gameObject.SetActive(false);
        }

        public void RegisterOnChooseMarker(Action<Sprite> listener)
        {
            if (listener == null)
            {
                return;
            }

            onChooseMarker += listener;
        }

        private void SetUpImageButtonsStatus(MarkerChoiceButton button)
        {
            foreach (var buttonSpawned in cachedSpawnedButtons)
            {
                buttonSpawned.SetIsSelected(false);
            }

            button.SetIsSelected(true);
            rootUI.gameObject.SetActive(false);
        }

        private void OnClickChoice(MarkerChoiceButton button)
        {
            onChooseMarker?.Invoke(button.GetMarker());

            SetUpImageButtonsStatus(button);
            rootUI.gameObject.SetActive(false);

            //TODO remove this:
            WorkSpaceSingleton.Instance.AddLayer(button.GetMarker());
        }

        public void ShowChooserUI() => rootUI.gameObject.SetActive(true);

    }

}