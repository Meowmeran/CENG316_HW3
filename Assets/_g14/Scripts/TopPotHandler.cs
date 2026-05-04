using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPotHandler : MonoBehaviour
{
    private float dt;
    [Header("Pot Variables")]
    [SerializeField] float potFillLevel = 0f; // 0 = empty, 1 = full
    [SerializeField] float potTemperature = 0f; // 0 = cold, 1 = hot, 2 = waay too hot (Visual cue for picking up the pot)
    [SerializeField] private float teaConcentration = 0f; // 0 = no tea, 1 = fully brewed tea 
    private float heatRate = 0.05f; // Rate at which the pot heats up
    private float coolRate = 0.02f; // Rate at which the pot cools down
    private float fillRate = 0.1f; // Rate at which the pot fills with hot water
    private float fillHeatRate = 0.4f; // Rate at which the pot heats up when hot water is being poured in (Hot water heats up the pot as it fills, but also cools it down, so this is a balancing act to make sure the pot doesn't just instantly heat up to max when you pour hot water in) 
    private bool isPouredWater = false; // Whether hot water is being poured into the pot
    private bool isOnStove = false; // Whether the pot is on the stove
    [SerializeField] private float pourDelay = 0.2f;
    private float pourDelayTimer = 0f; // Timer to track how long hot water has been poured in for gradual filling
    [Header("Visuals")]
    [Header("References")]
    [SerializeField] private GameObject teaLiquidPrefab;
    [SerializeField] private GameObject lidObject;
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
            ingredient.destroyIngredient();
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
        if (isOnStove)
        {
            potTemperature += heatRate * dt;
        }
        else
        {
            potTemperature -= coolRate * dt;
        }

        // Clamp the temperature between 0 and 2 for visual feedback purposes
        potTemperature = Mathf.Clamp(potTemperature, 0f, 2f);
    }

    void HandleWaterPoured()
    {
        if (isPouredWater)
        {
            potFillLevel += fillRate * dt; // Increase fill level over time
            potFillLevel = Mathf.Clamp(potFillLevel, 0f, 1f); // Clamp between 0 and 1

            // Adjust temperature based on pouring hot water
            potTemperature += fillHeatRate * dt; // Hot water heats up the pot
            potTemperature = Mathf.Clamp(potTemperature, 0f, 2f); // Clamp between 0 and 2
        }
    }

    void ProcessIngredient(string ingredientName)
    {
        if (ingredientName == "HotWater")
        {
            StartPouring();
        }
        else if (ingredientName == "TeaLeaves")
        {
            Debug.Log("Tea leaves added to the pot.");
            teaConcentration += 0.25f;
        }
    }

    void CheckUpright()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0.7f) // Check if the pot is tilted more than ~45 degrees
        {
            if (potFillLevel > 0f && teaConcentration > 0f && potTemperature > 1.5f) // Only spill if there's something in the pot
            {
                Debug.Log("Pot is tilted! Spilling contents...");
                if (teaLiquidPrefab != null && pourPoint != null && pourDelayTimer <= 0f)
                {
                    teaLiquidPrefab.GetComponent<Ingredient>().setAmount(potFillLevel); // Set the amount of tea in the spilled liquid based on the current fill level of the pot
                    Instantiate(teaLiquidPrefab, pourPoint.transform.position, pourPoint.transform.rotation);
                    pourDelayTimer = pourDelay; // Reset the delay timer
                }
            }



        }
    }

    void StartPouring()
    {
        isPouredWater = true;
        Debug.Log("Started pouring hot water into the pot.");
    }
    void StopPouring()
    {
        isPouredWater = false;
        Debug.Log("Stopped pouring hot water into the pot.");
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
            waterMat.SetColor("_TopColor", Color.Lerp(new Color(0.8f, 0.8f, 0.8f), new Color(0.2f, 0.1f, 0f), teaConcentration)); // Darker color for stronger tea
            waterMat.SetColor("_BottomColor", Color.Lerp(new Color(0.4f, 0.4f, 0.4f), new Color(0.1f, 0.05f, 0f), teaConcentration)); // Darker at the bottom for depth effect
            waterMat.SetColor("_Rim_Color", Color.Lerp(new Color(0.9f, 0.9f, 0.9f), new Color(0.3f, 0.2f, 0.1f), teaConcentration)); // Rim color also changes with tea concentration
            waterMat.SetFloat("_Glossiness", 0.5f + 0.5f * teaConcentration); // More tea makes the liquid glossier for visual appeal
        }

        // Show smoke effect if the pot is overheating
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
}
