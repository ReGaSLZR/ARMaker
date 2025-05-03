using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    [RequireComponent(typeof(Button))]
    public class TerminalButton : MonoBehaviour
    {

        [SerializeField]
        private Scene sceneTarget;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            switch (sceneTarget)
            {
                case Scene.ARPreview:
                    {
                        GameManager.Instance.LoadARWorld();
                        break;
                    }
                case Scene.WorkSpace:
                    {
                        GameManager.Instance.LoadWorkSpace();
                        break;
                    }
            }
        }

    }

}