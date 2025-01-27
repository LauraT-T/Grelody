using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanMelody
{

    // Snowman model
    private GameObject snowmanPrefab;

    // Corresponding melody
    private Melody melody;

    public SnowmanMelody(GameObject snowmanPrefab, Melody melody) 
    {
        this.snowmanPrefab = snowmanPrefab;
        this.melody = melody;
    }

    public GameObject GetSnowmanPrefab()
    {
        return this.snowmanPrefab;
    }

    public Melody GetMelody()
    {
        return this.melody;
    }
}
