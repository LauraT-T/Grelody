using System.Collections;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.XR.Interaction.Toolkit;


public class InstrumentCollision : MonoBehaviour
{

    private InstrumentManager instrumentManager;
    public GameObject invisibleInstrumentsParent;
    

    private void Start()
    {
        this.instrumentManager = (InstrumentManager)FindFirstObjectByType<InstrumentManager>();
        if(this.instrumentManager != null) {
            Debug.Log("InstrumentManager found");
        }
    }

    // On collison with game object, check tag and add instrument
    private void OnTriggerEnter(Collider other)
    {
        // Do not react to collisons with instruments already added and currently invisible
        if (other.transform.parent != null && other.transform.parent == this.invisibleInstrumentsParent.transform) {
            return;
        }

        if (other.CompareTag("Piano")) {
            Debug.Log("Piano collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.PIANO);

        } else if (other.CompareTag("Guitar")) {
            Debug.Log("Guitar collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.GUITAR);

        } else if (other.CompareTag("Violin")) {
            Debug.Log("Violin collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.STRINGS);

        } else if (other.CompareTag("Trumpet")) {
            Debug.Log("Trumpet collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.TRUMPET);

        } else if (other.CompareTag("Drums")) {
            Debug.Log("Drums collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.DRUMS);
        } 

        //StartCoroutine(ResetAfterDelay(5f));
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.instrumentManager.ResetVisibleInstruments();
    }
}
