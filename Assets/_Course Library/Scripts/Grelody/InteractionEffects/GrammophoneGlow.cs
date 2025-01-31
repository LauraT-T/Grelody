using UnityEngine;

/*
How to use (to turn on and off vinyl glow):

GrammophoneGlow grammophoneGlow = (GrammophoneGlow)FindFirstObjectByType<GrammophoneGlow>();
grammophoneGlow.EnableVinylGlow();
grammophoneGlow.DisableVinylGlow();

How to use (to turn on and off funnel glow):

GrammophoneGlow grammophoneGlow = (GrammophoneGlow)FindFirstObjectByType<GrammophoneGlow>();
grammophoneGlow.EnableFunnelGlow();
grammophoneGlow.DisableFunnelGlow();

*/
public class GrammophoneGlow : MonoBehaviour
{
    private Renderer objRenderer;
    private Material[] materials;
    private Color[] originalEmissionColors;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();

        if (objRenderer != null)
        {
            materials = objRenderer.materials;
            originalEmissionColors = new Color[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_EmissionColor"))
                {
                    originalEmissionColors[i] = materials[i].GetColor("_EmissionColor");
                }
            }
        }
    }

    // Turn on all glow effects
    public void EnableAllEmission()
    {
        if (materials != null)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_EmissionColor"))
                {
                    materials[i].EnableKeyword("_EMISSION");
                    materials[i].SetColor("_EmissionColor", originalEmissionColors[i]);
                }
            }
        }
    }

    // Turn off all glow effects
    public void DisableAllEmission()
    {
        if (materials != null)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_EmissionColor"))
                {
                    materials[i].DisableKeyword("_EMISSION");
                    materials[i].SetColor("_EmissionColor", Color.black); // Disable emission
                }
            }
        }
    }

    // Turn on vinyl glow
     public void EnableVinylGlow()
    {
        if (materials != null && materials.Length > 0)
        {
            if (materials[0].HasProperty("_EmissionColor"))
            {
                materials[0].EnableKeyword("_EMISSION");
                materials[0].SetColor("_EmissionColor", originalEmissionColors[0]);
                Debug.Log("Vinyl is glowing.");
            }
        }
    }

    // Turn on funnel glow
     public void EnableFunnelGlow()
    {
        if (materials != null && materials.Length > 2)
        {
            if (materials[2].HasProperty("_EmissionColor"))
            {
                materials[2].EnableKeyword("_EMISSION");
                materials[2].SetColor("_EmissionColor", originalEmissionColors[2]);
                Debug.Log("Funnel is glowing.");
            }
        }
    }

    // Turn off vinyl glow
    public void DisableVinylGlow()
    {
        if (materials != null && materials.Length > 0)
        {
            
            if (materials[0].HasProperty("_EmissionColor"))
            {
                materials[0].DisableKeyword("_EMISSION");
                materials[0].SetColor("_EmissionColor", Color.black); // Disable emission
                Debug.Log("Vinyl not glowing anymore.");
            }
        }  else {
            Debug.Log("No vinyl material found.");
        }
    }

    // Turn off funnel glow
    public void DisableFunnelGlow()
    {
        if (materials != null && materials.Length > 2)
        {
            if (materials[2].HasProperty("_EmissionColor"))
            {
                materials[2].DisableKeyword("_EMISSION");
                materials[2].SetColor("_EmissionColor", Color.black);
                Debug.Log("Funnel not glowing anymore.");
            }
        }
        else
        { 
            Debug.Log("No funnel material found.");
        }
    }
}
