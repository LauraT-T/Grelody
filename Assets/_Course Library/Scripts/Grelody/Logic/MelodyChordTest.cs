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
- X: Make snowman appear and replay tune
- M: Make adding instruments possible again after stopping the music with X

*/

public class MelodyChordTest : MonoBehaviour
{
    // Constants
    private const float DEFAULT_VOLUME = 0.5f;
    private const float DEFAULT_TEMPO = 120f;
    private readonly Vector3 SNOWMAN_POSITION = new Vector3(-0.2f, 1.0f, -1.1f);

    // Variables
    public MidiStreamPlayer midiStreamPlayer;
    private Dictionary<MusicalKey, CompositionProvider> compositionDict; // Dictionary of two composition providers (a major key and its minor equivalent)
    private CompositionProvider compositionProvider; // Current composition provider
    private const int BEATS_PER_CHORD = 4; // Number of beats played until chord change
    private int chordIndex = 0; // Current index of chord being played (0 - 3)
    private float overallVolume = DEFAULT_VOLUME; // Current volume (0.0 - 1.0)
    private float tempo = DEFAULT_TEMPO; // Default tempo in beats per minute

    // Coroutines
    private IEnumerator melodyCoroutine;
    private IEnumerator chordCoroutine;
    private IEnumerator bassCoroutine;
    private IEnumerator drumsCoroutine;
    private bool coroutinesRunning = false;

    // Variables for adding instruments
    private InstrumentProvider instrumentProvider;
    private bool melodyAdded = false;
    private bool chordsAdded = false;
    private bool bassAdded = false;
    private bool drumsAdded = false;
    private Dictionary<InstrumentType, TuneComponent> instrumentDict; // Which instrument plays what? (melody, chords, bass, drums)


    // Manages the appearance of snowflakes for the notes
    private SnowflakeManager snowflakeManager;

    // Manages the appearance of snowmen for the finished tunes
    private SnowmanManager snowmanManager;

    // Recording of the created tune
    private MelodyRecorder melodyRecorder;

    // Count the number of beats in major and minor to determine the snowman's appearance
    private int majorCounter = 0;
    private int minorCounter = 0;

    // Store which instruments were added in a HashMap to determine number of snowballs for snowman
    private HashSet<InstrumentType> addedInstruments = new HashSet<InstrumentType>();

    
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

        // Find the SnowmanManager in the scene
        snowmanManager = (SnowmanManager)FindFirstObjectByType<SnowmanManager>();
        
        if (!MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Not ready yet");
            System.Threading.Thread.Sleep(2000);
        }

        if (MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Ready after sleep");
        }

        Debug.Log("streamPlayer: " + (midiStreamPlayer != null ? "Initialized" : "Null"));

        SetOverallVolume(this.overallVolume);

        // Start the coroutines (nothing is heard until instrument is added)
        StartMusic();

        // Recorder to save the created melody
        this.melodyRecorder = new MelodyRecorder();
        

        
        // UNCOMMENT TO TEST BUILD ON QUEST
        
        AddInstrument(InstrumentType.PIANO);
        AddInstrument(InstrumentType.GUITAR);
        AddInstrument(InstrumentType.TRUMPET);
        AddInstrument(InstrumentType.DRUMS);

        Invoke("StopMusic", 25f); // Stops the music and makes snowman appear after 5 seconds
        

    }

    void Update()
    {
        // Increase volume with Up Arrow
        if (Keyboard.current.upArrowKey.isPressed)
        {
            overallVolume = Mathf.Clamp(overallVolume + 0.01f, 0.0f, 1.0f);
            SetOverallVolume(overallVolume);
            Debug.Log($"Volume increased: {overallVolume}");
        }

        // Decrease volume with Down Arrow
        if (Keyboard.current.downArrowKey.isPressed)
        {
            overallVolume = Mathf.Clamp(overallVolume - 0.01f, 0.0f, 1.0f);
            SetOverallVolume(overallVolume);
            Debug.Log($"Volume decreased: {overallVolume}");
        }

        // Switch between major and minor when pressing K
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            compositionProvider = compositionProvider.GetKey() == MusicalKey.MAJOR
                ? compositionDict[MusicalKey.MINOR]
                : compositionDict[MusicalKey.MAJOR];

            Debug.Log($"Key switch to {(compositionProvider.GetKey() == MusicalKey.MAJOR ? "major" : "minor")}");
        }

        // Increase tempo with F
        if (Keyboard.current.fKey.isPressed)
        {
            tempo = Mathf.Clamp(tempo + 0.1f, 30f, 240f);
            Debug.Log($"Tempo increased: {tempo} bpm");
        }

        // Decrease tempo with S
        if (Keyboard.current.sKey.isPressed)
        {
            tempo = Mathf.Clamp(tempo - 0.1f, 30f, 240f);
            Debug.Log($"Tempo decreased: {tempo} bpm");
        }

        // Add or remove piano with Q
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleInstrument(InstrumentType.PIANO);
        }

        // Add or remove guitar with W
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            ToggleInstrument(InstrumentType.GUITAR);
        }

        // Add or remove strings with E
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleInstrument(InstrumentType.STRINGS);
        }

        // Add or remove trumpet with R
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ToggleInstrument(InstrumentType.TRUMPET);
        }

        // Add or remove drums with T
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            ToggleInstrument(InstrumentType.DRUMS);
        }

        // Stop music and create a snowman
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            StopMusic();
        }

        // Start recording a new tune
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            StartMusic();
        }
    }

    void ToggleInstrument(InstrumentType instrument)
    {
        if (instrumentDict.ContainsKey(instrument))
        {
            RemoveInstrument(instrument);
        }
        else
        {
            AddInstrument(instrument);
        }
    }

    void StartMusic()
    {
        StopAllCoroutines();

        this.melodyCoroutine = PlayMelody();
        this.chordCoroutine = PlayChords();
        this.bassCoroutine = PlayBassNotes();
        this.drumsCoroutine = PlayDrumPattern();

        StartCoroutine(this.melodyCoroutine);
        StartCoroutine(this.chordCoroutine);
        StartCoroutine(this.drumsCoroutine);
        StartCoroutine(this.bassCoroutine);

        this.coroutinesRunning = true;
        Debug.Log("Coroutines started");
    }

    /*
    Stops the music, makes snowflakes disappear, creates snowman out of the recorded melody
    */
    void StopMusic()
    {
        // Stop music
        this.melodyAdded = false;
        this.chordsAdded = false;
        this.bassAdded = false;
        this.drumsAdded = false;

        StopCoroutine(this.melodyCoroutine);
        StopCoroutine(this.chordCoroutine);
        StopCoroutine(this.drumsCoroutine);
        StopCoroutine(this.bassCoroutine);

        StopAllCoroutines();

        this.coroutinesRunning = false;
        Debug.Log("Coroutines stopped");

        // Get the recorded melody and stop recording
        Melody recordedMelody = this.melodyRecorder.GetMelody();
        this.melodyRecorder.SetIsRecording(false);

        // Make snowflakes disappear with a callback
        this.snowflakeManager.DisappearAllSnowflakes(SNOWMAN_POSITION, () =>
        {
            // Make smowman appear
            bool isHappy = this.majorCounter >= this.minorCounter ? true : false;
            int beatCount = this.majorCounter + this.minorCounter;
            Debug.Log($"Beat count in MelodyChordTest : {this.majorCounter} major + {this.minorCounter} minor = {beatCount}");
            snowmanManager.SpawnSnowman(SNOWMAN_POSITION, this.addedInstruments.Count, isHappy, recordedMelody, beatCount);

            // Start replay of recorded melody
            recordedMelody.StartReplay(this, this.midiStreamPlayer); // TODO: move elsewhere?

            // Reset variables needed for snowman creation
            this.majorCounter = 0;
            this.minorCounter = 0;
            this.addedInstruments.Clear();
            this.instrumentDict.Clear();
        });

        // Reset other variables
        this.overallVolume = DEFAULT_VOLUME;
        this.tempo = DEFAULT_TEMPO;
        this.compositionProvider = compositionDict[MusicalKey.MAJOR];
    }

    IEnumerator PlayMelody()
    {
        while (true)
        {
            // Get random note chosen from notes and passing notes of current chord
            List<List<int>> allAllowedNotes = compositionProvider.GetAllowedNotes();
            List<int> allowedNotes = allAllowedNotes[chordIndex];
            int melodyNote = allowedNotes[Random.Range(0, allowedNotes.Count)];

            // Play one quarter note
            if(this.melodyAdded) {
                PlayNote(melodyNote);
                MakeSnowflakeAppear();
            }

            // Count major and minor beats
            if(this.melodyAdded || this.chordsAdded || this.bassAdded || this.drumsAdded) {
                if(Object.ReferenceEquals(this.compositionProvider, compositionDict[MusicalKey.MAJOR])) {
                    this.majorCounter++;
                } else {
                    this.minorCounter++;
                }
            }
            
            yield return new WaitForSeconds(getTimeBetweenNotes());
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
        MPTKEvent midiEvent = new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 0, // Melody on channel 0
            Velocity = 100,
            Duration = 500
        };

        midiStreamPlayer.MPTK_PlayEvent(midiEvent);
        this.melodyRecorder.RecordEvent(midiEvent);
    }

    // Plays current chord in chord progression
    void PlayChord()
    {
            List<List<int>> chords = compositionProvider.GetChords();
       
            foreach (var note in chords[chordIndex])
            {
                MPTKEvent midiEvent = new MPTKEvent
                {
                    Command = MPTKCommand.NoteOn,
                    Value = note,
                    Channel = 1, // Chords on channel 1
                    Velocity = 80,
                    Duration = 300
                };
                midiStreamPlayer.MPTK_PlayEvent(midiEvent);
                this.melodyRecorder.RecordEvent(midiEvent);
            }
    }

    // Plays the given beat note
    void PlayDrumNote(int note)
    {
        MPTKEvent midiEvent = new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 9, // Drum pattern on channel 9
            Velocity = 100,
            Duration = 500
        };
        midiStreamPlayer.MPTK_PlayEvent(midiEvent);
        this.melodyRecorder.RecordEvent(midiEvent);
    }

    // Plays the given bass note
    void PlayBassNote(int note)
    {
        MPTKEvent midiEvent = new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 2, // Bass notes on channel 3
            Velocity = 100,
            Duration = 500
        };
        midiStreamPlayer.MPTK_PlayEvent(midiEvent);
        this.melodyRecorder.RecordEvent(midiEvent);
    }


    // Sets volume of the specified channel
    void SetChannelVolume(int channel, int newVolume) {

        MPTKEvent midiEvent = new MPTKEvent
        {
            Command = MPTKCommand.ControlChange,
            Controller = MPTKController.VOLUME_MSB,
            Value = newVolume, // MIDI Volume (0-127)
            Channel = channel
        };
        midiStreamPlayer.MPTK_PlayEvent(midiEvent);
        this.melodyRecorder.RecordEvent(midiEvent);
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
        MPTKEvent midiEvent = new MPTKEvent()
        {
            Command = MPTKCommand.PatchChange,
            Value = instrument,
            Channel = channel
        };
        midiStreamPlayer.MPTK_PlayEvent(midiEvent);
        this.melodyRecorder.RecordEvent(midiEvent);
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
        // No instruments can be added if coroutines are not running and, thus, no music can be heard
        if(!this.coroutinesRunning) {
            Debug.Log("No coroutines running, so no instrument can be added.");
            return;
        }

        // Start recording if instrument is added for the first time
        if(this.addedInstruments.Count == 0 && !this.melodyRecorder.GetIsRecording()) {
            this.melodyRecorder.StartRecording();
        }

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

        // Store added instrument types in HashMap
        this.addedInstruments.Add(type);
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

    // changes the composition to Major
    public void changeToMajor() {

        if (compositionProvider.GetKey() == MusicalKey.MINOR) {
            this.compositionProvider = compositionDict[MusicalKey.MAJOR];
        }
        
    }

    // changes the composition to Major
    public void changeToMinor() {

        if (compositionProvider.GetKey() == MusicalKey.MAJOR) {
            this.compositionProvider = compositionDict[MusicalKey.MINOR];
        }
        
    }
}
