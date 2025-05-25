using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace ARMarker
{

    [RequireComponent(typeof(BoxCollider))]
    public class WorkLayer : MonoBehaviour,
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        [Header("Elements")]

        [SerializeField]
        private GameObject statusLocked;

        [SerializeField]
        private GameObject statusSelected;

        [Header("Settings")]

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private VideoPlayerController videoController;

        [SerializeField]
        private AudioSource audioSource;

        [Header("Layer Transform Handlers")]

        [SerializeField]
        private LayerRepositionXYHandler repositionXYHandler;

        [SerializeField]
        private LayerRotationHandler rotationHandler;

        [SerializeField]
        private LayerResizeXYHandler resizeHandler;

        public WorkLayerData Data => cachedData;

        private WorkLayerData cachedData;
        private BoxCollider boxCollider;
        private Camera mainCamera;
        private Action onSetUpData;

        private bool isLocked;
        private bool isInitialDrag;

        private void Awake()
        {
            mainCamera = Camera.main;

            boxCollider = GetComponent<BoxCollider>();
            statusLocked.SetActive(false);
        }

        private void Start()
        {
            repositionXYHandler.RegisterListener(OnSelectLayer);
            rotationHandler.RegisterListener(OnSelectLayer);
            resizeHandler.RegisterListener(OnSelectLayer);

            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayerEditMode(OnChangeLayerEditMode);
            OnChangeLayerEditMode(WorkSpaceSingleton.Instance.GetLayerEditMode());

            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayer(OnChangeActiveLayer);
        }

        private void OnDestroy()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayerEditMode(OnChangeLayerEditMode, true);
            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayer(OnChangeActiveLayer, true);
        }

        private void OnChangeActiveLayer(WorkLayer activeLayer)
        {
            if (this == activeLayer)
            {
                return;
            }

            Deselect();
        }

        private void OnChangeLayerEditMode(LayerEditMode mode)
        {
            switch (mode)
            {
                case LayerEditMode.Reposition:
                    {
                        repositionXYHandler.enabled = true;

                        rotationHandler.Deselect();
                        resizeHandler.Deselect();

                        rotationHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Rotate:
                    {
                        rotationHandler.enabled = true;

                        repositionXYHandler.Deselect();
                        resizeHandler.Deselect();

                        repositionXYHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Resize:
                    {
                        resizeHandler.enabled = true;

                        repositionXYHandler.Deselect();
                        rotationHandler.Deselect();

                        repositionXYHandler.enabled = false;
                        rotationHandler.enabled = false;
                        break;
                    }
            }
        }

        private void OnSelectLayer()
        {
            WorkSpaceSingleton.Instance.ChangeActiveLayer(this);
            //statusSelected.SetActive(true);

            var mode = WorkSpaceSingleton.Instance.GetLayerEditMode();

            switch (mode)
            {
                case LayerEditMode.Reposition:
                    {
                        rotationHandler.Deselect();
                        resizeHandler.Deselect();

                        rotationHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Rotate:
                    {
                        repositionXYHandler.Deselect();
                        resizeHandler.Deselect();

                        repositionXYHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Resize:
                    {
                        repositionXYHandler.Deselect();
                        rotationHandler.Deselect();

                        repositionXYHandler.enabled = false;
                        rotationHandler.enabled = false;
                        break;
                    }
            }
        }

        public void SetUp(WorkLayerData data)
        {
            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"data is null!", gameObject);
                return;
            }

            cachedData = data;

            gameObject.transform.localPosition = data.position;
            gameObject.transform.rotation = data.rotation;
            gameObject.transform.localScale = data.scale;

            boxCollider.enabled = !data.isTemporary;

            SetUpSprite();
            onSetUpData?.Invoke();
        }

        public void SetUpAnimator(RuntimeAnimatorController controller)
        {
            if (controller == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"controller is null!", gameObject);
                return;
            }

            cachedData.animController = controller;
            animator.runtimeAnimatorController = controller;
            onSetUpData?.Invoke();
        }

        public void SetUpSFX(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"clip is null!", gameObject);
                return;
            }

            cachedData.audioClip = clip;

            animator.enabled = false;
            spriteRenderer.enabled = false;
            videoController.gameObject.SetActive(false);
            boxCollider.enabled = false;

            audioSource.enabled = true;
            audioSource.clip = clip;
            audioSource.Play();
            onSetUpData?.Invoke();
        }

        public void SetToHalfTransparency()
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        public void SetUpVideoController(VideoClip clip)
        {
            if (clip == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " 
                    + $"clip is null!", gameObject);
                return;
            }

            cachedData.videoClip = clip;
            animator.enabled = false;
            spriteRenderer.enabled = false;

            videoController.SetUp(clip);
            videoController.gameObject.SetActive(true);
            onSetUpData?.Invoke();
        }

        public void RegisterOnSetUpData(Action listener, 
            bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onSetUpData -= listener;
            }
            else
            {
                onSetUpData += listener;
            }
        }

        public void ToggleLockState()
        {
            if (cachedData.audioClip != null)
            {
                return;
            }

            isLocked = !isLocked;
            boxCollider.enabled = !isLocked;
            statusLocked.SetActive(isLocked);
        }

        public void SetEnabledIfTemporary(bool isEnabled)
        {
            if (cachedData == null || !cachedData.isTemporary)
            {
                return;
            }

            gameObject.SetActive(isEnabled);
        }

        public void Deselect()
        {
            //statusSelected.SetActive(false);

            repositionXYHandler.Deselect();
            rotationHandler.Deselect();
            resizeHandler.Deselect();
        }

        private void SetUpSprite()
        {
            if (cachedData == null || cachedData.sprite == null)
            {
                Destroy(spriteRenderer);
                return;
            }

            spriteRenderer.sprite = cachedData.sprite;
            spriteRenderer.size = new Vector2(
                ConstantInts.AR_OBJECT_SIZE_DIVISOR, 
                ConstantInts.AR_OBJECT_SIZE_DIVISOR);
        }

        public void SetupInitialDrag(bool allowInitialDrag)
        {
            isInitialDrag = allowInitialDrag;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isInitialDrag)
            {
                var newPos = ScreenToWorld(eventData.position, cachedData.position.z);
                newPos.z = cachedData.position.z;
                transform.localPosition = newPos;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isInitialDrag)
            {
                isInitialDrag = false;
            }
        }

        private Vector3 ScreenToWorld(Vector3 screenPos, float z)
        {
            screenPos.z = z - mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(screenPos);
        }
    }

}