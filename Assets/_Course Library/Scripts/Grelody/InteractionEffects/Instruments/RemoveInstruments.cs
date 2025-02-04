using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

/*
Invisible cube.
Can be grabbed by making a left fist gesture.
Is released by ending the left fist gesture.
Moving the cube moves all added instruments, making it possible to remove them from the grammophone.
*/
public class RemoveInstruments : MonoBehaviour
{
    public string invisibleLayer = "InvisibleLayer"; // Name of the invisible layer
    public string defaultLayer = "Default"; // Name of the visible layer

    private int invisibleLayerIndex;
    private int defaultLayerIndex;

    public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor leftHandInteractor;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    
    public InvisibleInstruments invisibleInstruments; // Reference to manage instrument movement


    void Start()
    {
        // Get layer indices
        invisibleLayerIndex = LayerMask.NameToLayer(invisibleLayer);
        defaultLayerIndex = LayerMask.NameToLayer(defaultLayer);

        // Get XR Grab Interactable component
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component missing on " + gameObject.name);
        }
    }

    void Update()
    {
    }

    public void OnLeftFist()
    {
        if (grabInteractable == null)
        {
            Debug.LogError("Cannot grab object. XRGrabInteractable is missing.");
            return;
        }

        // Ensure instruments do not move along when cube jumps into player's hand
        invisibleInstruments.StopMovement(); 

        // Grab object with left hand
        leftHandInteractor.interactionManager.SelectEnter(
            (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)leftHandInteractor,
            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable
        );

        // Delay before re-enabling movement of instruments
        Invoke(nameof(EnableMovementAfterDelay), 0.3f);
    }

    private void EnableMovementAfterDelay()
    {
        invisibleInstruments.EnableMovement();
    }

    public void OnLeftFistEnded()
    {
        if (leftHandInteractor == null || grabInteractable == null)
        {
            Debug.LogWarning("Left hand interactor or grab interactable is missing.");
            return;
        }

        // Release object
        leftHandInteractor.interactionManager.SelectExit(
            (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)leftHandInteractor,
            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable
        );
    }
}

