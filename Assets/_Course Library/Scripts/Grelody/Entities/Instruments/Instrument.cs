
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Instrument {

    private InstrumentType type;
    private int midiValue;
    private bool playsRythm;
    

    public Instrument(InstrumentType type, int midiValue, bool playsRythm)
    {
        this.type = type;
        this.midiValue = midiValue;
        this.playsRythm = playsRythm;
    }

    // Get type
    public InstrumentType GetType()
    {
        return this.type;
    }

    // Get corresponding midi value
    public int GetMidiValue()
    {
        return this.midiValue;
    }

    // Get whether the instrument plays a rythm (if false: plays melody, chords or bass line)
    public bool GetPlaysRythm()
    {
        return this.playsRythm;
    }

}