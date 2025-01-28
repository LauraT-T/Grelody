using System.Collections.Generic;
using UnityEngine;

public class SnowmanInventoryManager : MonoBehaviour
{
    public Transform inventoryGrid; // Parent object for the snowmen in the inventory
    public Vector3 gridStartPosition = Vector3.zero;
    public float rowItemSpacing = 30.0f; // Horizontal gap between snowmen
    public float columnItemSpacing = 20.0f; // Vertical gap between snowmen
    public int columns = 4;

    private int gridIndex = 0;
    private List<SnowmanMelody> savedSnowmen = new List<SnowmanMelody>();

    // Saves the snowman to the inventory
    public void SaveSnowman(SnowmanMelody snowmanMelody)
    {
        // Add snowman to the inventory
        savedSnowmen.Add(snowmanMelody);
        GameObject inventorySnowman = Instantiate(snowmanMelody.GetSnowmanPrefab(), inventoryGrid);

        // Adapt scale and position
        inventorySnowman.transform.localScale = inventorySnowman.transform.localScale * 75.0f;
        inventorySnowman.transform.localPosition = GetNextInventoryPosition();
    }

    private Vector3 GetNextInventoryPosition()
    {
        int row = gridIndex / columns;
        int column = gridIndex % columns;
        gridIndex++;
        return gridStartPosition + new Vector3(column * columnItemSpacing, -row * rowItemSpacing, 0);
    }
}
