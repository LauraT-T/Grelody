using UnityEngine;

public class PreventMovement : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Reset the position and rotation if necessary to prevent floating menu buttons
        if (transform.position != initialPosition || transform.rotation != initialRotation)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }
}
