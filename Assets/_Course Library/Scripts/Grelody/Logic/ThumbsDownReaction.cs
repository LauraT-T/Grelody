using UnityEngine;

public class ThumbsDownReaction : MonoBehaviour
{

    // Variables
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

    public void OnHandGesture() {
        Debug.Log("ThumbsDown detected.");
        melodyChordTest.changeToMinor();
    }
}
