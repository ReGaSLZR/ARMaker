using UnityEngine;
using UnityEngine.EventSystems;

public class LayerResizeXYHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Customization")]
    public Color selectionBoxColor = Color.blue;
    public float lineThickness = 2f;
    public float cornerDotSize = 0.1f; // Size of the corner dots
    public float cornerDetectionRadius = 0.1f; // Radius for corner hit detection
    public GameObject cornerDotPrefab; // The prefab for the corner dots

    private GameObject selectedObject;
    private Vector2 originalMousePosition;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector3[] corners = new Vector3[4]; // To store the corner positions
    private LineRenderer lineRenderer;
    private bool isResizing = false;
    private int selectedCorner = -1;  // Tracks which corner is selected

    private GameObject[] cornerDots = new GameObject[4]; // Array to store the corner dot GameObjects

    private void Awake()
    {
        // Set up LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.positionCount = 5;  // 4 corners + 1 to close the rectangle

        // Initialize the corner dots
        for (int i = 0; i < 4; i++)
        {
            cornerDots[i] = Instantiate(cornerDotPrefab, transform); // Instantiate the prefab as a child
            cornerDots[i].SetActive(false); // Initially hidden, will show when selected
            cornerDots[i].transform.localScale = Vector3.one * cornerDotSize; // Set size of the dot
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            selectedObject = hit.collider.gameObject;
            SpriteRenderer sr = selectedObject.GetComponent<SpriteRenderer>();

            // Get the bounds of the sprite and calculate the world corners
            if (sr != null)
            {
                Bounds spriteBounds = sr.bounds;

                // Calculate the four corners based on the sprite's bounds
                corners[0] = spriteBounds.max; // Top-right
                corners[1] = new Vector3(spriteBounds.min.x, spriteBounds.max.y, spriteBounds.max.z); // Top-left
                corners[2] = spriteBounds.min; // Bottom-left
                corners[3] = new Vector3(spriteBounds.max.x, spriteBounds.min.y, spriteBounds.max.z); // Bottom-right

                originalMousePosition = eventData.position;
                originalScale = selectedObject.transform.localScale;
                originalPosition = selectedObject.transform.position;

                // Check if the user clicked on a corner to resize it
                selectedCorner = -1;
                for (int i = 0; i < 4; i++)
                {
                    if (Vector2.Distance(eventData.position, Camera.main.WorldToScreenPoint(corners[i])) < cornerDetectionRadius * 100) // Adjust sensitivity
                    {
                        selectedCorner = i;  // Track which corner is selected
                        break;
                    }
                }

                // If no corner is selected, allow resizing uniformly
                if (selectedCorner == -1)
                {
                    selectedCorner = -1;
                }

                UpdateSelectionBox();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectedObject == null) return;

        // Convert screen position to world position
        Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 delta = newMousePosition - Camera.main.ScreenToWorldPoint(originalMousePosition);

        // Only resize the sprite if a corner is selected
        if (selectedCorner != -1)
        {
            Vector3 newScale = originalScale;

            switch (selectedCorner)
            {
                case 0: // Top-right corner
                    newScale.x = originalScale.x + delta.x;
                    newScale.y = originalScale.y + delta.y;
                    break;

                case 1: // Top-left corner
                    newScale.x = originalScale.x - delta.x;
                    newScale.y = originalScale.y + delta.y;
                    break;

                case 2: // Bottom-left corner
                    newScale.x = originalScale.x - delta.x;
                    newScale.y = originalScale.y - delta.y;
                    break;

                case 3: // Bottom-right corner
                    newScale.x = originalScale.x + delta.x;
                    newScale.y = originalScale.y - delta.y;
                    break;
            }

            // Preserve the z-axis
            newScale.z = originalScale.z;

            // Apply new scale
            selectedObject.transform.localScale = newScale;

            // Update selection box and corner dots
            UpdateSelectionBox();
        }
        else
        {
            // Uniform resizing (both x and y axes) when no corner is selected
            Vector3 newScale = originalScale;

            // Calculate the scaling factor based on mouse movement
            float scaleFactor = delta.x * 0.1f; // This can be adjusted for sensitivity

            // Uniformly resize in both x and y axes
            newScale.x = originalScale.x + scaleFactor;
            newScale.y = originalScale.y + scaleFactor;

            // Ensure scale doesn't go negative
            newScale.x = Mathf.Max(newScale.x, 0.1f);
            newScale.y = Mathf.Max(newScale.y, 0.1f);

            // Preserve the z-axis
            newScale.z = originalScale.z;

            // Apply the uniform new scale
            selectedObject.transform.localScale = newScale;

            // Update selection box and corner dots
            UpdateSelectionBox();
        }
    }

    private void UpdateSelectionBox()
    {
        if (selectedObject == null) return;

        SpriteRenderer sr = selectedObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Bounds spriteBounds = sr.bounds;

            // Calculate the four corners based on the sprite's bounds
            corners[0] = spriteBounds.max; // Top-right
            corners[1] = new Vector3(spriteBounds.min.x, spriteBounds.max.y, spriteBounds.max.z); // Top-left
            corners[2] = spriteBounds.min; // Bottom-left
            corners[3] = new Vector3(spriteBounds.max.x, spriteBounds.min.y, spriteBounds.max.z); // Bottom-right
        }

        // Update LineRenderer to draw selection box
        for (int i = 0; i < 4; i++)
        {
            lineRenderer.SetPosition(i, corners[i]);
            // Position corner dots at each corner
            cornerDots[i].transform.position = corners[i];
            cornerDots[i].SetActive(true); // Make the corner dots visible when selected
        }

        // Close the rectangle by setting the last point to the first one
        lineRenderer.SetPosition(4, corners[0]);

        // Optionally, make corner dots invisible when deselected
        if (selectedObject == null)
        {
            foreach (var dot in cornerDots)
            {
                dot.SetActive(false);
            }
        }
    }
}
