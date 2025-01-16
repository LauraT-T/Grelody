
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class CompositionProvider {


    private List<int> scale;

    private List<List<int>> chords;
    private List<List<int>> allowedNotes;

    public CompositionProvider(List<int> scale, List<List<int>> chords, List<List<int>> allowedNotes)
    {
        this.scale = scale;
        this.chords = chords;
        this.allowedNotes = allowedNotes;
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

}