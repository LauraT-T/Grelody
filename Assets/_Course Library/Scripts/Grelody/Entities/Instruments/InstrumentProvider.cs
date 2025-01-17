
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InstrumentProvider {

    private Dictionary<InstrumentType, Instrument> instruments;

    public InstrumentProvider()
    {
        this.instruments = new Dictionary<InstrumentType, Instrument>() {
            {InstrumentType.PIANO, new Instrument(InstrumentType.PIANO, 1, false)},
            {InstrumentType.GUITAR, new Instrument(InstrumentType.GUITAR, 25, false)},
            {InstrumentType.STRINGS, new Instrument(InstrumentType.STRINGS, 43, false)},
            {InstrumentType.TRUMPET, new Instrument(InstrumentType.TRUMPET, 57, false)},

            /* In GM standard MIDI files, channel 9 is reserved for percussion instruments only.
            Notes recorded on channel 9 always produce percussion sounds.
            Each distinct note number specifies a unique percussive instrument, rather than the sound's pitch.
            (Source: https://en.wikipedia.org/wiki/General_MIDI) 
            --> This is why as a midi instrument value 0 is specified (invalid value, not needed as the midi note value determines the instrument)*/
            {InstrumentType.DRUMS, new Instrument(InstrumentType.DRUMS, 0, true)},
        }; 
    }

    // Get the instrument of the specified type
    public Instrument GetInstrument(InstrumentType type)
    {
        return this.instruments[type];
    }

}