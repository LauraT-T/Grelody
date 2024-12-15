using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using Melanchall.DryWetMidi.MusicTheory;

public class PlayPauseButtons : MonoBehaviour

{
    // MIDI variables
    private const string OutputDeviceName = "Microsoft GS Wavetable Synth";
    private const string OutputDeviceNameMac = "IAC Driver Bus 1";
    private OutputDevice _outputDevice;
    private Playback _playback;

    // Buttons
    public Button playButton;
    public Button pauseButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeOutputDevice();
        var midiFile = CreateTestFile();
        InitializeFilePlayback(midiFile);
        playButton.onClick.AddListener(PlayTrack);
        pauseButton.onClick.AddListener(PauseTrack);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Releasing playback and device...");

        if (_playback != null) {
            _playback.Dispose();
        }

        if (_outputDevice != null) {
            _outputDevice.Dispose();
        }
           
        Debug.Log("Playback and device released.");
    }
    
    // Play the melody
    private void PlayTrack() 
    {
        Debug.Log("Play track");
        StartPlayback();
    }

     private void StartPlayback()
    {
        Debug.Log("Starting playback...");
        _playback.Start();
    }

    // Pause the melody
    private void PauseTrack() 
    {
        Debug.Log("Pause track");
        PausePlayback();
    }

     private void PausePlayback() 
    {
        if(_playback != null) {
            _playback.Stop();
        }
        
    }

    // Initialize the MIDI output device
    private void InitializeOutputDevice()
    {
        Debug.Log($"Initializing output device [{OutputDeviceName}] or [{OutputDeviceNameMac}]...");

        var allOutputDevices = OutputDevice.GetAll();
        if (!allOutputDevices.Any(d => d.Name == OutputDeviceName))
        {
            if (!allOutputDevices.Any(d => d.Name == OutputDeviceNameMac)) {
                var allDevicesList = string.Join(Environment.NewLine, allOutputDevices.Select(d => $"  {d.Name}"));
                Debug.Log($"There is no [{OutputDeviceName}] device presented in the system. Here the list of all device:{Environment.NewLine}{allDevicesList}");
            }

            else {
                _outputDevice = OutputDevice.GetByName(OutputDeviceNameMac);
                Debug.Log($"Output device [{OutputDeviceNameMac}] initialized.");
            }
            
        }
        else {
            _outputDevice = OutputDevice.GetByName(OutputDeviceName);
            Debug.Log($"Output device [{OutputDeviceName}] initialized.");
        }
    }

    // Create a midi file (melody)
    private MidiFile CreateTestFile()
{
    var bassChord = new[] { Interval.Twelve };

    var chorusPattern = new PatternBuilder()
        .SetNoteLength(MusicalTimeSpan.Eighth.Triplet()) 
        .Anchor()
        .SetRootNote(Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.F, 4)) // Use fully qualified name for Note to avoid name conflict
        .Note(Interval.Zero)    // F4
        .Note(Interval.Four)    // A4
        .Note(Interval.Five)    // Bb4 
        .Note(Interval.Seven)   // C5
        .Note(Interval.Five)    // Bb4 
        .Note(Interval.Four)    // A4
        .Note(Interval.Zero)    // F4
        .Note(Interval.Four)    // A4
        .Note(Interval.Seven)   // C5
        .Note(Interval.Twelve)  // F5
        .Note(Interval.Seven)   // C5
        .Note(Interval.Five)    // Bb4 
        .MoveToFirstAnchor()
        .SetNoteLength(MusicalTimeSpan.Half)
        .Chord(bassChord, Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.F, 2))      // F2
        .Chord(bassChord, Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.ASharp, 2)) // Bb2
        .Chord(bassChord, Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.D, 2))      // D2
        .Chord(bassChord, Melanchall.DryWetMidi.MusicTheory.Note.Get(NoteName.C, 2))      // C2
        .Repeat(); // Make the pattern repeat

    var midiFile = chorusPattern.Build().ToFile(TempoMap.Default);
    Debug.Log("Test MIDI file created.");
    return midiFile;
}


    // Initialize the file playback
    private void InitializeFilePlayback(MidiFile midiFile)
    {
        Debug.Log("Initializing playback...");

        _playback = midiFile.GetPlayback(_outputDevice);
        _playback.Loop = true;
       
        Debug.Log($"Output device [{OutputDeviceName}] initialized.");
    }

}
