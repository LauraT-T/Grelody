using System.Collections;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.XR.Interaction.Toolkit;


public class InstrumentCollision : MonoBehaviour
{

    public AudioSource audioSource;
    private InstrumentManager instrumentManager;
    public GameObject invisibleInstrumentsParent;
    

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
        }

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

        // Sounds get individually played when an instrument gets added to avoid bug where sound always plays while creating melody
         // Play sound
        /* if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        } */

        if (other.CompareTag("Piano")) {
            Debug.Log("Piano collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.PIANO);
            audioSource.Play();

        } else if (other.CompareTag("Guitar")) {
            Debug.Log("Guitar collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.GUITAR);
            audioSource.Play();

        } else if (other.CompareTag("Violin")) {
            Debug.Log("Violin collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.STRINGS);
            audioSource.Play();

        } else if (other.CompareTag("Trumpet")) {
            Debug.Log("Trumpet collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.TRUMPET);
            audioSource.Play();

        } else if (other.CompareTag("Drums")) {
            Debug.Log("Drums collided with Grammophone");
            this.instrumentManager.AddInstrumentToGrammophone(InstrumentType.DRUMS);
            audioSource.Play();
        } 
    }
}
