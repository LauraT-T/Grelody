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

    private List<GameObject> createdSnowmen = new List<GameObject>();

    public void SpawnSnowman(int numberOfBalls, bool isHappy)
    {
        
        // Spawn position
        Vector3 spawnPosition = new Vector3(0, 0, 0);

        // Get snowman
        GameObject snowmanPrefab = GetSnowmanPrefab(numberOfBalls, isHappy);

        // Instantiate the snowman
        GameObject newSnowman = Instantiate(snowmanPrefab, spawnPosition, Quaternion.identity);

        // Add the snowman to list of created snowmen
        createdSnowmen.Add(newSnowman);
       
    }

    // Get the snowman prefab with correct facial expression and number of snowballs
    private GameObject GetSnowmanPrefab(int numberOfBalls, bool isHappy)
    {
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

}
