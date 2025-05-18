using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private Button buttonTempLayer;

        [Header("Data")]

        [SerializeField]
        private WorkLayerListItemUI prefabListItem;

        private void Start()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnNewLayerAdded);
            WorkSpaceSingleton.Instance
                .RegisterOnLayerCountChange(OnLayerCountChange);
            WorkSpaceSingleton.Instance
                .RegisterOnUpdatetempLayer(OnChangeTempLayer);

            buttonTempLayer.onClick.AddListener(WorkSpaceSingleton.Instance.ToggleTempLayer);

            rootTempViewOnEmpty.SetActive(true);
            rootUI.SetActive(false);

            SetUpCachedLayers();
            OnChangeTempLayer(WorkSpaceSingleton.Instance
                .GetTempLayer());
        }

        private void OnDestroy()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnNewLayerAdded(OnNewLayerAdded, true);
            WorkSpaceSingleton.Instance
                .RegisterOnLayerCountChange(OnLayerCountChange, true);
            WorkSpaceSingleton.Instance
                .RegisterOnUpdatetempLayer(OnChangeTempLayer, true);
        }

        private void SetUpCachedLayers()
        {
            var layers = WorkSpaceSingleton.Instance.GetLayers();
            
            foreach (var layer in layers)
            {
                OnNewLayerAdded(layer);
            }
        }

        private void OnChangeTempLayer(WorkLayer tempLayer)
        {
            buttonTempLayer.interactable = tempLayer != null;
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