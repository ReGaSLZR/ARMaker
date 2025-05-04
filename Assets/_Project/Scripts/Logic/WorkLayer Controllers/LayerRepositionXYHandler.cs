using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace ARMarker
{

    public class LayerRepositionXYHandler : BaseWorkLayerHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [Header("Visual Settings")]
        public Color selectionColor = Color.blue;
        public float cornerDotSize = 0.1f;
        public float lineThickness = 0.05f;
        public GameObject dotPrefab; // Assign a prefab with a SpriteRenderer

        private Camera mainCamera;
        //private Transform selectedTransform;
        private LineRenderer lineRenderer;
        private List<GameObject> cornerDots = new List<GameObject>();
        private Vector3 offset;
        private bool dragging;

        void Awake()
        {
            mainCamera = Camera.main;

            GameObject lineObj = new GameObject("SelectionBox");
            lineObj.transform.parent = transform;
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 5;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = lineThickness;
            lineRenderer.endWidth = lineThickness;
            lineRenderer.startColor = selectionColor;
            lineRenderer.endColor = selectionColor;
            lineRenderer.loop = false;
            lineRenderer.enabled = false;
        }

        private void Start()
        {
            
        }

        [ContextMenu("Select")]
        public override void Select()
        {
            base.Select();
            //TrySelectAtScreenPoint(Vector2.zero);
            UpdateVisuals();
        }

        [ContextMenu("Deselect")]
        public override void Deselect()
        {
            lineRenderer.enabled = false;

            foreach (GameObject dot in cornerDots)
            {
                Destroy(dot);
            }

            cornerDots.Clear();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
            //base.Select();
            //TrySelectAtScreenPoint(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //if (selectedTransform != null)
            //{
                Vector3 inputPos = eventData.position;
                offset = transform.position - ScreenToWorld(inputPos, transform.position.z);
                dragging = true;
            //}
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                Vector3 newPos = ScreenToWorld(eventData.position, transform.position.z) + offset;
                newPos.z = transform.position.z;
                transform.position = newPos;
                UpdateVisuals();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }

        //private void TrySelectAtScreenPoint(Vector2 screenPoint)
        //{

        //    //Ray ray = mainCamera.ScreenPointToRay(screenPoint);
        //    //if (Physics.Raycast(ray, out RaycastHit hit))
        //    //{
        //    //    SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
        //    //    if (sr != null)
        //    //    {
        //    //        selectedTransform = sr.transform;
        //            UpdateVisuals();
        //        //}
        //    //}
        //}

        private void UpdateVisuals()
        {
            //if (selectedTransform == null) return;

            Bounds bounds = transform.GetComponent<SpriteRenderer>().bounds;
            Vector3[] corners = new Vector3[5]
            {
            bounds.min,
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            bounds.max,
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            bounds.min
            };

            lineRenderer.enabled = true;
            lineRenderer.positionCount = corners.Length;
            lineRenderer.startWidth = lineThickness;
            lineRenderer.endWidth = lineThickness;
            lineRenderer.startColor = selectionColor;
            lineRenderer.endColor = selectionColor;
            lineRenderer.SetPositions(corners);

            foreach (GameObject dot in cornerDots)
                Destroy(dot);
            cornerDots.Clear();

            if (dotPrefab != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameObject dot = Instantiate(dotPrefab, corners[i], Quaternion.identity);
                    dot.transform.SetParent(transform);
                    dot.transform.localScale = Vector3.one * cornerDotSize;
                    dot.GetComponent<SpriteRenderer>().color = selectionColor;
                    cornerDots.Add(dot);
                }
            }
        }

        private Vector3 ScreenToWorld(Vector3 screenPos, float z)
        {
            screenPos.z = z - mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(screenPos);
        }

    }

}