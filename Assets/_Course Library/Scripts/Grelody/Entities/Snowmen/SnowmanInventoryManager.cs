using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanInventoryManager : MonoBehaviour
{
    public GameObject inventory; // Panel containing the grid
    public Transform inventoryGrid; // Parent object for the snowmen in the inventory
    public Button openInventoryButton; // Opens and closes inventory
    public Vector3 gridStartPosition = Vector3.zero;
    public float rowItemSpacing = 30.0f; // Horizontal gap between snowmen
    public float columnItemSpacing = 20.0f; // Vertical gap between snowmen
    public int columns = 4;

    private int gridIndex = 0;
    private List<SnowmanMelody> savedSnowmen = new List<SnowmanMelody>();



    public void Start()
    {
        openInventoryButton.onClick.AddListener(OpenCloseInventory);
    }

    // Saves the snowman to the inventory
    public void SaveSnowman(SnowmanMelody snowmanMelody)
    {
        // Add snowman to the inventory (creating a new SnowmanMelody, so that the melody is found for replay)
        GameObject inventorySnowman = Instantiate(snowmanMelody.GetSnowmanPrefab(), inventoryGrid);
        Vector3 inventoryPosition = GetNextInventoryPosition();
        SnowmanMelody newSnowmanMelody = new SnowmanMelody(inventorySnowman, snowmanMelody.GetMelody(), true, inventoryPosition);
        savedSnowmen.Add(newSnowmanMelody);

        // Adapt scale and position
        inventorySnowman.transform.localScale = inventorySnowman.transform.localScale * 25.0f;
        inventorySnowman.transform.localPosition = inventoryPosition;
    }

    private Vector3 GetNextInventoryPosition()
    {
        int row = gridIndex / columns;
        int column = gridIndex % columns;
        gridIndex++;
        return gridStartPosition + new Vector3(column * columnItemSpacing, -row * rowItemSpacing, 0);
    }

    // Shows inventory if not visible and hides inventory if visible
    private void OpenCloseInventory()
    {
        inventory.SetActive(!inventory.activeSelf);
    }

    // Find the SnowmanMelody corresponding to the passed snowman game object
    public SnowmanMelody FindSnowmanMelody(GameObject snowmanObject)
    {
        foreach (SnowmanMelody snowmanMelody in savedSnowmen)
        {
            if (snowmanMelody.GetSnowmanPrefab() == snowmanObject)
            {
                return snowmanMelody;
            }
        }
        Debug.Log("No SnowmanMelody for this game object found.");
        return null;
    }
}
