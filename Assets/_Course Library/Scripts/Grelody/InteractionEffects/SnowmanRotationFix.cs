using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;


//Quick and dirty fix for the bug which causes the snowman's rotation to reset when being grabbed
//(Causing it to look away from the player)
public class SnowmanRotationFix : MonoBehaviour
{
    private Quaternion initialRotation;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private SnowmanInventoryManager inventoryManager;
    private GrammophoneGlow grammophoneGlow;
    public GameObject backToInventoryButton; // Move Snowman back to inventory

    void Start()
    {
        this.backToInventoryButton = FindObjectsOfType<GameObject>(true)
                        .FirstOrDefault(obj => obj.name == "BackToInventory");

        if(this.backToInventoryButton == null) {
            Debug.Log("Button not found.");
        }

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        this.grammophoneGlow = (GrammophoneGlow)FindFirstObjectByType<GrammophoneGlow>();
        grammophoneGlow.EnableVinylGlow();
        grammophoneGlow.DisableVinylGlow();

        // Find the inventory manager in the scene
        this.inventoryManager = FindObjectOfType<SnowmanInventoryManager>();
        
        
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        initialRotation = transform.rotation; // Store rotation when first grabbed
       
       // Check if snowman has been saved to inventory
        SnowmanMelody snowmanMelody = inventoryManager.FindSnowmanMelody(gameObject);

        if (snowmanMelody != null) {

            // Vinyl glows if melody can be replayed by moving the snowman there
            grammophoneGlow.EnableVinylGlow();

            // Show button to move snowman back into inventory
            if (!this.backToInventoryButton.activeSelf) 
            {
               this.backToInventoryButton.SetActive(true);
            }
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Ensure rotation stays the same after release
        transform.rotation = initialRotation;

        // Turn off vinyl glow
        grammophoneGlow.DisableVinylGlow();
    }

    void Update()
    {
        // Keep the stored rotation while being held
        if (grabInteractable.isSelected)
        {
            transform.rotation = initialRotation;
        }
    }

}
