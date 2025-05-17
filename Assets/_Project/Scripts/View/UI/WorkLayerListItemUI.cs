using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class WorkLayerListItemUI : MonoBehaviour
    {

        [SerializeField]
        private RawImage rawImagePreview;

        [SerializeField]
        private TextMeshProUGUI textName;

        [Space]

        [SerializeField]
        private Button buttonLock;

        [SerializeField]
        private Button buttonDuplicate;

        [SerializeField]
        private Button buttonDelete;

        private WorkLayer cachedLayer;

        private void Awake()
        {
            buttonDelete.onClick.AddListener(DeleteSelf);
            buttonDuplicate.onClick.AddListener(DuplicateLayer);

            buttonLock.onClick.AddListener(LockLayer);
        }

        private void LockLayer()
        {
            if (cachedLayer == null)
            {
                return;
            }

            cachedLayer.ToggleLockState();
        }

        private void DuplicateLayer()
        { 
            WorkSpaceSingleton.Instance.DuplicateLayer(cachedLayer);
        }

        private void DeleteSelf()
        {
            WorkSpaceSingleton.Instance.DeleteLayer(cachedLayer);
            DestroyImmediate(gameObject);
        }

        public void SetUp(WorkLayer layer)
        {
            if (layer == null || layer.Data == null)
            {
                return;
            }

            cachedLayer = layer;
            textName.text = layer.Data.sprite.name;
            rawImagePreview.texture = layer.Data.sprite.texture;
        }

    }

}