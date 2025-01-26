using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowflakeManager : MonoBehaviour
{
    public GameObject snowflakeLowDetail;
    public GameObject snowflakeMediumDetail;
    public GameObject snowflakeHighDetail;
    public GameObject grammophone;

    private List<GameObject> activeSnowflakes = new List<GameObject>();

    // If there would be less than this many snowflakes in the scene, the snowflakes do not disappear
    private const int MIN_SNOWFLAKE_NUMBER = 20;

    public void SpawnSnowflake(DetailDegree detailDegree, Color snowflakeColor)
    {

        // Calculate spawn position
        Vector3 spawnPosition = CalculateSpawnPosition();

        // Get snowflake with passed detailDegree
        GameObject snowflakePrefab = GetSnowflakePrefab(detailDegree);

        // Instantiate the snowflake
        GameObject newSnowflake = Instantiate(snowflakePrefab, spawnPosition, Quaternion.identity);

        // Find the child object with the MeshRenderer
        MeshRenderer renderer = newSnowflake.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Debug.Log("Renderer was found in child object");
            renderer.material.color = snowflakeColor;
        }
        else
        {
            Debug.Log("Renderer is null, check prefab structure.");
        }

        // Add the snowflake to active list
        activeSnowflakes.Add(newSnowflake);

        // Remove snowflakes after a set duration to prevent clutter
        StartCoroutine(RemoveSnowflakeAfterTime(newSnowflake, 10f));
    }

    // Get the snowflake prefab corresponding to the detail level the snowflake is supposed to have
    private GameObject GetSnowflakePrefab(DetailDegree detailLevel)
    {
        switch (detailLevel)
        {
            case DetailDegree.LOW: return snowflakeLowDetail;
            case DetailDegree.MEDIUM: return snowflakeMediumDetail;
            case  DetailDegree.HIGH: return snowflakeHighDetail;
            default: return snowflakeLowDetail;
        }
    }

    // Remove the snowflake from the scene to avoid cluttering
    private IEnumerator RemoveSnowflakeAfterTime(GameObject snowflake, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (snowflake != null && this.activeSnowflakes.Count > MIN_SNOWFLAKE_NUMBER)
        {
            this.activeSnowflakes.Remove(snowflake);
            Destroy(snowflake);
        }
    }

    // Calculates the spawn position based on the position of the grammophone
    private Vector3 CalculateSpawnPosition()
    {
        // Get position of grammophone
    	Vector3 grammophonePosition = grammophone.transform.position;
        
        // Get the grammophone height
        MeshRenderer meshRenderer = grammophone.GetComponent<MeshRenderer>();
        float grammophoneHeight = meshRenderer.bounds.size.y;

        // Spawn position is at the top of the grammophone
        return grammophone.transform.position + new Vector3(0, grammophoneHeight, 0);
    }
}
