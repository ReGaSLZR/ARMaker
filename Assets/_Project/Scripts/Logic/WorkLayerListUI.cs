using UnityEngine;

namespace ARMarker
{

    public class WorkLayerListUI : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootLayer;

        [Header("Data")]

        [SerializeField]
        private WorkLayerListItemUI prefabListItem;

        private void Start()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnNewLayerAdded);

            SetUpCachedLayers();
        }

        private void OnDestroy()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnNewLayerAdded, true);
        }

        private void SetUpCachedLayers()
        {
            var layers = WorkSpaceSingleton.Instance.GetLayers();
            
            foreach (var layer in layers)
            {
                OnNewLayerAdded(layer);
            }
        }

        private void OnNewLayerAdded(WorkLayer newLayer)
        {
            if (newLayer == null)
            {
                return;
            }

            var listItem = Instantiate(prefabListItem, rootLayer);
            listItem.SetUp(newLayer);
        }

    }

}