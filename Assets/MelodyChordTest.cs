using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class MelodyChordTest : MonoBehaviour
{
    public MidiStreamPlayer streamPlayer;

    List<int> cMajorScale = new List<int> { 60, 62, 64, 65, 67, 69, 71 }; // C Major Scale
    Dictionary<int, List<int>> chords = new Dictionary<int, List<int>>
    {
        { 60, new List<int> { 60, 64, 67 } }, // C Major
        { 62, new List<int> { 62, 65, 69 } }, // D Minor
        { 64, new List<int> { 64, 67, 71 } }, // E Minor
    };

    void Start()
    {
        Debug.Log("Playing random melody in C Major");

        // Find the MidiFilePlayer in the scene
        streamPlayer = (MidiStreamPlayer)FindFirstObjectByType(typeof(MidiStreamPlayer));

        if (!MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Not ready yet");
            System.Threading.Thread.Sleep(2000);
        }

        if (MidiPlayerGlobal.MPTK_IsReady()) {
            Debug.Log("Ready after sleep");
        }

        Debug.Log("streamPlayer: " + (streamPlayer != null ? "Initialized" : "Null"));

        // Set instruments for channels
        // Change instrument to Marimba for channel 0
        MPTKEvent PatchChange = new MPTKEvent() {
            Command = MPTKCommand.PatchChange,
            Value = 12, // generally Marimba but depend on the SoundFont selected
            Channel = 0 }; // Instrument are defined by channel (from 0 to 15). So at any time, only 16 différents instruments can be used simultaneously.
        streamPlayer.MPTK_PlayEvent(PatchChange);    

        streamPlayer.MPTK_PlayEvent(new MPTKEvent()
        {
            Command = MPTKCommand.PatchChange,
            Value = 48, // Instrument for chords (e.g., Strings)
            Channel = 1
        });

        StartCoroutine(PlayMelodyAndChords());
    }

    IEnumerator PlayMelodyAndChords()
    {
        while (true)
        {
            int melodyNote = cMajorScale[Random.Range(0, cMajorScale.Count)];
            PlayMelody(melodyNote);
            PlayChords(melodyNote);

            yield return new WaitForSeconds(0.5f); // Adjust tempo
        }
    }

    void PlayMelody(int note)
    {
        streamPlayer.MPTK_PlayEvent(new MPTKEvent
        {
            Command = MPTKCommand.NoteOn,
            Value = note,
            Channel = 0, // Melody on channel 0
            Velocity = 100,
            Duration = 500
        });
    }

    void PlayChords(int rootNote)
    {
        if (chords.ContainsKey(rootNote))
        {
            foreach (var note in chords[rootNote])
            {
                streamPlayer.MPTK_PlayEvent(new MPTKEvent
                {
                    Command = MPTKCommand.NoteOn,
                    Value = note,
                    Channel = 1, // Chords on channel 1
                    Velocity = 80,
                    Duration = 500
                });
            }
        }
    }
}
