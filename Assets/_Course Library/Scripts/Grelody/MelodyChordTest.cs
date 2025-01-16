using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.InputSystem;

/*
Controls (for testing)

- Up arrow: increase volume
- Down arrow: decrease volume
- K: switch beween major and minor
- F: increase tempo / faster
- S: decrease tempo / slower

*/

public class MelodyChordTest : MonoBehaviour
{
    public MidiStreamPlayer midiStreamPlayer;
    private Dictionary<MusicalKey, CompositionProvider> compositionDict; // Dictionary of two composition providers (a major key and its minor equivalent)
    private CompositionProvider compositionProvider; // Current composition provider
    private double beatCount = 0; // Number of beats that has passed
    private const int BEATS_PER_CHORD = 4; // Number of beats played until chord change
    private int chordIndex = 0; // Current index of chord being played (0 - 3)
    private float overallVolume = 0.5f; // Current volume (0.0 - 1.0)
    private float tempo = 120f; // Default tempo in beats per minute
    
    // Drum pattern
    List<int> drumPattern = new List<int>
    {
        35, 42, 38, 42, 35, 42, 38, 46 // Kick, hi-hat, snare, hi-hat, repeat
    };

    private int drumIndex = 0; // Current index of drum pattern note being played

    void Start()
    {
        // Melody is in C Major and C Minor
        this.compositionDict = new Dictionary<MusicalKey, CompositionProvider>(){
        {MusicalKey.MAJOR, new CMajorCompositionProvider()},
        {MusicalKey.MINOR, new CMinorCompositionProvider()}}; 

        // Default key is major
        this.compositionProvider = compositionDict[MusicalKey.MAJOR]; 

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

        StartCoroutine(PlayMelody());
        StartCoroutine(PlayChords());
        StartCoroutine(PlayDrumPattern());
        StartCoroutine(PlayBassNotes());

    }

    void Update()
    {
        // Increase volume with Up Arrow
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.overallVolume = Mathf.Clamp(this.overallVolume + 0.01f, 0.0f, 1.0f);
            SetOverallVolume(overallVolume);
            Debug.Log($"Volume increased: {this.overallVolume}");
        }

        // Decrease volume with Down Arrow
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.overallVolume = Mathf.Clamp(this.overallVolume - 0.01f, 0.0f, 1.0f);
            SetOverallVolume(overallVolume);
            Debug.Log($"Volume decreased: {this.overallVolume}");
        }

        // Switch between major and minor when pressing K
        if (Input.GetKeyDown(KeyCode.K))
        {
            if(this.compositionProvider.GetKey() == MusicalKey.MAJOR) {
                this.compositionProvider = compositionDict[MusicalKey.MINOR];
                Debug.Log("Key switch to minor"); 
            } else {
                this.compositionProvider = compositionDict[MusicalKey.MAJOR]; 
                Debug.Log("Key switch to major");
            }
        }

        // Increase tempo with F
        if (Input.GetKey(KeyCode.F))
        {
            tempo = Mathf.Clamp(this.tempo + 0.1f, 30f, 240f); // Limit between 30 BPM and 240 BPM
            Debug.Log($"Tempo increased: {this.tempo} bpm");
        }

        // Decrease tempo with S
        if (Input.GetKey(KeyCode.S))
        {
            tempo = Mathf.Clamp(this.tempo - 0.1f, 30f, 240f); // Limit between 30 BPM and 240 BPM
            Debug.Log($"Tempo decreased: {tempo} bpm");
        }
    }

    IEnumerator PlayMelody()
    {
        while (true)
        {
            // Get random note chosen from notes and passing notes of current chord
            List<List<int>> allAllowedNotes = compositionProvider.GetAllowedNotes();
            List<int> allowedNotes = allAllowedNotes[chordIndex];
            int melodyNote = allowedNotes[Random.Range(0, allowedNotes.Count)];

            // Emphasize note on beat 1
            if(beatCount % BEATS_PER_CHORD == 0) {
                Debug.Log("Emphasis");
                SetChannelVolume(0, 127);
            }

           /*  // Randomly decide to play two eighth notes or one quarter note
            if (Random.value > 0.5f)
            {
                // Play two short notes (0.25 each)
                PlayNote(melodyNote);
                yield return new WaitForSeconds(getTimeBetweenNotes() / 4);

                // Reset volume
                SetChannelVolume(0, 75);

                // Choose a different note for the second short note
                melodyNote = allowedNotes[Random.Range(0, allowedNotes.Count)];
                PlayNote(melodyNote);
                beatCount++;
                Debug.Log($"Beat Count: {beatCount}");
                yield return new WaitForSeconds(getTimeBetweenNotes() / 2);
            }
            else
            {
                // Play one long note (0.5)
                PlayNote(melodyNote);
                beatCount++;
                Debug.Log($"Beat Count: {beatCount}");
                yield return new WaitForSeconds(getTimeBetweenNotes());
            }
            */
             // Play one long note (0.5)
                PlayNote(melodyNote);
                beatCount++;
                Debug.Log($"Beat Count: {beatCount}");
                yield return new WaitForSeconds(getTimeBetweenNotes());
            

            // Reset volume
            SetChannelVolume(0, 75);
        }
    }

    // Plays chords
    IEnumerator PlayChords()
    {
        while (true)
        {
            // Pattern: 3/8, 3/8, 2/8
            PlayChord();
            yield return new WaitForSeconds(getTimeBetweenNotes() * 1.5f);
            PlayChord();
            yield return new WaitForSeconds(getTimeBetweenNotes() * 1.5f);
            PlayChord();
            yield return new WaitForSeconds(getTimeBetweenNotes());

            // Move to next chord in chord progression (Every four beats there is a chord change)
            List<List<int>> chords = compositionProvider.GetChords();
            chordIndex = (chordIndex + 1) % chords.Count;
            Debug.Log("Chord change");
        }
    }

    // Plays drum pattern
    IEnumerator PlayDrumPattern()
    {
        while (true)
        {
            PlayDrumNote(this.drumPattern[this.drumIndex]);
            this.drumIndex = (this.drumIndex + 1) % this.drumPattern.Count;
            yield return new WaitForSeconds(getTimeBetweenNotes() / 2);
        }
    }

    // Plays bass note to current chord
    IEnumerator PlayBassNotes()
    {
        while (true)
        {
            PlayBassNote(this.compositionProvider.GetBassNotes()[this.chordIndex]);
            yield return new WaitForSeconds(getTimeBetweenNotes() / 2);
        }
    }

    // Plays the given melody note
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
            List<List<int>> chords = compositionProvider.GetChords();
       
            foreach (var note in chords[chordIndex])
            {
                midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
                {
                    Command = MPTKCommand.NoteOn,
                    Value = note,
                    Channel = 1, // Chords on channel 1
                    Velocity = 80,
                    Duration = 250
                });
            }
    }

    // Plays the given beat note
    void PlayDrumNote(int note)
    {
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 9, // Drum pattern on channel 9
            Velocity = 100,
            Duration = 500
        });
    }

    // Plays the given bass note
    void PlayBassNote(int note)
    {
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 3, // Bass notes on channel 3
            Velocity = 100,
            Duration = 500
        });
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

    // Calculate how long a note is played in seconds
    private float getTimeBetweenNotes() {
        return 60 / this.tempo;
    }
}
