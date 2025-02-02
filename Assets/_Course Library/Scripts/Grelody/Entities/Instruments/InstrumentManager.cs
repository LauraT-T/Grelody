using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.InputSystem;

public class InstrumentManager : MonoBehaviour
{
    public GameObject instrumentMenu;
    public GameObject piano;
    public GameObject guitar;
    public GameObject violin;
    public GameObject trumpet;
    public GameObject drums;
    private MelodyChordTest melodyChordTest;
    
    void Start()
    {
        MidiStreamPlayer midiPlayer = FindObjectOfType<MidiStreamPlayer>();
        this.melodyChordTest = midiPlayer.GetComponent<MelodyChordTest>();
    }

    // Add instrument and make corresponding game object disappear
    public void AddInstrumentToGrammophone(InstrumentType type) {

        try {
            this.melodyChordTest.AddInstrument(type);

        } catch (System.InvalidOperationException e) {
            Debug.LogWarning(e.Message);
            return;
        }
        
        switch (type)
        {
            case InstrumentType.PIANO:
                this.piano.SetActive(false);
                break;
            case InstrumentType.GUITAR:
                this.guitar.SetActive(false);
                break;
            case InstrumentType.STRINGS:
                this.violin.SetActive(false);
                break;
            case InstrumentType.TRUMPET:
                this.trumpet.SetActive(false);
                break;
            case InstrumentType.DRUMS:
                this.drums.SetActive(false);
                break;
            default:
                Debug.LogWarning("Unknown instrument");
                break;
        }
    }
}
