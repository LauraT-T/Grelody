using UnityEngine;

public class SnowflakeMovement : MonoBehaviour
{
    public float fallSpeed = 0.5f;   // Speed of vertical swaying
    public float swayAmount = 0.05f;  // How much it sways up and down
    public float moveLeftSpeed = 0.3f;  // Speed of moving left
    public float stopXPosition = -1.0f; // X position where movement stops

    private Vector3 initialPosition;
    private bool hasStopped = false;

    void Start()
    {
        // Store the initial position for the sway effect
        initialPosition = transform.position;

        // Randomize values to make snowflakes look different
        fallSpeed = Random.Range(0.3f, 0.7f);
        swayAmount = Random.Range(0.3f, 0.8f);
        moveLeftSpeed = Random.Range(0.1f, 0.3f);
    }

    void Update()
    {
        // Up and down swaying effect using a sine wave
        float newY = initialPosition.y + Mathf.Sin(Time.time * fallSpeed) * swayAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Move left until the stopping position is reached
        if (transform.position.x > stopXPosition)
        {
            transform.Translate(Vector3.left * moveLeftSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Stop moving left after reaching the stop position
            hasStopped = true;
        }
    }
}
