using System.Collections;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.XR.Interaction.Toolkit;


public class ReplayMelody : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation while playing
    private SnowmanInventoryManager inventoryManager;
    private IEnumerator spinCoroutine = null;
    private readonly Vector3 VINYL_POSITION = new Vector3(0.55f, 0.76f, -0.84f);
    private SnowmanMelody snowmanMelody;


    private void Start()
    {
        // Find the inventory manager in the scene
        inventoryManager = FindObjectOfType<SnowmanInventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snowman"))
        {
            Debug.Log("Snowman collided with Grammophone");

            // Find the corresponding SnowmanMelody in the inventory
            this.snowmanMelody = inventoryManager.FindSnowmanMelody(other.gameObject);

            if (this.snowmanMelody != null)
            {

                DisableGrabbing(other);
                StartCoroutine(EnableGrabbingAfterDelay(other, 1f));

                // Move the snowman to the vinyl
                if (other.transform.position != this.VINYL_POSITION)
                {
                    other.transform.position = this.VINYL_POSITION;
                }

                // Start playing the melody
                MidiStreamPlayer midiPlayer = FindObjectOfType<MidiStreamPlayer>();
                this.snowmanMelody.GetMelody().StartReplay(this, midiPlayer);

                // Start spinning the snowman
                if(this.spinCoroutine != null) {
                    StopCoroutine(this.spinCoroutine);
                }

                this.spinCoroutine = SpinSnowman(other.transform);
                StartCoroutine(this.spinCoroutine);
                StopSpinningOnGrab(other);
            }
        }
    }

    private IEnumerator SpinSnowman(Transform snowman)
    {
        while (true)
        {
            if (snowman == null) yield break;
            snowman.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Disable grabbing temporarily so that snowman can spawn on top of vinyl
    private void DisableGrabbing(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.interactionManager.SelectExit(grabInteractable.firstInteractorSelecting, grabInteractable);
            grabInteractable.enabled = false; 
        }
    }

    // Re-enable grabbing
    private void EnableGrabbing(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = true;
        }
    }

    private IEnumerator EnableGrabbingAfterDelay(Collider obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableGrabbing(obj);
    }

    // Stop spinning
    private void StopSpinningOnGrab(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Stop spinning
        StopCoroutine(this.spinCoroutine);

        // Stop melody replay
        if(this.snowmanMelody != null) {
            MidiStreamPlayer midiPlayer = FindObjectOfType<MidiStreamPlayer>();
            this.snowmanMelody.GetMelody().StopReplay(this);
        }
    }
}
