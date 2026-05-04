using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomPotHandler : MonoBehaviour
{

    private float dt;
    [Header("Pot Variables")]
    [SerializeField] float potFillLevel = 0f; // 0 = empty, 1 = full
    [SerializeField] float potTemperature = 0f; // 0 = cold, 1 = hot, 2 = waay too hot (Visual cue for picking up the pot)
    private float heatRate = 0.05f; // Rate at which the pot heats up
    private float coolRate = 0.02f; // Rate at which the pot cools down
    private float fillRate = 0.1f; // Rate at which the pot fills with water
    private bool isPouredWater = false; // Whether water is being poured into the pot
    private bool isOnStove = false; // Whether the pot is on the stove

    [SerializeField] private float pourDelay = 0.2f;
    private float pourDelayTimer = 0f; // Timer to track how long hot water has been poured in for gradual filling
    [Header("Visuals")]
    [Header("References")]
    [SerializeField] private GameObject hotWaterPrefab;
    [SerializeField] private GameObject pourPoint;
    [SerializeField] private MeshRenderer potRenderer; // Reference to the pot's material for visual updates
    [SerializeField] private ParticleSystem smokeEffect; // Reference to the smoke effect for visual feedback when overheating
    [SerializeField] private MeshRenderer teaLiquidRenderer; // Reference to the tea liquid's material for visual updates

    void Start()
    {
        dt = Time.deltaTime;
    }

    void Update()
    {
        dt = Time.deltaTime;
        HandleTemperature();
        HandleWaterPoured();
        HandleVisuals();
        CheckUpright();
        pourDelayTimer -= dt;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ProcessIngredient(ingredient.ingredientName);
            if (ingredient.ingredientName == "Stove")
            {
                isOnStove = true;
                Debug.Log("Pot placed on stove.");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            StopPouring();
            if (ingredient.ingredientName == "Stove")
            {
                isOnStove = false;
                Debug.Log("Pot removed from stove.");
            }
        }
    }

    void HandleTemperature()
    {
        if (isOnStove && potFillLevel > 0f)
        {
            potTemperature += heatRate * dt; // Increase temperature over time
            potTemperature = Mathf.Clamp(potTemperature, 0f, 2f); // Clamp between 0 and 2
        }
        else
        {
            potTemperature -= coolRate * dt; // Cool down when not on stove
            potTemperature = Mathf.Clamp(potTemperature, 0f, 2f); // Clamp between 0 and 2
        }

        if (potTemperature > 0.8f)
        {
            pourPoint.SetActive(true); // Available to pour when hot enough
            smokeEffect.gameObject.SetActive(true); // Show smoke effect when overheating
            smokeEffect.Play(); // Play smoke effect when overheating
        }
        else
        {
            smokeEffect.Stop(); // Stop smoke effect when not overheating
            smokeEffect.gameObject.SetActive(false); // Hide smoke effect when not overheating
            pourPoint.SetActive(false); // Not available to pour when too cold
        }
    }

    void CheckUpright()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0.7f) // Check if the pot is tilted more than ~45 degrees
        {
            if (potFillLevel > 0f && potTemperature > 0.8f) // Only spill if there's something in the pot
            {
                Debug.Log("Pot is tilted! Spilling contents...");
                if (hotWaterPrefab != null && pourPoint != null && pourDelayTimer <= 0f)
                {
                    Instantiate(hotWaterPrefab, pourPoint.transform.position, pourPoint.transform.rotation);
                    pourDelayTimer = pourDelay; // Reset the delay timer
                }
            }



        }
    }


    void HandleVisuals()
    {
        // Update pot color based on temperature (for example, from blue to red)
        Color potColor = Color.Lerp(Color.white, Color.red, potTemperature / 2f);
        potRenderer.material.color = potColor;
        if (teaLiquidRenderer != null)
        {
            Material waterMat = teaLiquidRenderer.material;
            Vector3 fillAmount = (potFillLevel > 0.05f) ? new Vector3(0, potFillLevel * (0.58f - 0.47f) - 0.58f, 0f) : new Vector3(0f, -10f, 0f); // Only show fill when above a certain threshold to avoid visual glitches
            waterMat.SetVector("_FillAmount", fillAmount);
        }
        if (potTemperature > 1.5f)
        {
            smokeEffect.gameObject.SetActive(true);
            if (!smokeEffect.isPlaying)
            {
                smokeEffect.Play();
            }
        }
        else
        {
            smokeEffect.gameObject.SetActive(false);
            if (smokeEffect.isPlaying)
            {
                smokeEffect.Stop();
            }
        }
    }

    void HandleWaterPoured()
    {
        if (isPouredWater)
        {
            potFillLevel += fillRate * dt; // Increase fill level over time
            potFillLevel = Mathf.Clamp(potFillLevel, 0f, 1f); // Clamp between 0 and 1
            Debug.Log("Filling Pot: Fill Level = " + potFillLevel);
        }
    }
    void StopPouring()
    {
        isPouredWater = false;
    }

    void ProcessIngredient(string name)
    {
        if (name == "Water" && potFillLevel < 1f)
        {
            isPouredWater = true;
        }
        else if (name == "Stove")
        {
            isOnStove = true;
            Debug.Log("Pot placed on stove.");
        }
    }
}
