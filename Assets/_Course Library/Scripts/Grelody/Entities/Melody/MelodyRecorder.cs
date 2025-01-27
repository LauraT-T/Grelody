
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

class MelodyRecorder
{
    private Melody currentMelody; // The melody currently being recorded
    private float startTime; // The start time of the recording

    public MelodyRecorder() 
    {

    }

    // Creates new melody and determines start time of recording
    public void StartRecording()
    {
        this.currentMelody = new Melody();
        startTime = Time.time;
        Debug.Log("Recording started");
    }

    // Adds new midi event to the melody with current timestamp
    public void RecordEvent(MPTKEvent midiEvent)
    {
        float currentTime = Time.time - this.startTime;
        this.currentMelody.GetRecordedEvents().Add(new MelodyEvent(currentTime, midiEvent));
        Debug.Log("Event recorded at time " + currentTime);
    }

    public Melody GetMelody()
    {
        return this.currentMelody;
    }
}
