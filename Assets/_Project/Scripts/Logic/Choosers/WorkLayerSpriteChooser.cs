using UnityEngine;

namespace ARMarker
{

    public class WorkLayerSpriteChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [Header("Data")]

        [SerializeField]
        private WorkLayerSpriteChoices choices;

        [SerializeField]
        private SpriteChoiceButton prefabButton;

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