using UnityEngine;
using UnityEngine.UI;
using TMPro;
// Karma's gonna come for all of us but I hope and I hope I just hope she comes, comes for you first
// Joni's on a juice cleanse, joni's got the whole room trained
// YOU WISH
// YOU WERE
// LIKE HER
// Hold onto hope if you got it and don't let it go for nobody
// They say that dreaming is free
// I wouldn't care what it cost me
public class LevelEditor : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs for placing
    public TextMeshProUGUI selectedPrefabText; // UI element to display the selected prefab
    public Material highlightMaterial; // Material to use for highlighting

    private GameObject currentPrefab; // Prefab selected from the UI
    private GameObject selectedObject; // Object currently selected for movement
    private Vector3 mousePosition; // Position of the mouse in the world space
    private Material originalMaterial; // Store the original material of the selected object

    private bool ObjectSelected;

    // Define the grid size (for example, 1 unit)
    public float gridSize = 1.0f;
    // Define the layer for the prefabs (set this in the Inspector)
    public LayerMask prefabLayer;

    void Start()
    {
        ObjectSelected = false;
        UpdateSelectedPrefabUI();
    }

    void Update()
    {
        HandleMousePosition();
        HandleInput();
    }

    // Convert mouse position to world position
    void HandleMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition = hit.point;
        }
    }

    // Handle user input for placing and selecting objects
    void HandleInput()
    {
        // Use Alpha keys to select prefabs
        if (Input.GetKeyDown(KeyCode.Alpha1) && prefabs.Length > 0) // Press 1 to select first prefab
        {
            currentPrefab = prefabs[0];
            DeselectObject(); // Deselect any currently selected object
            UpdateSelectedPrefabUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && prefabs.Length > 1) // Press 2 to select second prefab
        {
            currentPrefab = prefabs[1];
            DeselectObject(); // Deselect any currently selected object
            UpdateSelectedPrefabUI();
        }

        // Place object when left mouse button is clicked
        if (Input.GetMouseButtonDown(0) && currentPrefab != null) // Left mouse button to place objects
        {
            PlaceObject();
            DeselectObject();
            Debug.Log("Object Placed");
            ObjectSelected = false;
        }

        // Select object for movement when right mouse button is clicked
        if (Input.GetMouseButtonDown(1)) // Right mouse button to select an object
        {
            SelectObject();
            ObjectSelected = true;
        }

        // Move the selected object if we have one
        if (selectedObject != null)
        {
            MoveSelectedObject();
        }
    }

    // Method to place objects at the mouse position
    void PlaceObject()
    {
        if (currentPrefab != null)
        {
            Vector3 adjustedPosition = new Vector3(
                SnapToGrid(mousePosition.x),
                SnapToGrid(mousePosition.y),
                SnapToGrid(mousePosition.z)
            );

            Collider prefabCollider = currentPrefab.GetComponent<Collider>();
            if (prefabCollider != null)
            {
                // Check for overlap before placing and adjust Y if needed
                while (IsOverlapping(prefabCollider, adjustedPosition))
                {
                    adjustedPosition.y += prefabCollider.bounds.size.y + 0.5f; // Adjust Y position
                    Debug.Log("Raising object by Y to avoid overlap.");
                }

                // Instantiate the object at the adjusted position
                Instantiate(currentPrefab, adjustedPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Prefab does not have a collider. Unable to adjust placement.");
            }
        }
        else
        {
            Debug.LogWarning("Current prefab is null! Please select a prefab.");
        }
    }

    // Method to select an object with a right-click
    void SelectObject()
    {
        ObjectSelected = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, prefabLayer))
        {
            if (selectedObject != hit.collider.gameObject)
            {
                DeselectObject(); // Deselect the previous object

                selectedObject = hit.collider.gameObject; // Select the new object
                currentPrefab = null; // Deselect prefab to move the object

                // Highlight the selected object
                Renderer objectRenderer = selectedObject.GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    originalMaterial = objectRenderer.material; // Store the original material
                    objectRenderer.material = highlightMaterial; // Set the highlight material
                }

                // Log the selected object
                Debug.Log("Selected Object: " + selectedObject.name);
            }

            UpdateSelectedPrefabUI();
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }

    // Method to deselect the currently selected object and remove highlight
    void DeselectObject()
    {
        if (selectedObject != null)
        {
            Renderer objectRenderer = selectedObject.GetComponent<Renderer>();
            if (objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial; // Restore the original material
            }

            selectedObject = null;
            originalMaterial = null;
            Debug.Log("Deselected Object");
        }

    }

    // Method to move the selected object to the mouse position
    void MoveSelectedObject()
    {
        if (selectedObject != null)
        {
            Vector3 adjustedPosition = new Vector3(
                SnapToGrid(mousePosition.x),
                SnapToGrid(mousePosition.y),
                SnapToGrid(mousePosition.z)
            );
            
            selectedObject.transform.position = adjustedPosition;

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Destroy(selectedObject);
            }
        }
    }

    // Method to check for overlaps with stricter conditions
    private bool IsOverlapping(Collider prefabCollider, Vector3 position)
    {
        // Check if there are any colliders overlapping within a certain proximity
        Collider[] overlappingColliders = Physics.OverlapBox(position, prefabCollider.bounds.extents, Quaternion.identity, prefabLayer);

        foreach (Collider collider in overlappingColliders)
        {
            if (collider != prefabCollider)
            {
                Vector3 colliderPosition = collider.transform.position;

                // Check if the X and Z are the same
                bool xAndZSame = Mathf.Abs(colliderPosition.x - position.x) < 0.01f && Mathf.Abs(colliderPosition.z - position.z) < 0.01f;

                // If X and Z are the same, check if the Y is different enough (allow stacking with enough space)
                float yDifference = Mathf.Abs(colliderPosition.y - position.y);
                float minimumAllowedYDifference = prefabCollider.bounds.size.y + 1.0f;

                if (xAndZSame && yDifference < minimumAllowedYDifference)
                {
                    return true; // Overlapping detected
                }
            }
        }

        return false; // No overlapping detected
    }

    // Method to snap a value to the nearest grid size
    private float SnapToGrid(float value)
    {
        return Mathf.Round(value / gridSize) * gridSize; // Snap to the nearest grid point
    }

    // Update the UI to show the selected prefab or object
    void UpdateSelectedPrefabUI()
    {
        if (currentPrefab != null)
        {
            selectedPrefabText.text = "Selected Prefab: " + currentPrefab.name;
        }
        else if (selectedObject != null)
        {
            selectedPrefabText.text = "Selected Object: " + selectedObject.name;
        }
        else
        {
            selectedPrefabText.text = "No Prefab or Object Selected";
        }
    }
}
