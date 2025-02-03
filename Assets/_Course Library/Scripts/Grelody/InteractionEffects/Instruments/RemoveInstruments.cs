using UnityEngine;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;

/*
Invisible cube.
Can be grabbed by making a left fist gesture.
Is ungrabbed by ending the left fist gesture.
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

        // Start the object on the invisible layer
        SetLayer(gameObject, invisibleLayerIndex);
    }

    void Update()
    {
    }

    // Set the layer for the object and its children
    private void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
        }
    }
 
    public void OnLeftFist()
    {

        if (grabInteractable == null)
        {
            Debug.LogError("Cannot grab object. XRGrabInteractable is missing.");
            return;
        }

        // Grab object with left hand
        leftHandInteractor.interactionManager.SelectEnter(
        (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)leftHandInteractor, 
        (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable
        );

    }

    public void OnLeftFistEnded() {
        
        // Ungrab object
        if (leftHandInteractor == null || grabInteractable == null)
        {
            Debug.LogWarning("Left hand interactor or grab interactable is missing.");
            return;
        }

        leftHandInteractor.interactionManager.SelectExit(
            (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)leftHandInteractor,
            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable
        );
            
    }
}
