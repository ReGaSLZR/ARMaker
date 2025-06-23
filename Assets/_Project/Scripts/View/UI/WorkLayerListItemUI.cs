using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class WorkLayerListItemUI : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private RawImage rawImagePreview;

        [SerializeField]
        private TextMeshProUGUI textName;

        [SerializeField]
        private SwipeBlocker swipeBlocker;

        [Header("UI Indicators")]

        [SerializeField]
        private GameObject indicatorVideo;

        [SerializeField]
        private GameObject indicatorGIF;

        [SerializeField]
        private GameObject indicatorSFX;

        [SerializeField]
        private string prefixLocked = "[LOCKED] ";

        [Header("Colors")]

        [SerializeField]
        private Color colorBgNormal;

        [SerializeField]
        private Color colorBgHighlighted;

        [Header("Buttons")]

        [SerializeField]
        private Button buttonBackground;

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

        private void OnEnable()
        {
            ExecuteSetUp();
        }

        private void OnDestroy()
        {
            if (cachedLayer != null)
            {
                cachedLayer.RegisterOnSetUpData(
                    ExecuteSetUp, OnSelectedLayer, true);
            }
        }

        private void LockLayer()
        {
            if (cachedLayer == null)
            {
                return;
            }

            cachedLayer.ToggleLockState();

            var spriteName = cachedLayer.Data.sprite.name;
            textName.text = (cachedLayer.IsLocked)
                ? (prefixLocked + spriteName) : spriteName;
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

        private void OnSelectedLayer(bool isSelected)
        {
            buttonBackground.image.color = isSelected 
                ? colorBgHighlighted : colorBgNormal;
        }

        public void SetLayer(WorkLayer layer)
        {
            cachedLayer = layer;
            cachedLayer.RegisterOnSetUpData(ExecuteSetUp, OnSelectedLayer);
            buttonBackground.onClick.AddListener(cachedLayer.Select);
            ExecuteSetUp();
        }

        public void SetScrollRect(ScrollRect scrollRect)
        { 
            swipeBlocker.SetScrollRect(scrollRect);
        }

        private void ExecuteSetUp()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            StartCoroutine(C_SetUpCache());
        }

        private IEnumerator C_SetUpCache()
        {
            yield return null;

            if (cachedLayer == null || cachedLayer.Data == null)
            {
                Debug.LogWarning($"NULL!!! Layer? {cachedLayer == null} | Data? {cachedLayer.Data == null}", gameObject);
                yield break;
            }

            textName.text = cachedLayer.Data.sprite.name;
            rawImagePreview.texture = cachedLayer.Data.sprite.texture;

            indicatorGIF.SetActive(
                cachedLayer.Data.animController != null);
            indicatorVideo.SetActive(
                cachedLayer.Data.videoClip != null);

            bool hasNoAudio = (cachedLayer.Data.audioClip == null);
            buttonLock.gameObject.SetActive(hasNoAudio);
            indicatorSFX.SetActive(!hasNoAudio);
            if (!hasNoAudio)
            { 
                textName.text = cachedLayer.Data.audioClip.name;
            }
        }

    }

}