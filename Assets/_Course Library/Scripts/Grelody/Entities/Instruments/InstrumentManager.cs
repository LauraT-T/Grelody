using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.InputSystem;

public class InstrumentManager : MonoBehaviour
{
    public GameObject instrumentMenu;
    public GameObject invisibleInstrumentsParent;
    public GameObject piano;
    public GameObject guitar;
    public GameObject violin;
    public GameObject trumpet;
    public GameObject drums;
    private MelodyChordTest melodyChordTest;

    // Store original positions of instruments in instrument inventory
    private Dictionary<InstrumentType, Vector3> instrumentPositions = new Dictionary<InstrumentType, Vector3>();

    // Positions of invisible insruments
    private List<Vector3> invisiblePositions;
    private List<GameObject> invisibleInstruments;
    
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

        // Posisitions for invisible instruments
        this.invisiblePositions = new List<Vector3>();

        this.invisiblePositions.Add(new Vector3(0.7f, 1f, -0.9f)); 
        this.invisiblePositions.Add(new Vector3(1.0f, 1f, -0.9f)); 
        this.invisiblePositions.Add(new Vector3(1.3f, 1f, -0.9f)); 
        this.invisiblePositions.Add(new Vector3(1.6f, 1f, -0.9f));
        this.invisiblePositions.Add(new Vector3(1.9f, 1f, -0.9f));

        // List of currently invisible instruments
        this.invisibleInstruments = new List<GameObject>();

        AddInstrumentToGrammophone(InstrumentType.PIANO);
        AddInstrumentToGrammophone(InstrumentType.STRINGS);
        AddInstrumentToGrammophone(InstrumentType.TRUMPET);
        AddInstrumentToGrammophone(InstrumentType.DRUMS);
        AddInstrumentToGrammophone(InstrumentType.GUITAR);
    }

    // Add instrument and make corresponding game object disappear
    public void AddInstrumentToGrammophone(InstrumentType type) {

        try {
            this.melodyChordTest.AddInstrument(type);

        } catch (System.InvalidOperationException e) {
            Debug.LogWarning(e.Message);
            return;
        }

        int index;
        
        switch (type)
        {
            case InstrumentType.PIANO:
                Debug.Log("ADD TO GRAMMOPHONE PIANO");
                this.invisibleInstruments.Add(this.piano);

                // Set position
                index = this.invisibleInstruments.IndexOf(this.piano);
                this.piano.transform.position = this.invisiblePositions[index];
                Debug.Log("Index: " + index + " Vektor: " + this.invisiblePositions[index]);

                // Set parent
                this.invisibleInstruments[index].transform.SetParent(invisibleInstrumentsParent.transform);

                // Make invisible 
                
                break;
            case InstrumentType.GUITAR:
                Debug.Log("ADD TO GRAMMOPHONE GUITAR");
                this.invisibleInstruments.Add(this.guitar);

                // Set position
                index = this.invisibleInstruments.IndexOf(this.guitar);
                this.guitar.transform.position = this.invisiblePositions[index];
                Debug.Log("Index: " + index + " Vektor: " + this.invisiblePositions[index]);

                // Set parent
                this.invisibleInstruments[index].transform.SetParent(invisibleInstrumentsParent.transform);

                // Make invisible

                break;
            case InstrumentType.STRINGS:
                Debug.Log("ADD TO GRAMMOPHONE STRINGS");
                this.invisibleInstruments.Add(this.violin);

                // Set position
                index = this.invisibleInstruments.IndexOf(this.violin);
                this.violin.transform.position = this.invisiblePositions[index];
                Debug.Log("Index: " + index + " Vektor: " + this.invisiblePositions[index]);

                // Set parent
                this.invisibleInstruments[index].transform.SetParent(invisibleInstrumentsParent.transform);

                // Make invisible

                break;
            case InstrumentType.TRUMPET:
                Debug.Log("ADD TO GRAMMOPHONE TRUMPET");
                this.invisibleInstruments.Add(this.trumpet);

                // Set position
                index = this.invisibleInstruments.IndexOf(this.trumpet);
                this.trumpet.transform.position = this.invisiblePositions[index];
                Debug.Log("Index: " + index + " Vektor: " + this.invisiblePositions[index]);

                // Set parent
                this.invisibleInstruments[index].transform.SetParent(invisibleInstrumentsParent.transform);

                // Make invisible

                break;
            case InstrumentType.DRUMS:
                Debug.Log("ADD TO GRAMMOPHONE DRUMS");
                this.invisibleInstruments.Add(this.drums);

                // Set position
                index = this.invisibleInstruments.IndexOf(this.drums);
                this.drums.transform.position = this.invisiblePositions[index];
                Debug.Log("Index: " + index + " Vektor: " + this.invisiblePositions[index]);

                // Set parent
                this.invisibleInstruments[index].transform.SetParent(invisibleInstrumentsParent.transform);
                
                // Make invisible
                break;
            default:
                Debug.LogWarning("Unknown instrument");
                break;
        }
    }

    // Reset all instruments to their original positions and make them visible
    public void ResetInstruments()
    {
        SetParentsToNull();

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

    // Set parent to null and clear list of added instruments
    public void SetParentsToNull()
    {
        foreach (var instrument in invisibleInstruments)
        {
            instrument.transform.SetParent(null);
        }

        this.invisibleInstruments.Clear();
    }
}
