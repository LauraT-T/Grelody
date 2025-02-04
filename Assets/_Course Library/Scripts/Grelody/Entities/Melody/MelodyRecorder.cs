
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

class MelodyRecorder
{
    private Melody currentMelody; // The melody currently being recorded
    private float startTime; // The start time of the recording

    private float pauseTimeStart; // The start time of the pause

    private float pauseTimeLenght; // The general ending time of the pause
    private bool isRecording;

    public MelodyRecorder() 
    {
        this.currentMelody = new Melody();
        this.isRecording = false;
        this.pauseTimeLenght = 0f;
        this.pauseTimeStart = 0f;
    }

    // Creates new melody and determines start time of recording
    public void StartRecording()
    {
        // If a melody has been recorded already, create a new one
        if(this.currentMelody.GetRecordedEvents().Count != 0) {
            this.currentMelody = new Melody();
            this.pauseTimeLenght = 0f;
            this.pauseTimeStart = 0f;
        }
        startTime = Time.time;
        this.isRecording = true;
        Debug.Log("Recording started");
    }

    // Adds new midi event to the melody with current timestamp
    public void RecordEvent(MPTKEvent midiEvent)
    {
        float currentTime = Time.time - this.startTime - this.pauseTimeLenght;
        this.currentMelody.GetRecordedEvents().Add(new MelodyEvent(currentTime, midiEvent));
        Debug.Log("Event recorded at time " + currentTime);
    }

    public Melody GetMelody()
    {
        return this.currentMelody;
    }

    public bool GetIsRecording()
    {
        return this.isRecording;
    }

    public void SetIsRecording(bool isRecording)
    {
        this.isRecording = isRecording;
    }

    public void SetPauseTimeStart(float time) {
        this.pauseTimeStart = time;
    }

    public void CalculatePauseLength(float pause) {
        this.pauseTimeLenght += pause - this.pauseTimeStart;
    }
}
