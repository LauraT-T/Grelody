/* using UnityEngine;

public class SnowmanRotationFix : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    
    //Quick and dirty fix for the bug which causes the snowman's rotation to reset when being grabbed
    //(Causing it to look away from the player)
    
    void Update()
    {
        transform.rotation = initialRotation;
    }

    
}
 */

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

 //Quick and dirty fix for the bug which causes the snowman's rotation to reset when being grabbed
//(Causing it to look away from the player)
public class SnowmanRotationFix : MonoBehaviour
{
    private Quaternion initialRotation;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        initialRotation = transform.rotation; // Store rotation when first grabbed
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Ensure rotation stays the same after release
        transform.rotation = initialRotation;
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
