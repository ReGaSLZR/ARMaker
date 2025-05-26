using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{

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
        private RotationCircle xCircle, yCircle, zCircle;

        [SerializeField]
        private BoxCollider boxCollider;
        private Vector3 originalColliderSize;

        private RotationCircle activeCircle = null;


        private void Awake()
        {
            originalColliderSize = boxCollider.size;
        }

        private void Start()
        {

        }

        [ContextMenu("Select")]
        public override void Select()
        {
            base.Select();
            boxCollider.size = originalColliderSize * boxColliderSizeMultiplierOnSelection;
            SelectSprite(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
            //DetectCircleHit(eventData.position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
            //DetectCircleHit(eventData.position);
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

        void SelectSprite(GameObject sprite)
        {
            ClearCircles();
            selectedSprite = sprite;

            CreateCircles();
        }

        [ContextMenu("Deselect")]
        public override void Deselect()
        {
            boxCollider.size = originalColliderSize;
            ClearCircles();
            base.Deselect();
        }

        void CreateCircles()
        {
            xCircle = RotationCircle.Create(Axis.X, xAxisColor, circleRadius, circleThickness);
            yCircle = RotationCircle.Create(Axis.Y, yAxisColor, circleRadius, circleThickness);
            zCircle = RotationCircle.Create(Axis.Z, zAxisColor, circleRadius, circleThickness);

            xCircle.transform.SetParent(transform);
            yCircle.transform.SetParent(transform);
            zCircle.transform.SetParent(transform);

            UpdateCircles();
        }

        void UpdateCircles()
        {
            Vector3 pos = selectedSprite.transform.position;
            xCircle.UpdatePosition(pos, selectedSprite.transform);
            yCircle.UpdatePosition(pos, selectedSprite.transform);
            zCircle.UpdatePosition(pos, selectedSprite.transform);
        }

        void ClearCircles()
        {
            if (xCircle != null) DestroyImmediate(xCircle.gameObject);
            if (yCircle != null) DestroyImmediate(yCircle.gameObject);
            if (zCircle != null) DestroyImmediate(zCircle.gameObject);

            xCircle = null;
            yCircle = null;
            zCircle = null;
        }

        void DetectCircleHit(Vector2 screenPos)
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

        public enum Axis { X, Y, Z }

        class RotationCircle : MonoBehaviour
        {
            public Axis axis;
            private LineRenderer lr;
            private int segments = 64;
            private float radius;
            private float thickness;

            public static RotationCircle Create(Axis axis, Color color, float radius, float thickness)
            {
                var obj = new GameObject($"RotationCircle_{axis}");
                var circle = obj.AddComponent<RotationCircle>();
                circle.axis = axis;
                circle.radius = radius;
                circle.thickness = thickness;

                var lr = obj.AddComponent<LineRenderer>();
                circle.lr = lr;
                lr.useWorldSpace = false;
                lr.loop = true;
                lr.positionCount = circle.segments;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = lr.endColor = color;
                lr.startWidth = lr.endWidth = thickness;
                lr.sortingOrder = 100;

                return circle;
            }

            public void UpdatePosition(Vector3 worldPos, Transform spriteTransform)
            {
                transform.position = worldPos;

                Quaternion rotation = axis switch
                {
                    Axis.X => Quaternion.Euler(0, 90, 0),
                    Axis.Y => Quaternion.Euler(90, 0, 0),
                    Axis.Z => Quaternion.identity,
                    _ => Quaternion.identity,
                };
                transform.rotation = rotation;

                Vector3[] points = new Vector3[segments];
                for (int i = 0; i < segments; i++)
                {
                    float angle = 2 * Mathf.PI * i / segments;
                    points[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                }

                lr.SetPositions(points);
            }

            public float DistanceToScreen(Vector2 screenPos, Camera cam)
            {
                float closestDist = float.MaxValue;

                for (int i = 0; i < lr.positionCount; i++)
                {
                    Vector3 worldPoint = transform.TransformPoint(lr.GetPosition(i));
                    Vector2 screenPoint = cam.WorldToScreenPoint(worldPoint);
                    float dist = Vector2.Distance(screenPos, screenPoint);
                    if (dist < closestDist) closestDist = dist;
                }

                return closestDist;
            }
        }
    }

}