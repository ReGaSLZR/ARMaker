using UnityEditor.Animations;
using UnityEngine;

namespace ARMarker
{

    [RequireComponent(typeof(BoxCollider))]
    public class WorkLayer : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeField]
        private GameObject statusLocked;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Animator animator;

        [Space]

        [SerializeField]
        private LayerRepositionXYHandler repositionHandler;

        [SerializeField]
        private LayerRotationHandler rotationHandler;

        [SerializeField]
        private LayerResizeXYHandler resizeHandler;

        public WorkLayerData Data => cachedData;

        private WorkLayerData cachedData;
        private BoxCollider boxCollider;

        private bool isLocked;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            statusLocked.SetActive(false);
        }

        private void Start()
        {
            repositionHandler.RegisterListener(OnSelectLayer);
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
                        repositionHandler.enabled = true;

                        rotationHandler.Deselect();
                        resizeHandler.Deselect();

                        rotationHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Rotate:
                    {
                        rotationHandler.enabled = true;

                        repositionHandler.Deselect();
                        resizeHandler.Deselect();

                        repositionHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Resize:
                    {
                        resizeHandler.enabled = true;

                        repositionHandler.Deselect();
                        rotationHandler.Deselect();

                        repositionHandler.enabled = false;
                        rotationHandler.enabled = false;
                        break;
                    }
            }
        }

        private void OnSelectLayer()
        {
            WorkSpaceSingleton.Instance.ChangeActiveLayer(this);

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
                        repositionHandler.Deselect();
                        resizeHandler.Deselect();

                        repositionHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Resize:
                    {
                        repositionHandler.Deselect();
                        rotationHandler.Deselect();

                        repositionHandler.enabled = false;
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
        }

        public void SetUpAnimator(AnimatorController controller)
        {
            if (controller == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"controller is null!", gameObject);
                return;
            }

            cachedData.controller = controller;
            animator.runtimeAnimatorController = controller;
        }

        public void ToggleLockState()
        {
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
            repositionHandler.Deselect();
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

    }

}