using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{ 

    public class ARMarkerChooserSingleton : BaseSingleton<ARMarkerChooserSingleton>
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

        private readonly List<MarkerChoiceButton> buttonsSpawned = new();
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
            rootUI.gameObject.SetActive(false);

            //TODO remove this:
            WorkCanvasSingleton.Instance.AddLayer(cachedMarker);
        }

        public void ShowChooserUI() => rootUI.gameObject.SetActive(true);

        public Sprite GetChosenMarker() => cachedMarker;

    }

}