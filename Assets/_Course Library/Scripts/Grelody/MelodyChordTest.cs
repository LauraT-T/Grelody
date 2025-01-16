using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.InputSystem;

public class MelodyChordTest : MonoBehaviour
{
    public MidiStreamPlayer midiStreamPlayer;
    private double beatCount = 0; // Number of beats that has passed
    private const int BEATS_PER_CHORD = 4; // Number of beats played until chord change
    private int chordIndex = 0; // Current index of chord being played

    private float overallVolume = 0.5f;

    List<int> cMajorScale = new List<int> { 60, 62, 64, 65, 67, 69, 71 }; // C Major Scale

    // I-V-vi-IV chord progression for the C Major scale
    List<List<int>> cMajorChords = new List<List<int>>
    {
        new List<int> { 60, 64, 67 }, // C Major
        new List<int> { 62, 67, 71 }, // G Major
        new List<int> { 60, 64, 69 }, // A Minor
        new List<int> { 65, 69, 72 }, // F Major
    };

    // Notes and passing notes for each chord of the chord progression
    List<List<int>> cMajorAllowedNotes = new List<List<int>>
    {
        new List<int> { 60, 62, 64, 65, 67 }, // C Major
        new List<int> { 60, 62, 67, 69, 71 }, // G Major
        new List<int> { 60, 62, 64, 69, 71 }, // A Minor
        new List<int> { 65, 67, 69, 71, 72 }, // F Major
    };

    List<int> cMinorScale = new List<int> { 60, 62, 63, 65, 67, 68, 71 }; // Harmonic C minor Scale

    // i-V-VI-iv chord progression for the C Minor scale
    List<List<int>> cMinorChords = new List<List<int>>
    {
        new List<int> { 60, 63, 67 }, // C Minor
        new List<int> { 62, 67, 71 }, // G Major
        new List<int> { 60, 63, 68 }, // Ab Major
        new List<int> { 65, 68, 72 }, // F Minor
    };

    // Notes and passing notes for each chord of the chord progression
    List<List<int>> cMinorAllowedNotes = new List<List<int>>
    {
        new List<int> { 60, 62, 63, 65, 67 }, // C Minor
        new List<int> { 60, 62, 67, 69, 71 }, // G Major
        new List<int> { 60, 62, 63, 68, 71 }, // Ab Major
        new List<int> { 65, 67, 68, 71, 72 }, // F Minor
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

        SetOverallVolume(this.overallVolume);

        StartCoroutine(PlayMelodyAndChords());
    }

    void Update()
    {
        // Increase volume with Up Arrow
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.overallVolume = Mathf.Clamp(this.overallVolume + 0.01f, 0.0f, 1.0f);
            SetOverallVolume(overallVolume);
            Debug.Log($"Volume increased: {overallVolume}");
        }

        // Decrease volume with Down Arrow
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.overallVolume = Mathf.Clamp(this.overallVolume - 0.01f, 0.0f, 1.0f);
            SetOverallVolume(overallVolume);
            Debug.Log($"Volume decreased: {overallVolume}");
        }
    }

    IEnumerator PlayMelodyAndChords()
    {
        while (true)
        {
            // Get random note chosen from notes and passing notes of current chord
            List<int> allowedNotes = cMinorAllowedNotes[chordIndex];
            int melodyNote = allowedNotes[Random.Range(0, allowedNotes.Count)];

            // Every four beats there is a chord change
            if(beatCount % BEATS_PER_CHORD == 0) {
                Debug.Log("Chord change");
                melodyNote = 60;
                PlayChord();
            }

            // Emphasize note on beat 1
            if(beatCount % BEATS_PER_CHORD == 0) {
                Debug.Log("Emphasis");
                SetChannelVolume(0, 127);
            }

            // Randomly decide to play two eighth notes or one quarter note
            if (Random.value > 0.5f)
            {
                // Play two short notes (0.25 each)
                PlayNote(melodyNote);
                yield return new WaitForSeconds(0.25f);

                // Reset volume
                SetChannelVolume(0, 75);

                // Choose a different note for the second short note
                melodyNote = allowedNotes[Random.Range(0, allowedNotes.Count)];
                PlayNote(melodyNote);
                beatCount++;
                Debug.Log($"Beat Count: {beatCount}");
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                // Play one long note (0.5)
                PlayNote(melodyNote);
                beatCount++;
                Debug.Log($"Beat Count: {beatCount}");
                yield return new WaitForSeconds(0.5f);
            }

            // Reset volume
            SetChannelVolume(0, 75);
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
    }

    // Plays current chord in chord progression
    void PlayChord()
    {
       
            foreach (var note in cMinorChords[chordIndex])
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

    // Sets volume of the specified channel
    void SetChannelVolume (int channel, int newVolume) {

        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
        {
            Command = MPTKCommand.ControlChange,
            Controller = MPTKController.VOLUME_MSB,
            Value = newVolume, // MIDI Volume (0-127)
            Channel = channel
        });
    }

    // Sets volume for the whole melody / all channels
    void SetOverallVolume (float newVolume) {
        midiStreamPlayer.MPTK_Volume = newVolume;
    }
}
