using UnityEngine;

public class BackToInventoryCollision : MonoBehaviour
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
     private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Snowman")) { 

            Debug.Log("Collision snowman - backToInventory detected.");

            SnowmanInventoryManager inventoryManager = FindObjectOfType<SnowmanInventoryManager>();
            SnowmanMelody snowmanMelody = inventoryManager.FindSnowmanMelody(other.gameObject);

            if (snowmanMelody != null)
            {
                // Move the snowman to its saved inventory position
                other.gameObject.transform.localPosition = snowmanMelody.GetInventoryPosition();
                Debug.Log("Snowman moved back to inventory at position: " + snowmanMelody.GetInventoryPosition());

                // Set the parent GameObject (whole button consisting of two snowmen) inactive
                if (transform.parent != null) 
                {
                    transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("No parent found to deactivate.");
                }
            }
            else
            {
                Debug.Log("No corresponding SnowmanMelody found.");
            }
        }
        
    }
}
