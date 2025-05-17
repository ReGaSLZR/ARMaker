using UnityEngine;

namespace ARMarker
{

    public class WorkLayerSpriteChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [SerializeField]
        private GameObject rootChoicesUI;

        [Header("Data")]

        [SerializeField]
        private WorkLayerSpriteChoices choices;

        [SerializeField]
        private SpriteChoiceButton prefabButton;

        private void Start()
        {
            SetUpButtons();

            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnAddNewLayer);
        }

        private void OnDestroy()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnAddNewLayer, true);
        }

        private void OnAddNewLayer(WorkLayer layer)
        { 
            rootChoicesUI.SetActive(false);
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

            foreach (var sprite in choices.Choices)
            { 
                if(sprite == null)
                {
                    continue;
                }

                var button = Instantiate(prefabButton, rootChoicesButton);
                button.SetUp(sprite);
                button.RegisterOnClick(OnClickChoice);
            }
        }

        private void OnClickChoice(Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            WorkSpaceSingleton.Instance.AddLayer(sprite);
        }

    }

}