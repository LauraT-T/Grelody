using UnityEngine;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;

public class InvisibleLayerHandler : MonoBehaviour
{
    public string invisibleLayer = "InvisibleLayer"; // Name of the invisible layer
    public string defaultLayer = "Default"; // Name of the visible layer
    public float xThreshold = 0.0f; // X position where object becomes visible

    private int invisibleLayerIndex;
    private int defaultLayerIndex;
    private Renderer objRenderer;

    public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor leftHandInteractor;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        // Get layer indices
        invisibleLayerIndex = LayerMask.NameToLayer(invisibleLayer);
        defaultLayerIndex = LayerMask.NameToLayer(defaultLayer);

        objRenderer = GetComponent<Renderer>();

        // Get XR Grab Interactable component
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component missing on " + gameObject.name);
        }

        // Start the object on the invisible layer
        //SetLayer(gameObject, invisibleLayerIndex);
        //OnLeftFist();
    }

    void Update()
    {
        // If object surpasses the xThreshold, make it visible
        if (transform.position.x < xThreshold)
        {
            SetLayer(gameObject, defaultLayerIndex);
        }
    }

    // Recursively set the layer for the object and its children
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

        // Change color to see if gesture detection worked
        if (objRenderer != null)
        {
            objRenderer.material.color = Color.magenta;
        }

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

        Debug.Log("Grabbed: " + gameObject.name);
    }
}
