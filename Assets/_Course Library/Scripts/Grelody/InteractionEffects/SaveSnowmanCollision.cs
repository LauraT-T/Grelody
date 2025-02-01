using UnityEngine;

public class SaveSnowmanCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Save snowman to inventory when snowman is entering inventory button
    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Snowman")) { 
            SnowmanManager snowmanManager = (SnowmanManager)FindFirstObjectByType<SnowmanManager>();
            snowmanManager.SaveSnowman();
            Debug.Log("Collion detected. Snowman saved to inventory.");
        }
    }
}
