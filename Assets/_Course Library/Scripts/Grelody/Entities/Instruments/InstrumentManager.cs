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

    // Store original positions of instruments in instrument inventory
    private Dictionary<InstrumentType, Vector3> instrumentPositions = new Dictionary<InstrumentType, Vector3>();
    
    void Start()
    {
        MidiStreamPlayer midiPlayer = FindObjectOfType<MidiStreamPlayer>();
        this.melodyChordTest = midiPlayer.GetComponent<MelodyChordTest>();

        // Store original positions of each instrument
        instrumentPositions[InstrumentType.PIANO] = piano.transform.position;
        instrumentPositions[InstrumentType.GUITAR] = guitar.transform.position;
        instrumentPositions[InstrumentType.STRINGS] = violin.transform.position;
        instrumentPositions[InstrumentType.TRUMPET] = trumpet.transform.position;
        instrumentPositions[InstrumentType.DRUMS] = drums.transform.position;
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

    // Reset all instruments to their original positions and make them visible
    public void ResetInstruments()
    {
        foreach (var entry in instrumentPositions)
        {
            InstrumentType type = entry.Key;
            Vector3 originalPosition = entry.Value;

            GameObject instrument = GetInstrumentByType(type);
            if (instrument != null)
            {
                instrument.SetActive(true);
                instrument.transform.position = originalPosition;
            }
        }
    }

    // Helper method to get the correct instrument GameObject based on type
    private GameObject GetInstrumentByType(InstrumentType type)
    {
        switch (type)
        {
            case InstrumentType.PIANO: return piano;
            case InstrumentType.GUITAR: return guitar;
            case InstrumentType.STRINGS: return violin;
            case InstrumentType.TRUMPET: return trumpet;
            case InstrumentType.DRUMS: return drums;
            default: return null;
        }
    }
}
