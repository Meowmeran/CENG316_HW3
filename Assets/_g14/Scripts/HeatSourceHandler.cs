using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSourceHandler : MonoBehaviour
{
    private bool isHeating = false; // Whether this heat source is currently heating
    [SerializeField] private MeshRenderer[] heatSourceRenderers; // Reference to the heat source's material for visual updates
    private Collider heatSourceCollider; // Reference to the collider component for detecting pot placement

    private float dt = 0f; // Delta time for consistent updates
    [SerializeField] private int targetEmissionIntensity = 2; // Target emission intensity when fully heated
    private float emissionIntensity = 85f; // Intensity of the heat source's emission for visual feedback
    void Start()
    {
        heatSourceCollider = GetComponent<Collider>();
        if (heatSourceCollider == null)
        {
            Debug.LogError("HeatSourceHandler requires a Collider component for detecting pot placement.");
        }
    }


    void Update()
    {
        dt = Time.deltaTime;
        HandleVisuals();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BottomPot"))
        {
            BottomPotHandler pot = other.GetComponent<BottomPotHandler>();
            if (pot != null)
            {
               // toggleHeating(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BottomPot"))
        {
            BottomPotHandler pot = other.GetComponent<BottomPotHandler>();
            if (pot != null)
            {
               // toggleHeating(false);
            }
        }
    }

    
    public void toggleHeating()
    {
        toggleHeating(!isHeating);
    }
    private void toggleHeating(bool heating)
    {
        isHeating = heating;
        heatSourceCollider.enabled = heating; // Enable collider when heating, disable when not heating
        Debug.Log("Heat source " + (heating ? "started" : "stopped") + " heating.");
    }

    private void HandleVisuals()
    {
        foreach (MeshRenderer renderer in heatSourceRenderers)
        {
            Material mat = renderer.material;
            float timeTakesToFullHeat = 1f; // Time in seconds to reach full heat
            float emissionChangeRate = 1f / timeTakesToFullHeat; // Rate at which emission changes per second
            
            if (isHeating)
            {
                emissionIntensity += emissionChangeRate * dt;
            }
            else
            {
                emissionIntensity -= emissionChangeRate * dt;
            }
            emissionIntensity = Mathf.Clamp(emissionIntensity, 0, targetEmissionIntensity);
            mat.SetColor("Color_91233a34c0ca401bad93f6030e558b35", Color.red); // Red emission when heating
            mat.SetFloat("Vector1_396f178539e24f0eb2def9703fe21579", isHeating ? emissionIntensity : 0);
        }

    }
}
