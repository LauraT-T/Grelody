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

        // Find all instruments on an invisible layer which are grabbbed using the left fist gesture
        InvisibleLayerHandler[] invisibleObjects = FindObjectsOfType<InvisibleLayerHandler>();

        foreach (InvisibleLayerHandler invisibleLayerHandler in invisibleObjects)
        {
            invisibleLayerHandler.OnLeftFist();
        }

    }

     public void OnLeftFistEnded() {
        Debug.Log("LeftFistEnded detected");

        // Ungrab instruments
        InvisibleLayerHandler[] invisibleObjects = FindObjectsOfType<InvisibleLayerHandler>();

        foreach (InvisibleLayerHandler invisibleLayerHandler in invisibleObjects)
        {
            invisibleLayerHandler.OnLeftFistEnded();
        }

    }
}
