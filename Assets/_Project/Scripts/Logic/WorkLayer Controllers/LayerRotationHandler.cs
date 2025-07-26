using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{

    /// <summary>
    /// NOTE: Please refine and revamp this code soon. 
    /// Right now, it's at this dirty/prototype state to save time for prototyping.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class LayerRotationHandler : BaseWorkLayerHandler, 
        IPointerDownHandler, IDragHandler, IBeginDragHandler, IPointerClickHandler
    {

        [SerializeField]
        private float boxColliderSizeMultiplierOnSelection = 2f;

        [Space]

        [SerializeField]
        private float circleRadius = 1f;
        [SerializeField]
        private float circleThickness = 0.02f;
        [SerializeField]
        private float dragSensitivity = 1f;

        [Space]

        [SerializeField]
        private Color xAxisColor = Color.red;
        [SerializeField]
        private Color yAxisColor = Color.green;
        [SerializeField]
        private Color zAxisColor = Color.blue;

        private GameObject selectedSprite;
        private LayerRotationCircleView xCircle, yCircle, zCircle;

        [SerializeField]
        private BoxCollider boxCollider;
        private Vector3 originalColliderSize;

        private LayerRotationCircleView activeCircle = null;

        private void Awake()
        {
            originalColliderSize = boxCollider.size;
        }

        public override void Select()
        {
            base.Select();
            boxCollider.size = originalColliderSize * boxColliderSizeMultiplierOnSelection;
            SelectSprite(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            DetectCircleHit(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta * dragSensitivity;
            float angleDelta = delta.x + delta.y;

            if (activeCircle == null)
            {
                activeCircle = zCircle;
            }

            switch (activeCircle.axis)
            {
                case Axis.X:
                    selectedSprite.transform.Rotate(Vector3.right, angleDelta, Space.World);
                    break;
                case Axis.Y:
                    selectedSprite.transform.Rotate(Vector3.up, angleDelta, Space.World);
                    break;
                case Axis.Z:
                default:
                    selectedSprite.transform.Rotate(Vector3.forward, angleDelta, Space.Self);
                    break;
            }

            UpdateCircles();
        }

        private void SelectSprite(GameObject sprite)
        {
            ClearCircles();
            selectedSprite = sprite;

            CreateCircles();
        }

        public override void Deselect()
        {
            boxCollider.size = originalColliderSize;
            ClearCircles();
            base.Deselect();
        }

        private void CreateCircles()
        {
            xCircle = LayerRotationCircleView.Create(Axis.X, xAxisColor, circleRadius, circleThickness);
            yCircle = LayerRotationCircleView.Create(Axis.Y, yAxisColor, circleRadius, circleThickness);
            zCircle = LayerRotationCircleView.Create(Axis.Z, zAxisColor, circleRadius, circleThickness);

            xCircle.transform.SetParent(transform);
            yCircle.transform.SetParent(transform);
            zCircle.transform.SetParent(transform);

            UpdateCircles();
        }

        private void UpdateCircles()
        {
            Vector3 pos = selectedSprite.transform.position;
            xCircle.UpdatePosition(pos, selectedSprite.transform);
            yCircle.UpdatePosition(pos, selectedSprite.transform);
            zCircle.UpdatePosition(pos, selectedSprite.transform);
        }

        private void ClearCircles()
        {
            if (xCircle != null) DestroyImmediate(xCircle.gameObject);
            if (yCircle != null) DestroyImmediate(yCircle.gameObject);
            if (zCircle != null) DestroyImmediate(zCircle.gameObject);

            xCircle = null;
            yCircle = null;
            zCircle = null;
        }

        private void DetectCircleHit(Vector2 screenPos)
        {
            activeCircle = null;

            float bestDistance = float.MaxValue;
            foreach (var circle in new[] { xCircle, yCircle })
            {
                float distance = circle.DistanceToScreen(screenPos, Camera.main);
                if (distance < 30f && distance < bestDistance)
                {
                    bestDistance = distance;
                    activeCircle = circle;
                }
            }

            // If no X or Y circle is hit, default to Z-axis rotation
            if (activeCircle == null)
            {
                activeCircle = zCircle;
            }
        }
      
    }

}