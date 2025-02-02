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
}
