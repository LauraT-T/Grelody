using UnityEngine;

public class SnowmanRotationFix : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    /*
    Quick and dirty fix for the bug which causes the snowman's rotation to reset when being grabbed
    (Causing it to look away from the player)
    */
    void Update()
    {
        transform.rotation = initialRotation;
    }
}
