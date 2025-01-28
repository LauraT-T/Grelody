using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanManager : MonoBehaviour
{

    // Sad snowmen with one, two and three snowballs
    public GameObject snowmanSadOne;
    public GameObject snowmanSadTwo;
    public GameObject snowmanSadTree;

    // Happy snowmen with one, two and three snowballs
    public GameObject snowmanHappyOne;
    public GameObject snowmanHappyTwo;
    public GameObject snowmanHappyThree;

    public GameObject grammophone;

    // Scale depends on length of created melody
    private const float MIN_SCALE = 0.15f;
    private const float MAX_SCALE = 0.35f;

    // List of all created and saved melodies with their corresponding snowman model
    private List<SnowmanMelody> createdSnowmen = new List<SnowmanMelody>();

    /*
    Makes a snowman appear in the scene.
    Is passed the created melody,
    the number of snowballs the snowman has (reflects how many instruments were added)
    and whether the snowman is happy or sad (reflects whether the melody is mostly mejor or minor)
    */
    public void SpawnSnowman(int instrumentNumber, bool isHappy, Melody melody, int beatCount)
    {
        
        // Spawn position
        Vector3 spawnPosition = new Vector3(0, 0, 0);

        // Get snowman
        int numberOfBalls = CalculateSnowballNumber(instrumentNumber);
        GameObject snowmanPrefab = GetSnowmanPrefab(numberOfBalls, isHappy);

        // Determine scale factor based on melody length
        // Scale factor is between minimum and maximum scale factor
        float scaleFactor = Mathf.Clamp(MIN_SCALE + (beatCount * 0.001f), MIN_SCALE, MAX_SCALE); 
        Debug.Log($"Beat count: {beatCount} Snowman scale factor: {scaleFactor}");

        // Instantiate the snowman
        GameObject newSnowman = Instantiate(snowmanPrefab, spawnPosition, Quaternion.identity);
        newSnowman.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Add the snowman prefab together with the saved melody to list of created snowmen
        SnowmanMelody newSnowmanMelody = new SnowmanMelody(newSnowman, melody);
        createdSnowmen.Add(newSnowmanMelody);
       
    }

    // Get the snowman prefab with correct facial expression and number of snowballs
    private GameObject GetSnowmanPrefab(int numberOfBalls, bool isHappy)
    {
       Debug.Log($"Snowman created with {(isHappy ? "happy" : "sad")} mood and {numberOfBalls} snowballs");

        switch (numberOfBalls)
        {
            case 1: 
                if(isHappy) {
                    return snowmanHappyOne;
                } else {
                    return snowmanSadOne;
                }
            case 2: 
                if(isHappy) {
                    return snowmanHappyTwo;
                } else {
                    return snowmanSadTwo;
                }
            case 3: 
                if(isHappy) {
                    return snowmanHappyThree;
                } else {
                    return snowmanSadTree;
                }
            default: return snowmanHappyOne;
        }
    }

    /*
    Calculates number of snowballs (1-3) based on number of instruments which have been added to the melody
    */
    private int CalculateSnowballNumber(int instrumentNumber)
    {
        switch(instrumentNumber) 
        {
            case 0: return 1;
            case 1: return 1;
            case 2: return 2;
            case 3: return 3;
            default: return 3;
        }
    }

}
