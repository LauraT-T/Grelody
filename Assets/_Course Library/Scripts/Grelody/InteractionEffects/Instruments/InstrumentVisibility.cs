using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/*
Attached to each instrument.
Makes them invisible when being added to the grammophone
and visible again when being pulled out towards the left, surpassing the specified x-threshold.
*/
public class InstrumentVisibility : MonoBehaviour
{
    public string invisibleLayer = "InvisibleLayer"; // Name of the invisible layer
    public string defaultLayer = "Default"; // Name of the visible layer
    public float xThreshold = 0.0f; // X position where object becomes visible
    public GameObject invisibleInstrumentsParent; // Parent to all invisible instruments

    private int invisibleLayerIndex;
    private int defaultLayerIndex;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private InstrumentManager instrumentManager;

    void Start()
    {
        // Find instrument manager
        this.instrumentManager = (InstrumentManager)FindFirstObjectByType<InstrumentManager>();

        // Get layer indices
        invisibleLayerIndex = LayerMask.NameToLayer(invisibleLayer);
        defaultLayerIndex = LayerMask.NameToLayer(defaultLayer);

        // Start the object on the invisible layer
        SetLayer(gameObject, invisibleLayerIndex);

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnUngrabbed);

        Debug.Log($"Instrument: {gameObject.name}, Layer: {gameObject.layer}, Parent: {transform.parent}");
    }

    void Update()
    {

        // Ensure all objects start invisible
        if (gameObject.layer != invisibleLayerIndex && transform.parent == invisibleInstrumentsParent.transform)
        {
            SetLayer(gameObject, invisibleLayerIndex);
        }

        // If object surpasses the xThreshold or is not added to grammophone, make it visible
        if (transform.position.x < xThreshold || transform.parent != this.invisibleInstrumentsParent.transform)
        {
            if(gameObject.layer != defaultLayerIndex) {
                SetLayer(gameObject, defaultLayerIndex);

                // Remove instrument from melody
                switch (gameObject.tag)
                {
                    case "Piano":
                        this.instrumentManager.RemoveFromGrammophone(InstrumentType.PIANO);
                        break;

                    case "Guitar":
                        this.instrumentManager.RemoveFromGrammophone(InstrumentType.GUITAR);
                        break;

                    case "Violin":
                        this.instrumentManager.RemoveFromGrammophone(InstrumentType.STRINGS);
                        break;

                    case "Trumpet":
                        this.instrumentManager.RemoveFromGrammophone(InstrumentType.TRUMPET);
                        break;

                    case "Drums":
                        this.instrumentManager.RemoveFromGrammophone(InstrumentType.DRUMS);
                        break;

                    default:
                        Debug.Log("Unknown Tag");
                        break;
                }
            }
        }
    }

    // Set the layer for the object and its children
    private void SetLayer(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }


    // Make it possible to add instrument to grammophone when being grabbed by removing parent
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if(transform.parent == this.invisibleInstrumentsParent.transform) {
            transform.SetParent(null, true);
        }
    }

    // Ensure parent stays null and does not reset when releasing the instrument
     private void OnUngrabbed(SelectExitEventArgs args)
    {
        if(transform.parent == this.invisibleInstrumentsParent.transform) {
            transform.SetParent(null, true);
        }
    }
}
