using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    private Material material;
    private Color originalEmissionColor;
    public bool isGlowing = true;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;

            // Ensure the shader supports emission
            if (material.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = material.GetColor("_EmissionColor");
            }
        }

        if(isGlowing) {
            EnableGlow();
        }
    }

    public void EnableGlow()
    {
        if (material != null && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.white * 2f); // Increase intensity
            isGlowing = true;
        }
    }

    public void DisableGlow()
    {
        if (material != null && material.HasProperty("_EmissionColor") && isGlowing)
        {
            material.SetColor("_EmissionColor", originalEmissionColor);
            material.DisableKeyword("_EMISSION");
            isGlowing = false;
        }
    }
}
