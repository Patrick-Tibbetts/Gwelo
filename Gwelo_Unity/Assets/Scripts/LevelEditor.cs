using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEditor : MonoBehaviour
{
    public GameObject[] prefabs;
    public TextMeshProUGUI selectedPrefabText;

    private GameObject currentPrefab;
    private Vector3 mousePosition;

    // Define the grid size (for example, 1 unit)
    public float gridSize = 2f;
    // Define the layer for the prefabs (set this in the Inspector)
    public LayerMask prefabLayer;

    void Start()
    {
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

    // Handle user input for placing objects
    void HandleInput()
    {
        // Use Alpha keys to select prefabs
        if (Input.GetKeyDown(KeyCode.Alpha1) && prefabs.Length > 0) // Press 1 to select first prefab
        {
            currentPrefab = prefabs[0];
            UpdateSelectedPrefabUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && prefabs.Length > 1) // Press 2 to select second prefab
        {
            currentPrefab = prefabs[1];
            UpdateSelectedPrefabUI();
        }

        // Place object when left mouse button is clicked
        if (Input.GetMouseButtonDown(0) && currentPrefab != null) // Left mouse button to place objects
        {
            PlaceObject();
        }
    }

    // Method to place objects at the mouse position
    void PlaceObject()
    {
        if (currentPrefab != null)
        {
            // Create an instance of the prefab to calculate its height
            GameObject prefabInstance = Instantiate(currentPrefab);

            // Get the collider of the prefab to determine its bounds
            Collider prefabCollider = prefabInstance.GetComponent<Collider>();
            if (prefabCollider != null)
            {
                // Adjust the Y position based on the prefab's collider
                float yOffset = prefabCollider.bounds.extents.y; // Half the height of the prefab

                // Snap the position to the grid, and adjust the Y to place the bottom of the prefab on the ground
                Vector3 adjustedPosition = new Vector3(
                    SnapToGrid(mousePosition.x), // Snap X to grid
                    SnapToGrid(mousePosition.y + yOffset), // Adjust Y by the height of the prefab
                    SnapToGrid(mousePosition.z)  // Snap Z to grid
                );

                // Destroy the temporary instance used for height calculation
                Destroy(prefabInstance);

                // Check for overlap at the snapped position
                if (!IsOverlapping(prefabCollider, adjustedPosition))
                {
                    // Instantiate the prefab at the calculated position if no overlap
                    Instantiate(currentPrefab, adjustedPosition, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("Cannot place prefab here; it would overlap with another object.");
                }
            }
            else
            {
                Debug.LogWarning("Prefab does not have a collider. Unable to adjust placement.");
                Destroy(prefabInstance); // Destroy the temporary instance
            }
        }
        else
        {
            Debug.LogWarning("Current prefab is null! Please select a prefab.");
        }
    }

    // Method to check for overlaps using the prefab's bounds
    private bool IsOverlapping(Collider prefabCollider, Vector3 position)
    {
        // Use the collider bounds to create the check box size, with a small margin to account for snapping accuracy
        Vector3 boxSize = prefabCollider.bounds.size / 2; // Half extents

        // Use Physics.OverlapBox to check for any colliders at the intended placement position
        Collider[] colliders = Physics.OverlapBox(position, boxSize, Quaternion.identity, prefabLayer);

        // If there are any colliders found that aren't the ground or itself, it means there's overlap
        return colliders.Length > 0;
    }

    // Method to snap a value to the nearest grid size
    private float SnapToGrid(float value)
    {
        return Mathf.Round(value / gridSize) * gridSize; // Snap to the nearest grid point
    }

    // Update the UI to show the selected prefab
    void UpdateSelectedPrefabUI()
    {
        if (currentPrefab != null)
        {
            selectedPrefabText.text = "Selected Prefab: " + currentPrefab.name;
        }
        else
        {
            selectedPrefabText.text = "No Prefab Selected";
        }
    }
}
