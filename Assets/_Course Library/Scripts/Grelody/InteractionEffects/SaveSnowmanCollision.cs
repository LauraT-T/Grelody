using UnityEngine;

public class SaveSnowmanCollision : MonoBehaviour
{
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
        }
    }

    // Save snowman to inventory when snowman is entering inventory button
    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Snowman")) {

            // Play sound
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            SnowmanManager snowmanManager = (SnowmanManager)FindFirstObjectByType<SnowmanManager>();
            snowmanManager.SaveSnowman();
            Debug.Log("Collion detected. Snowman saved to inventory.");
        }
    }
}
