using UnityEngine;

namespace ARMarker
{

    public class WorkLayerListUI : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private GameObject rootUI;

        [SerializeField]
        private Transform rootListItem;

        [SerializeField]
        private GameObject rootTempViewOnEmpty;

        [Header("Data")]

        [SerializeField]
        private WorkLayerListItemUI prefabListItem;

        private void Start()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnNewLayerAdded);
            WorkSpaceSingleton.Instance
                .RegisterOnLayerCountChange(OnLayerCountChange);
            GameManager.Instance.RegisterOnError(OnError);

            rootTempViewOnEmpty.SetActive(true);
            rootUI.SetActive(false);
            SetUpCachedLayers();
        }

        private void OnDestroy()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnNewLayerAdded, true);
            WorkSpaceSingleton.Instance
                .RegisterOnLayerCountChange(OnLayerCountChange, true);
            GameManager.Instance.RegisterOnError(OnError, true);
        }

        private void OnError(string error)
        {
            if (GameManager.Instance.GetMarker() == null)
            {
                return;
            }

            rootUI.SetActive(true);
        }

        private void SetUpCachedLayers()
        {
            var layers = WorkSpaceSingleton.Instance.GetLayers();
            
            foreach (var layer in layers)
            {
                OnNewLayerAdded(layer);
            }
        }

        private void OnLayerCountChange(int count)
        {
            rootTempViewOnEmpty.SetActive(count == 0);
        }

        private void OnNewLayerAdded(WorkLayer newLayer)
        {
            if (newLayer == null)
            {
                return;
            }

            var listItem = Instantiate(prefabListItem, rootListItem);
            listItem.SetUp(newLayer);
        }

    }

}