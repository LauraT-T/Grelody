
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CompositionProvider {


    // C Major Scale
    List<int> cMajorScale = new List<int> { 60, 62, 64, 65, 67, 69, 71 }; 

    // I-V-vi-IV chord progression for the C Major scale
    List<List<int>> cMajorChords = new List<List<int>>
    {
        new List<int> { 60, 64, 67 }, // C Major
        new List<int> { 62, 67, 71 }, // G Major
        new List<int> { 60, 64, 69 }, // A Minor
        new List<int> { 65, 69, 72 }, // F Major
    };

    // Notes and passing notes for each chord of the chord progression
    List<List<int>> cMajorAllowedNotes = new List<List<int>>
    {
        new List<int> { 60, 62, 64, 65, 67 }, // C Major
        new List<int> { 60, 62, 67, 69, 71 }, // G Major
        new List<int> { 60, 62, 64, 69, 71 }, // A Minor
        new List<int> { 65, 67, 69, 71, 72 }, // F Major
    };



    // Harmonic C minor Scale
    List<int> cMinorScale = new List<int> { 60, 62, 63, 65, 67, 68, 71 }; 

    // i-V-VI-iv chord progression for the C Minor scale
    List<List<int>> cMinorChords = new List<List<int>>
    {
        new List<int> { 60, 63, 67 }, // C Minor
        new List<int> { 62, 67, 71 }, // G Major
        new List<int> { 60, 63, 68 }, // Ab Major
        new List<int> { 65, 68, 72 }, // F Minor
    };

    // Notes and passing notes for each chord of the chord progression
    List<List<int>> cMinorAllowedNotes = new List<List<int>>
    {
        new List<int> { 60, 62, 63, 65, 67 }, // C Minor
        new List<int> { 60, 62, 67, 69, 71 }, // G Major
        new List<int> { 60, 62, 63, 68, 71 }, // Ab Major
        new List<int> { 65, 67, 68, 71, 72 }, // F Minor
    };
}