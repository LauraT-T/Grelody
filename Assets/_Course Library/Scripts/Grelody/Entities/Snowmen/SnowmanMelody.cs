using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanMelody
{

    // Snowman model
    private GameObject snowmanPrefab;

    // Corresponding melody
    private Melody melody;

    // Whether it was saved to the inventory
    private bool savedToInventory;

    public SnowmanMelody(GameObject snowmanPrefab, Melody melody) 
    {
        this.snowmanPrefab = snowmanPrefab;
        this.melody = melody;
        this.savedToInventory = false;
    }

    public SnowmanMelody(GameObject snowmanPrefab, Melody melody, bool savedToInventory) 
    {
        this.snowmanPrefab = snowmanPrefab;
        this.melody = melody;
        this.savedToInventory = savedToInventory;
    }

    public GameObject GetSnowmanPrefab()
    {
        return this.snowmanPrefab;
    }

    public Melody GetMelody()
    {
        return this.melody;
    }

    public bool GetSavedToInventory()
    {
        return this.savedToInventory;
    }
}
