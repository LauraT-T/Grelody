using UnityEngine;

/* 
Parent object to all invisible instruments added to the grammophone.
Moves in the same direction as the invisible cube used to pull out all instruments at once.
*/
public class InvisibleInstruments : MonoBehaviour
{
    public GameObject removeInstrumentsCube;
    private Vector3 lastPosition;

    void Start()
    {
        if (removeInstrumentsCube != null)
        {
            lastPosition = removeInstrumentsCube.transform.position;
        }
    }

    // Moving the invisible cube moves all added instruments (out of the grammophone)
    void Update()
    {
        if (removeInstrumentsCube != null) {
            Vector3 delta = removeInstrumentsCube.transform.position - lastPosition;
            // Move twice as fast in the x direction, so that removing the instruments is faster
            transform.position += new Vector3(delta.x * 2, delta.y, delta.z);
            lastPosition = removeInstrumentsCube.transform.position;
        }
    }
}
