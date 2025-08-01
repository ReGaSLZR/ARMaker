using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{

    [RequireComponent(typeof(BoxCollider))]
    public class LayerRotationHandler : BaseWorkLayerHandler,
        IPointerDownHandler
    {

        [SerializeField]
        private GameObject rotationIndicators;

        [Space]

        [SerializeField]
        private GameObject xDial;
        [SerializeField]
        private GameObject yDial;
        [SerializeField]
        private GameObject zDial;

        [Space]

        [SerializeField]
        private BoxCollider boxCollider;

        [SerializeField]
        private Vector3 colliderSizeOnActivation;

        [Space]

        [SerializeField]
        private float rotationSensitivity = 0.4f;

        private Camera mainCam;
        private Transform selectedAxis;
        private Vector2 startTouchPos;
        private bool isDragging = false;

        private Vector3 rotationAxis;
        private Vector3 originalColliderSize;

        private void Awake()
        {
            mainCam = Camera.main;
            originalColliderSize = boxCollider.size;
        }

        private void OnDisable()
        {
            boxCollider.size = originalColliderSize;
        }

        private void Update()
        {
            if (!isDragging)
            {
                return;
            }

            if (Input.touchCount != 1)
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                var delta = touch.deltaPosition;
                var deltaRotation = delta.magnitude * rotationSensitivity;

                var direction = Vector2.Dot(delta, 
                    GetAxisScreenDirection(rotationAxis)) > 0 ? 1f : -1f;

                transform.Rotate(rotationAxis, direction * deltaRotation, Space.Self);
            }
            else if ((touch.phase == TouchPhase.Ended) 
                || (touch.phase == TouchPhase.Canceled))
            {
                ResetDragging();
            }
        }

        public override void Select()
        {
            base.Select();
            rotationIndicators.SetActive(true);
            boxCollider.size = colliderSizeOnActivation;
        }

        public override void Deselect()
        {
            base.Deselect();
            rotationIndicators.SetActive(false);
            boxCollider.size = originalColliderSize;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Ray ray = mainCam.ScreenPointToRay(eventData.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var hitObj = hit.collider.gameObject.GetHashCode();

                if (hitObj == xDial.GetHashCode())
                {
                    selectedAxis = xDial.transform;
                    rotationAxis = Vector3.right;
                }
                else if (hitObj == yDial.GetHashCode())
                {
                    selectedAxis = yDial.transform;
                    rotationAxis = Vector3.up;
                }
                else if (hitObj == zDial.GetHashCode())
                {
                    selectedAxis = zDial.transform;
                    rotationAxis = Vector3.forward;
                }
                else
                {
                    Select();
                    return;
                }

                startTouchPos = eventData.position;
                isDragging = true;
            }
            else
            {
                Select();
            }
        }

        private void ResetDragging()
        {
            isDragging = false;
            selectedAxis = null;
            rotationAxis = Vector3.zero;
        }

        private Vector2 GetAxisScreenDirection(Vector3 worldAxis)
        {
            var screenStart = mainCam.WorldToScreenPoint(transform.position);
            var screenEnd = mainCam.WorldToScreenPoint(
                transform.position + transform.TransformDirection(worldAxis));
            return (screenEnd - screenStart).normalized;
        }

    }

}