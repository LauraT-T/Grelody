using UnityEngine;
using System;
using System.Collections.Generic;
using FluidSynthUnity;

public class FluidSynthUnityTest : MonoBehaviour
{
    public SoundFontAsset SoundFontAsset; // Assign this in the Unity Inspector
    public MidiSynthBehavior MidiSynth;  // Attach the MidiSynthBehavior component

    private List<Tone> melodyTones;
    private int currentNoteIndex = 0;

    private void Awake()
    {
        if (SoundFontAsset == null)
        {
            Debug.LogError("SoundFontAsset is not assigned in the Inspector.");
        }
        else
        {
            // Initialize FluidSynth with the provided SoundFont
            SoundFontManager.LoadSoundFont(SoundFontAsset);
        }

        if (MidiSynth == null)
        {
            Debug.LogError("MidiSynthBehavior is not assigned in the Inspector.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Generate a melody using FluidSynthUnity tones
        melodyTones = GenerateMelody();

        if (melodyTones == null || melodyTones.Count == 0)
        {
            Debug.LogError("The melodyTones list is empty or not initialized.");
            return;
        }

        // Start playing the melody
        PlayNextNote();
    }

    // Update is called once per frame
    void Update()
    {
        SoundFont soundFont = SoundFontManager.soundFont;
        if (soundFont == null || !SoundFontManager.soundFontInitialized)
        {
            return;
        }
    }

    private List<Tone> GenerateMelody()
    {
        // Define the root note and intervals for the melody
        var rootNote = Tone.C_4; // Starting note (C4)
        
        // Intervals for the melody in semitones (MIDI-style intervals)
        var intervals = new[] {
            0, 2, 3, 5, 3, -2,    // C4, D4, D#4, F4, D#4, D4 (first phrase)
            0, 2, 7, 12, 7, 3,    // C4, D4, G4, C5, G4, D#4 (second phrase)
            0, 2, 7, 3, -2, 0,    // C4, D4, G4, D#4, D4, C4 (third phrase)
            3, 5, 12, 7, 2        // D#4, F4, C5, G4, D4 (fourth phrase)
        };

        var melodyTones = new List<Tone>();
        var currentNote = rootNote;

        foreach (var interval in intervals)
        {
            // Convert the current note to a Tone
            melodyTones.Add(currentNote);

            // Transpose to the next note using the interval
            currentNote = TransposeTone(currentNote, interval);
        }

        return melodyTones;
    }

    private Tone TransposeTone(Tone currentNote, int interval)
    {
        // Get the corresponding integer for the current note
        int currentNoteValue = (int)currentNote;

        // Transpose the note by the given interval
        int newNoteValue = currentNoteValue + interval;

        // Ensure the new note stays within valid bounds
        newNoteValue = Mathf.Clamp(newNoteValue, 0, Enum.GetValues(typeof(Tone)).Length - 1);

        // Return the new transposed note as a Tone
        return (Tone)newNoteValue;
    }

    private void PlayNextNote()
    {
        if (currentNoteIndex >= melodyTones.Count)
        {
            Debug.Log("Finished playing all notes.");
            return;
        }

        // Get the next tone
        var tone = melodyTones[currentNoteIndex];

        if (MidiSynth == null)
        {
            Debug.LogError("MidiSynth is null, cannot play note.");
            return;
        }

        // Select the first instrument (e.g., Piano)
        var instrument = SoundFontManager.soundFont.Instruments[(0, 0)];

        // Play the tone using FluidSynthUnity
        MidiSynth.PlayNote(tone, (0, 0), 500, 100);

        // Increment the note index and schedule the next note
        currentNoteIndex++;
        Invoke(nameof(PlayNextNote), 0.5f); // Schedule the next note after 500ms
    }
}
