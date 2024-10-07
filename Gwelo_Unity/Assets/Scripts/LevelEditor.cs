using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this at the top to use TextMeshPro

public class LevelEditor : MonoBehaviour
{
    public GameObject[] prefabs;
    public TextMeshProUGUI selectedPrefabText; // Use TextMeshProUGUI instead of Text

    private GameObject currentPrefab;
    private Vector3 mousePosition;

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
        if (Input.GetMouseButtonDown(0)) // Left mouse button to place objects
        {
            PlaceObject();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press 1 to select first prefab
        {
            currentPrefab = prefabs[0];
            UpdateSelectedPrefabUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Press 2 to select second prefab
        {
            currentPrefab = prefabs[1];
            UpdateSelectedPrefabUI();
        }
        // Add more keys for additional prefabs if necessary.
    }

    // Method to place objects at the mouse position
    void PlaceObject()
    {
        if (currentPrefab != null)
        {
            Instantiate(currentPrefab, mousePosition, Quaternion.identity);
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
