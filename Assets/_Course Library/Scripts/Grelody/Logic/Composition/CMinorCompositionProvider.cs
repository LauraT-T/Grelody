
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CMinorCompositionProvider : CompositionProvider {


    public CMinorCompositionProvider() : base(

            MusicalKey.MINOR, // The key is minor

            new List<int> { 60, 62, 63, 65, 67, 68, 71 }, // Harmonic C minor Scale

            // i-V-VI-iv chord progression for the C Minor scale
            new List<List<int>>
            {
                new List<int> { 60, 63, 67 }, // C Minor
                new List<int> { 62, 67, 71 }, // G Major
                new List<int> { 60, 63, 68 }, // Ab Major
                new List<int> { 65, 68, 72 }, // F Minor
            },

            // Notes and passing notes for each chord of the chord progression
            new List<List<int>>
            {
                new List<int> { 60, 62, 63, 65, 67 }, // C Minor
                new List<int> { 60, 62, 67, 69, 71 }, // G Major
                new List<int> { 60, 62, 63, 68, 71 }, // Ab Major
                new List<int> { 65, 67, 68, 71, 72 }, // F Minor
            }
        )
    {}

}