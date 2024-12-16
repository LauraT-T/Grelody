using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiPlayerTK;

public class PlayPauseButtonsMaestro : MonoBehaviour
{

    // Buttons
    public Button playButton;
    public Button pauseButton;

    // MidiPlayerGlobal is a singleton: only one instance can be created. Making static to have only one reference.
    MidiFilePlayer midiFilePlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playButton.onClick.AddListener(PlayTrack);
        pauseButton.onClick.AddListener(PauseTrack);
        
    }

    private void Awake()
        {
            Debug.Log("Awake: dynamically add MidiFilePlayer component");

            // MidiPlayerGlobal is a singleton: only one instance can be created. 
            if (MidiPlayerGlobal.Instance == null)
                gameObject.AddComponent<MidiPlayerGlobal>();

            // When running, this component will be added to this gameObject. Set essential parameters.
            midiFilePlayer = gameObject.AddComponent<MidiFilePlayer>();
            midiFilePlayer.MPTK_CorePlayer = true;
            midiFilePlayer.MPTK_DirectSendToPlayer = true;

            // Select a MIDI from the MIDI DB (with exact name)
            midiFilePlayer.MPTK_MidiName = "Bach - Fugue";
        }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Play the melody
    private void PlayTrack() 
    {
        Debug.Log("Play track");
        midiFilePlayer.MPTK_Play();
        
    }

    // Pause the melody
    private void PauseTrack() 
    {
        Debug.Log("Pause track");
         midiFilePlayer.MPTK_Pause();
        
    }
}
