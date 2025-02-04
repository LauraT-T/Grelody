using System.Collections;
using UnityEngine;
using MidiPlayerTK;
using UnityEngine.XR.Interaction.Toolkit;

public class BackToInventoryCollision : MonoBehaviour
{
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Snowman")) 
        { 
            // Play sound
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            Debug.Log("Collision detected: Snowman - BackToInventory.");

            SnowmanInventoryManager inventoryManager = FindObjectOfType<SnowmanInventoryManager>();
            SnowmanMelody snowmanMelody = inventoryManager.FindSnowmanMelody(other.gameObject);

            if (snowmanMelody != null)
            {
                // Move the snowman to its saved inventory position
                DisableGrabbing(other.gameObject);

                other.gameObject.transform.localPosition = snowmanMelody.GetInventoryPosition();
                Debug.Log("Snowman moved back to inventory at position: " + snowmanMelody.GetInventoryPosition());

                // Ensure Collider is enabled
                Collider col = other.gameObject.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                }

                // Set the parent GameObject (whole button consisting of two snowmen) inactive 
                if (transform.parent != null) 
                {
                    transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("No parent found to deactivate.");
                }
            }
            else
            {
                Debug.Log("No corresponding SnowmanMelody found.");
            }

            EnableGrabbing(other.gameObject);
        }
    }

    // Disable grabbing temporarily so that snowman can move properly
    private void DisableGrabbing(GameObject obj)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component is missing");
            return;
        }

        // Ensure the XRInteractionManager is assigned
        if (grabInteractable.interactionManager == null)
        {
            XRInteractionManager xrManager = FindObjectOfType<XRInteractionManager>();
            if (xrManager != null)
            {
                grabInteractable.interactionManager = xrManager;
                Debug.Log("Assigned XRInteractionManager");
            }
            else
            {
                Debug.LogError("No XRInteractionManager found in the scene");
                return;
            }
        }

        // Check if the object is currently being held before calling SelectExit()
        if (grabInteractable.firstInteractorSelecting != null)
        {
            grabInteractable.interactionManager.SelectExit(grabInteractable.firstInteractorSelecting, grabInteractable);
            Debug.Log("Successfully detached snowman from interactor.");
        }
        else
        {
            Debug.LogWarning("Cannot call SelectExit() because firstInteractorSelecting is null.");
        } 

        grabInteractable.enabled = false;
        Debug.Log("Grabbing disabled.");
    }


    // Re-enable grabbing
    private void EnableGrabbing(GameObject obj)
    {
        Debug.Log("Enable grabbing is called.");
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component is missing");
            return;
        }

        grabInteractable.enabled = true;

        // Ensure that we only call SelectEnter if there is an interactor
        if (grabInteractable.firstInteractorSelecting != null)
        {
            grabInteractable.interactionManager.SelectEnter(grabInteractable.firstInteractorSelecting, grabInteractable);
            Debug.Log("Grabbing re-enabled and object reassigned.");
        }
        else
        {
            Debug.Log("Grabbing re-enabled, but no interactor detected yet.");
        }
    }
}
