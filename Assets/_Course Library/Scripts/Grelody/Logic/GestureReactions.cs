using UnityEngine;

public class GestureReactions : MonoBehaviour
{
    //Variables
    private MelodyChordTest melodyChordTest;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        melodyChordTest = (MelodyChordTest)FindFirstObjectByType<MelodyChordTest>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // reaction to detection of thumbsup
    public void OnThumbsUp() {
        Debug.Log("ThumbsUp detected.");
        melodyChordTest.changeToMajor();
    }

    // reaction to detection of thumbsdown
    public void OnThumbsDown() {
        Debug.Log("ThumbsDown detected.");
        melodyChordTest.changeToMinor();
    }

    public void OnLeftFist() {
        Debug.Log("LeftFist detected");

        // Test cube, can be deleted later
        InvisibleLayerHandler[] invisibleObjects = FindObjectsOfType<InvisibleLayerHandler>();

        foreach (InvisibleLayerHandler invisibleLayerHandler in invisibleObjects)
        {
            invisibleLayerHandler.OnLeftFist();
        }

        // Cube for removing all the instruments
        RemoveInstruments removeInstruments = FindFirstObjectByType<RemoveInstruments>();

        if (removeInstruments != null) 
        {
            removeInstruments.OnLeftFist();
        }
        else 
        {
            Debug.LogWarning("RemoveInstruments script not found in the scene.");
        }

    }

     public void OnLeftFistEnded() {
        Debug.Log("LeftFistEnded detected");

        // test cube, can be deletes later
        InvisibleLayerHandler[] invisibleObjects = FindObjectsOfType<InvisibleLayerHandler>();

        foreach (InvisibleLayerHandler invisibleLayerHandler in invisibleObjects)
        {
            invisibleLayerHandler.OnLeftFistEnded();
        }


        // Cube for removing all the instruments
        RemoveInstruments removeInstruments = FindFirstObjectByType<RemoveInstruments>();

        if (removeInstruments != null) 
        {
            removeInstruments.OnLeftFistEnded();
        }
        else 
        {
            Debug.LogWarning("RemoveInstruments script not found in the scene.");
        }

    }

    public void OnRightFist() {
        Debug.Log("RightFist detected");
        
        melodyChordTest.ContinueMusic();
    }

    public void OnRightFistEnded() {

        Debug.Log("RightFist ended");

        melodyChordTest.PauseMusic();

    }

    // generic name due to placeholder gesture
    public void EndMelody() {
        Debug.Log("Gesture detected, melody ended");
        melodyChordTest.StopMusic();
    }
}
