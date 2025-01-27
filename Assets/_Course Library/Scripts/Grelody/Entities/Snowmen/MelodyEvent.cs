
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

class MelodyEvent
{
    public float timeStamp; // Time when the event occurs
    public MPTKEvent midiEvent; // The MIDI event (note, volume, etc.)

    public MelodyEvent(float timeStamp, MPTKEvent midiEvent) {
        this.timeStamp = timeStamp;
        this.midiEvent = midiEvent;
    }
}
