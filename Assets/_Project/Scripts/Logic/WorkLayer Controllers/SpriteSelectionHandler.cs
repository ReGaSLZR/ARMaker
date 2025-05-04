using UnityEngine;
using System.Collections.Generic;

public class SpriteSelectionHandler : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color selectionColor = Color.blue;
    public float cornerDotSize = 0.1f;
    public float lineThickness = 0.05f;
    public GameObject dotPrefab; // A small circular sprite with SpriteRenderer

    private Camera mainCamera;
    private Transform selectedTransform;
    private LineRenderer lineRenderer;
    private List<GameObject> cornerDots = new List<GameObject>();
    private Vector3 offset;
    private bool dragging;

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize LineRenderer
        GameObject lineObj = new GameObject("SelectionBox");
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = selectionColor;
        lineRenderer.endColor = selectionColor;
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.loop = false;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            Vector3 inputPos = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(inputPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Select(sr.transform);
                    offset = selectedTransform.position - mainCamera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, mainCamera.WorldToScreenPoint(selectedTransform.position).z));
                    dragging = true;
                }
            }
        }

        if ((Input.GetMouseButton(0) || Input.touchCount > 0) && dragging && selectedTransform != null)
        {
            Vector3 inputPos = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
            Vector3 newPos = mainCamera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, mainCamera.WorldToScreenPoint(selectedTransform.position).z)) + offset;
            newPos.z = selectedTransform.position.z; // Keep original z
            selectedTransform.position = newPos;
            UpdateVisuals();
        }

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            dragging = false;
        }
    }

    void Select(Transform target)
    {
        selectedTransform = target;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (selectedTransform == null) return;

        Bounds bounds = selectedTransform.GetComponent<SpriteRenderer>().bounds;
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

        // Clear old dots
        foreach (GameObject dot in cornerDots)
            Destroy(dot);
        cornerDots.Clear();

        if (dotPrefab != null)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject dot = Instantiate(dotPrefab, corners[i], Quaternion.identity);
                dot.transform.localScale = Vector3.one * cornerDotSize;
                dot.GetComponent<SpriteRenderer>().color = selectionColor;
                cornerDots.Add(dot);
            }
        }
    }
}
