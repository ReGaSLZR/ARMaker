﻿using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{
    public class WorkLayerVideoChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [Header("Data")]

        [SerializeField]
        private WorkLayerVideoChoices choices;

        [SerializeField]
        private VideoChoiceButton prefabButton;

        private void Start()
        {
            SetUpButtons();
        }

        private void SetUpButtons()
        {
            if (choices == null
                || choices.Choices.Count == 0)
            {
                Debug.LogError($"{GetType().Name} " +
                    $"Missing Choices!", gameObject);
                return;
            }

            ScrollRect scrollRect = rootChoicesButton.GetComponentInParent<ScrollRect>(true);
            RectTransform dropAreaRect = FindObjectOfType<DropArea>().GetComponent<RectTransform>();

            foreach (var choice in choices.Choices)
            {
                if (choice == null)
                {
                    continue;
                }

                var button = Instantiate(prefabButton, rootChoicesButton);
                button.SetUp(choice, scrollRect, dropAreaRect);
                button.RegisterOnClick(OnClickChoice);
            }
        }

        private void OnClickChoice(VideoLayerData data)
        {
            if (data == null)
            {
                return;
            }

            WorkSpaceSingleton.Instance.AddVideoLayer(data);
        }

    }

}