using System.Collections;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.XR.Interaction.Toolkit;


public class InstrumentCollision : MonoBehaviour
{

    private InstrumentManager instrumentManager;
    

    private void Start()
    {
        this.instrumentManager = (InstrumentManager)FindFirstObjectByType<InstrumentManager>();
        if(this.instrumentManager != null) {
            Debug.Log("InstrumentManager found");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Piano")) {
            Debug.Log("Piano collided with Grammophone");

        } else if (other.CompareTag("Guitar")) {
            Debug.Log("Guitar collided with Grammophone");

        } else if (other.CompareTag("Violin")) {
            Debug.Log("Violin collided with Grammophone");

        } else if (other.CompareTag("Trumpet")) {
            Debug.Log("Trumpet collided with Grammophone");

        } else if (other.CompareTag("Drums")) {
            Debug.Log("Drums collided with Grammophone");
        } 
    }
}
