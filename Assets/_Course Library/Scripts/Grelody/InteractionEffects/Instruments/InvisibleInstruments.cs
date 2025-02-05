using UnityEngine;

/*
Parent object to all invisible instruments added to the grammophone.
By moving the invisible cube which the script RemoveInstruments is attached to,
the parent object moves along and makes it possible to remove the instruments from the grammophone.
*/
public class InvisibleInstruments : MonoBehaviour
{
    public GameObject removeInstrumentsCube;
    private Vector3 lastPosition;
    private bool isBeingMoved = false; // Flag to check if cube is being moved intentionally

    void Start()
    {
        if (removeInstrumentsCube != null)
        {
            lastPosition = removeInstrumentsCube.transform.position;
        }
    }

    void Update()
    {
        if (removeInstrumentsCube != null)
        {
            Vector3 currentPosition = removeInstrumentsCube.transform.position;
            Vector3 delta = currentPosition - lastPosition;

            // Only move the instruments if the cube is being moved by the player, not when jumping into the player's hand
            if (isBeingMoved)
            {
                transform.position += new Vector3(delta.x * 2, delta.y, delta.z);
            }

           lastPosition = currentPosition;
        }
    }

    public void EnableMovement()
    {
        isBeingMoved = true;
    }

    public void StopMovement()
    {
        isBeingMoved = false;
    }
}

