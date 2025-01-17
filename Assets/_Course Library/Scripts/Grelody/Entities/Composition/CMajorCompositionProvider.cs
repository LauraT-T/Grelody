
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CMajorCompositionProvider : CompositionProvider {


    public CMajorCompositionProvider() : base(

            MusicalKey.MAJOR, // The key is major

            new List<int> { 60, 62, 64, 65, 67, 69, 71 }, // C Major Scale

            // I-V-vi-IV chord progression for the C Major scale
            new List<List<int>>
            {
                new List<int> { 60, 64, 67 }, // C Major
                new List<int> { 62, 67, 71 }, // G Major
                new List<int> { 60, 64, 69 }, // A Minor
                new List<int> { 65, 69, 72 }, // F Major
            },

            // Notes and passing notes for each chord of the chord progression
            new List<List<int>>
            {
                new List<int> { 60, 62, 64, 65, 67 }, // C Major
                new List<int> { 60, 62, 67, 69, 71 }, // G Major
                new List<int> { 60, 62, 64, 69, 71 }, // A Minor
                new List<int> { 65, 67, 69, 71, 72 }, // F Major
            }, 

            // Chord bass notes
            new List<int> { 36, 43, 45, 41 }
        )
    {}

}