using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this at the top to use TextMeshPro

public class LevelEditor : MonoBehaviour
{
    public GameObject[] prefabs;
    public TextMeshProUGUI selectedPrefabText;

    private GameObject currentPrefab;
    private Vector3 mousePosition;
    private bool isPlacing = false; // Flag to determine if we are placing an object

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
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press 1 to select first prefab
        {
            if (prefabs.Length > 0)
            {
                currentPrefab = prefabs[0];
                UpdateSelectedPrefabUI();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Press 2 to select second prefab
        {
            if (prefabs.Length > 1)
            {
                currentPrefab = prefabs[1];
                UpdateSelectedPrefabUI();
            }
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
            // Instantiate the object at the mouse position with the current rotation
            Instantiate(currentPrefab, mousePosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Current prefab is null! Please select a prefab.");
        }
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
