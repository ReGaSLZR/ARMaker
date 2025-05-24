using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

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

        private void OnEnable()
        {
            StartCoroutine(C_SetUpCache());
            StartCoroutine(C_DelayedCheckForAudio());
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
            cachedLayer = layer;
            
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(C_SetUpCache());
                StartCoroutine(C_DelayedCheckForAudio());
            }
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
        }

        private IEnumerator C_DelayedCheckForAudio()
        { 
            yield return null;

            if (cachedLayer != null
                && cachedLayer.Data != null
                && cachedLayer.Data.audioClip != null)
            {
                buttonLock.gameObject.SetActive(false);
            }
        }

    }

}