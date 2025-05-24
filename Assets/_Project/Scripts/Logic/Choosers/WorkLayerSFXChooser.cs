using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class WorkLayerSFXChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform dropAreaRect;

        [Header("Data")]

        [SerializeField]
        private WorkLayerSFXChoices choices;

        [SerializeField]
        private SFXChoiceButton prefabButton;

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

            foreach (var choice in choices.Choices)
            {
                if (choice == null)
                {
                    continue;
                }

                var button = Instantiate(prefabButton, rootChoicesButton);
                button.SetUp(choice, scrollRect, dropAreaRect, false);
                //button.RegisterOnClick(OnClickChoice);
            }
        }

        //private void OnClickChoice(SFXLayerData data)
        //{
        //    if (data == null)
        //    {
        //        return;
        //    }

        //    WorkSpaceSingleton.Instance.AddSFXLayer(data);
        //}

    }

}