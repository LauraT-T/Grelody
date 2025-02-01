using System.Collections;
using UnityEngine;
using MidiPlayerTK;

public class ReplayMelody : MonoBehaviour
{
    public Transform vinylTransform; // The position where the snowman should move
    public float rotationSpeed = 100f; // Speed of rotation while playing
    private SnowmanInventoryManager inventoryManager;
    private IEnumerator spinCoroutine = null;
    private readonly Vector3 VINYL_POSITION = new Vector3(0.55f, 0.76f, -0.84f);


    private void Start()
    {
        // Find the inventory manager in the scene
        inventoryManager = FindObjectOfType<SnowmanInventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snowman"))
        {
            Debug.Log("Snowman collided with Grammophone!");

            // Find the corresponding SnowmanMelody in the inventory
            SnowmanMelody snowmanMelody = inventoryManager.FindSnowmanMelody(other.gameObject);

            if (snowmanMelody != null)
            {
                // Move the snowman to the vinyl
                other.transform.position = this.VINYL_POSITION;

                // Start playing the melody
                MidiStreamPlayer midiPlayer = FindObjectOfType<MidiStreamPlayer>();
                snowmanMelody.GetMelody().StartReplay(this, midiPlayer);

                // Start spinning the snowman
                if(this.spinCoroutine != null) {
                    StopCoroutine(this.spinCoroutine);
                }

                this.spinCoroutine = SpinSnowman(other.transform);
                StartCoroutine(SpinSnowman(other.transform));
            }
        }
    }

    private IEnumerator SpinSnowman(Transform snowman)
    {
        while (true)
        {
            if (snowman == null) yield break;
            snowman.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
