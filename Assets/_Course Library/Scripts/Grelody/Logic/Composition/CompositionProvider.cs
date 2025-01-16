
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class CompositionProvider {

    private MusicalKey key;
    private List<int> scale;

    private List<List<int>> chords;
    private List<List<int>> allowedNotes;
    private List<int> bassNotes;

    public CompositionProvider(MusicalKey key, List<int> scale, List<List<int>> chords, List<List<int>> allowedNotes, List<int> bassNotes)
    {
        this.key = key;
        this.scale = scale;
        this.chords = chords;
        this.allowedNotes = allowedNotes;
        this.bassNotes = bassNotes;
    }

    // Get key
    public MusicalKey GetKey()
    {
        return this.key;
    }

    // Getter method for the scale
    public List<int> GetScale()
    {
        return this.scale;
    }

    // Getter method for the chords
    public List<List<int>> GetChords()
    {
        return this.chords;
    }

    // Getter method for the allowed notes
    public List<List<int>> GetAllowedNotes()
    {
        return this.allowedNotes;
    }

    public List<int> GetBassNotes() 
    {
        return this.bassNotes;
    }

}