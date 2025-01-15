using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class MelodyChordTest : MonoBehaviour
{
    public MidiStreamPlayer midiStreamPlayer;
    private int noteCount = 0; // Number of notes that has been played
    private const int NOTES_PER_CHORD = 4; // Number of notes played until chord change
    private int chordIndex = 0; // Current index of chord being played

    List<int> cMajorScale = new List<int> { 60, 62, 64, 65, 67, 69, 71 }; // C Major Scale

    // I-V-vi-IV chord progression for the C Major scale
    List<List<int>> cMajorChords = new List<List<int>>
    {
        new List<int> { 60, 64, 67 }, // C Major
        new List<int> { 62, 67, 71 }, // G Major
        new List<int> { 60, 64, 69 }, // A Minor
        new List<int> { 60, 65, 69 }, // F Major
    };

    // Notes and passing notes for each chord of the chord progression
    List<List<int>> cMajorAllowedNotes = new List<List<int>>
    {
        new List<int> { 60, 62, 64, 65, 67 }, // C Major
        new List<int> { 60, 62, 67, 69, 71 }, // G Major
        new List<int> { 60, 62, 64, 69, 71 }, // A Minor
        new List<int> { 60, 65, 67, 69, 71 }, // F Major
    };

    void Start()
    {
        Debug.Log("Playing random melody in C Major");

        // Find the MidiFilePlayer in the scene
        midiStreamPlayer = (MidiStreamPlayer)FindFirstObjectByType(typeof(MidiStreamPlayer));

        if (!MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Not ready yet");
            System.Threading.Thread.Sleep(2000);
        }

        if (MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Ready after sleep");
        }

        Debug.Log("streamPlayer: " + (midiStreamPlayer != null ? "Initialized" : "Null"));

        // Set instrument for channel 0 (melody)
        MPTKEvent PatchChange = new MPTKEvent() {
            Command = MPTKCommand.PatchChange,
            Value = 12, // Marimba
            Channel = 0 }; // Instrument are defined by channel (from 0 to 15). So at any time, only 16 differents instruments can be used simultaneously.
        midiStreamPlayer.MPTK_PlayEvent(PatchChange);    

        // Set instrument for channel 1 (chords)
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.PatchChange,
            Value = 48, // Strings
            Channel = 1
        });

        StartCoroutine(PlayMelodyAndChords());
    }

    IEnumerator PlayMelodyAndChords()
    {
        while (true)
        {
            // Get random note chosen from notes and passing notes of current chord
            List<int> allowedNotes = cMajorAllowedNotes[chordIndex];
            int melodyNote = allowedNotes[Random.Range(0, allowedNotes.Count)];

            // Every four notes there is a chord change
            if(noteCount % NOTES_PER_CHORD == 0) {
                Debug.Log("Chord change");
                melodyNote = 60;
                PlayChord();
            }

            PlayNote(melodyNote);
            
            yield return new WaitForSeconds(0.5f); // Adjust tempo
        }
    }

    // Plays the given note and increments note counter
    void PlayNote(int note)
    {
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 0, // Melody on channel 0
            Velocity = 100,
            Duration = 500
        });

        noteCount++;
        Debug.Log($"Note Count: {noteCount}, Note: {note}");
    }

    // Plays current chord in chord progression
    void PlayChord()
    {
       
            foreach (var note in cMajorChords[chordIndex])
            {
                midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
                {
                    Command = MPTKCommand.NoteOn,
                    Value = note,
                    Channel = 1, // Chords on channel 1
                    Velocity = 80,
                    Duration = 500
                });
            }

            // Move to next chord in chord progression
            chordIndex = (chordIndex + 1) % cMajorChords.Count;
    }
}
