using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRClickArea : MonoBehaviour
{
    Material mat;
    private AudioSource audioSource;
    private AudioClip clickSound;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        audioSource = GetComponent<AudioSource>();
        clickSound = audioSource.clip;
        OnHoverExit(); // Set initial state to not hovered
    }

    public void OnHoverEnter()
    {
        mat.SetColor("_EmissionColor", Color.green); // Green emission on hover
        mat.SetFloat("_EmissionIntensity", 1f); // Set emission intensity to 1 when hovered
    }
    public void OnHoverExit()
    {
        mat.SetColor("_EmissionColor", Color.red); // Red emission when not hovered
        mat.SetFloat("_EmissionIntensity", 0f); // Set emission intensity to 0 when not hovered
    }
    public void OnClick()
    {
        if (clickSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Randomize pitch for variety
            audioSource.PlayOneShot(clickSound); // Play click sound on click
        }
        else
        {
            Debug.LogWarning("No click sound assigned to VRClickArea.");
        }
    }
}
