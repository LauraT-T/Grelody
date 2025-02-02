
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.InputSystem;

public class Melody
{
    private List<MelodyEvent> recordedEvents;
    private bool isPlaying = false;
    private IEnumerator replayCoroutine;

    public Melody() 
    {
        this.recordedEvents = new List<MelodyEvent>();
        this.replayCoroutine = null;
    }

    // Start melody replay
    public void StartReplay(MonoBehaviour coroutineOwner, MidiStreamPlayer midiStreamPlayer) 
    {
        if (coroutineOwner == null)
        {
            Debug.LogError("Coroutine owner is null. Ensure you're passing a valid MonoBehaviour instance.");
            return;
        }

        this.replayCoroutine = ReplayMelody(midiStreamPlayer);
        coroutineOwner.StartCoroutine(this.replayCoroutine);
    }

    // Stop melody replay
    public void StopReplay(MonoBehaviour coroutineOwner)
    {
        if (coroutineOwner == null)
        {
            Debug.LogError("Coroutine owner is null. Ensure you're passing a valid MonoBehaviour instance.");
            return;
        }

        if(this.replayCoroutine != null) {
            coroutineOwner.StopCoroutine(this.replayCoroutine);
            this.isPlaying = false;
        }
    }

    // Replays the midi events at the specified time stamps
    private IEnumerator ReplayMelody(MidiStreamPlayer midiStreamPlayer)
    {
        // Start replay if not playing yet
        if(!this.isPlaying) {

            this.isPlaying = true;
            Debug.Log("Starting melody replay");
            float playbackStartTime = Time.time;
            foreach (var melodyEvent in this.recordedEvents)
            {
                float waitTime = melodyEvent.timeStamp - (Time.time - playbackStartTime);
                if (waitTime > 0)
                    yield return new WaitForSeconds(waitTime);

                if (melodyEvent.midiEvent != null)
                {
                    Debug.Log("Replaying midiEvent");
                    midiStreamPlayer.MPTK_PlayEvent(melodyEvent.midiEvent);
                }
            }

            this.isPlaying = false;
        }
        
    }

    public List<MelodyEvent> GetRecordedEvents()
    {
        return this.recordedEvents;
    }
}
