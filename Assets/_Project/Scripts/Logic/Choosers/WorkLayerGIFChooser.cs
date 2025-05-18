using UnityEngine;

namespace ARMarker
{

    public class WorkLayerGIFChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [Header("Data")]

        [SerializeField]
        private WorkLayerGIFChoices choices;

        [SerializeField]
        private SpriteChoiceButton prefabButton;

    }

}