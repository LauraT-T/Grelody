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
- Q: Add / remove piano
- W: Add / remove guitar
- E: Add / remove strings
- R: Add / remove trumpet
- T: Add / remove drum beat

*/

public class MelodyChordTest : MonoBehaviour
{
    public MidiStreamPlayer midiStreamPlayer;
    private Dictionary<MusicalKey, CompositionProvider> compositionDict; // Dictionary of two composition providers (a major key and its minor equivalent)
    private CompositionProvider compositionProvider; // Current composition provider
    private double beatCount = 0; // Number of beats that have passed
    private const int BEATS_PER_CHORD = 4; // Number of beats played until chord change
    private int chordIndex = 0; // Current index of chord being played (0 - 3)
    private float overallVolume = 0.5f; // Current volume (0.0 - 1.0)
    private float tempo = 120f; // Default tempo in beats per minute

    // Variables for adding instruments
    private InstrumentProvider instrumentProvider;
    private bool melodyAdded = false;
    private bool chordsAdded = false;
    private bool bassAdded = false;
    private bool drumsAdded = false;
    private Dictionary<InstrumentType, TuneComponent> instrumentDict; // Which instrument plays what? (melody, chords, bass, drums)


    // Manages the appearance of snowflakes for the notes
    private SnowflakeManager snowflakeManager;

    
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

        // Instrument Provider specifies all available instruments
        this.instrumentProvider = new InstrumentProvider();

        // Instrument dictionary to remember which instruments have been added and which part of the tune they are playing
        this.instrumentDict = new Dictionary<InstrumentType, TuneComponent>(){}; 

        Debug.Log("Playing random melody in C Major");

        // Find the MidiFilePlayer in the scene
        midiStreamPlayer = (MidiStreamPlayer)FindFirstObjectByType(typeof(MidiStreamPlayer));

        // Find the SnowflakeManager in the scene
        snowflakeManager = (SnowflakeManager)FindFirstObjectByType<SnowflakeManager>();
        
        if (!MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Not ready yet");
            System.Threading.Thread.Sleep(2000);
        }

        if (MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Ready after sleep");
        }

        Debug.Log("streamPlayer: " + (midiStreamPlayer != null ? "Initialized" : "Null"));

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

        // Add or remove piano with Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (this.instrumentDict.ContainsKey(InstrumentType.PIANO)) { 
                RemoveInstrument(InstrumentType.PIANO);
            } else {
                AddInstrument(InstrumentType.PIANO);
            }
        }

        // Add or remove guitar with W
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (this.instrumentDict.ContainsKey(InstrumentType.GUITAR)) { 
                RemoveInstrument(InstrumentType.GUITAR);
            } else {
                AddInstrument(InstrumentType.GUITAR);
            }
        }

        // Add or remove strings with E
        if (Input.GetKeyDown(KeyCode.E))
        {   
            if (this.instrumentDict.ContainsKey(InstrumentType.STRINGS)) { 
                RemoveInstrument(InstrumentType.STRINGS);
            } else {
                AddInstrument(InstrumentType.STRINGS);
            }
        }

        // Add or remove trumpet with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (this.instrumentDict.ContainsKey(InstrumentType.TRUMPET)) { 
                RemoveInstrument(InstrumentType.TRUMPET);
            } else {
                AddInstrument(InstrumentType.TRUMPET);
            }
        }

        // Add or remove drums with T
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (this.instrumentDict.ContainsKey(InstrumentType.DRUMS)) { 
                RemoveInstrument(InstrumentType.DRUMS);
            } else {
                AddInstrument(InstrumentType.DRUMS);
            }
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

            // Play one quarter note
            if(this.melodyAdded) {
                PlayNote(melodyNote);
                MakeSnowflakeAppear();
            }
            beatCount++;
            //Debug.Log($"Beat Count: {beatCount}");
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
            if(this.chordsAdded) {
                PlayChord();
                MakeSnowflakeAppear();
            }
            yield return new WaitForSeconds(getTimeBetweenNotes() * 1.5f);

             if(this.chordsAdded) {
                PlayChord();
                MakeSnowflakeAppear();
            }
            yield return new WaitForSeconds(getTimeBetweenNotes() * 1.5f);

             if(this.chordsAdded) {
                PlayChord();
                MakeSnowflakeAppear();
            }
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
            if(this.drumsAdded) {
                PlayDrumNote(this.drumPattern[this.drumIndex]);
                MakeSnowflakeAppear();
            }
            this.drumIndex = (this.drumIndex + 1) % this.drumPattern.Count;
            yield return new WaitForSeconds(getTimeBetweenNotes() / 2);
        }
    }

    // Plays bass note to current chord
    IEnumerator PlayBassNotes()
    {
        while (true)
        {
            if(this.bassAdded) {
                PlayBassNote(this.compositionProvider.GetBassNotes()[this.chordIndex]);
                MakeSnowflakeAppear();
            }
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
                    Duration = 300
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
            Channel = 2, // Bass notes on channel 3
            Velocity = 100,
            Duration = 500
        });
    }


    // Sets volume of the specified channel
    void SetChannelVolume(int channel, int newVolume) {

        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent
        {
            Command = MPTKCommand.ControlChange,
            Controller = MPTKController.VOLUME_MSB,
            Value = newVolume, // MIDI Volume (0-127)
            Channel = channel
        });
    }

    // Sets volume for the whole melody / all channels
    void SetOverallVolume(float newVolume) {
        midiStreamPlayer.MPTK_Volume = newVolume;
    }

    // Calculate how long a note is played in seconds
    private float getTimeBetweenNotes() {
        return 60 / this.tempo;
    }

    // Sets an instrument for a channel
    void SetInstrumentForChannel(int channel, int instrument) {
        midiStreamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.PatchChange,
            Value = instrument,
            Channel = channel
        });
    }

    /*
    Adds an instrument

    What makes up our tune, sorted by priority:
     1. Melody (channel 0)
     2. Chords (channel 1)
     3. Bass line for chords (channel 2)
     4. Drums (channel 9)

     When a non-rythm instrument is added, 
     it plays the tune component with the highest priority still missing.

     When a rythm instrument is added, it plays a rythm / the drums.
    */
    void AddInstrument(InstrumentType type)
    {
       Instrument newInstrument = this.instrumentProvider.GetInstrument(type);
       Debug.Log("NEW INSTRUMENT:");
       Debug.Log(newInstrument.GetMidiValue());
       
       if(!newInstrument.GetPlaysRythm()) {

            if(!this.melodyAdded) {
                SetInstrumentForChannel(0, newInstrument.GetMidiValue());
                this.melodyAdded = true;
                this.instrumentDict[type] = TuneComponent.MELODY;
                Debug.Log("Melody added");

            } else if(!this.chordsAdded) {
                SetInstrumentForChannel(1, newInstrument.GetMidiValue());
                this.chordsAdded = true;
                this.instrumentDict[type] = TuneComponent.CHORDS;
                Debug.Log("Chords added");

            } else if(!this.bassAdded) {
                SetInstrumentForChannel(2, newInstrument.GetMidiValue());
                this.bassAdded = true;
                this.instrumentDict[type] = TuneComponent.BASSLINE;
                Debug.Log("Bass added");

            } else {
                Debug.Log("No new non-rythm instrument can be added");
            }

       } else {
            this.drumsAdded = true;
            this.instrumentDict[type] = TuneComponent.DRUMS;
            Debug.Log("Drums added");
       }
    }

    /*
    Removes the instrument with the given type
    if it is currently playing by looking up
    in the instrument dictionary what part it is playing (melody, chords, bass, drums)
    and muting the respective part of the tune
    */
     void RemoveInstrument(InstrumentType type)
    {
        // If the instrument is currently playing
        if (instrumentDict.ContainsKey(type)) { 
            
            // Determine which part of the tune the instrument is playing and remove it
            switch(instrumentDict[type]) {

                case TuneComponent.MELODY:
                {
                    this.melodyAdded = false;
                    this.instrumentDict.Remove(type);
                    break;
                }
                case TuneComponent.CHORDS:
                {
                    this.chordsAdded = false;
                    this.instrumentDict.Remove(type);
                    break;
                }
                case TuneComponent.BASSLINE:
                {
                    this.bassAdded = false;
                    this.instrumentDict.Remove(type);
                    break;
                }
                case TuneComponent.DRUMS:
                {
                    this.drumsAdded = false;
                    this.instrumentDict.Remove(type);
                    break;
                }
                default: break;
            }
        }
    }

    // Makes a snowflake appear with correct detail degree, color and transparency
    private void MakeSnowflakeAppear() 
    {
        DetailDegree detailDegree = CalculateDetailDegree();
        Color snowflakeColor = GetSnowflakeColor();
        this.snowflakeManager.SpawnSnowflake(detailDegree, snowflakeColor);
    }


    /*
    Calculates the detail degree of the appearing snowflake
    - One instrument = low detail
    - Two instruments = medium detail
    - At least three instruments = high detail
    */
    private DetailDegree CalculateDetailDegree()
    {
        int degreeCounter = 0;
        DetailDegree detailDegree;

        if(this.melodyAdded) {
            degreeCounter++;
        }
        if(this.chordsAdded) {
            degreeCounter++;
        }
        if(this.bassAdded) {
            degreeCounter++;
        }
        if(this.drumsAdded) {
            degreeCounter++;
        }

        switch(degreeCounter) {
            case 1:
                detailDegree = DetailDegree.LOW;
                break;
            case 2:
                detailDegree = DetailDegree.MEDIUM;
                break;
            case 3:
                detailDegree = DetailDegree.HIGH;
                break;
            default:
                detailDegree = DetailDegree.HIGH;
                break;
        }

        return detailDegree;
    }

    /*
    If the current key is major, the appearing snowflake is a warm color,
    otherwise, in case of minor, a cold color.
    The louder the volume, the less transparent the snowflake.
    */
    private Color GetSnowflakeColor()
    {
        float transparency = this.overallVolume; // Value between 0.0f and 1.0f
        Color happyRed = new Color(0.8f, 0.22f, 0.12f, transparency);
        Color sadBlue = new Color(0.11f, 0.54f, 0.58f, transparency);

        if(Object.ReferenceEquals(this.compositionProvider, compositionDict[MusicalKey.MAJOR])) {
            return happyRed;
        } else {
            return sadBlue;
        }
    }

}
