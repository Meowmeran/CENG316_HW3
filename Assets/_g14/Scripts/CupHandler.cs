using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupHandler : MonoBehaviour
{
    private float dt;
    [Header("Cup Variables")]
    float cupFillLevel = 0f; // 0 = empty, 1 = full
    float teaStrength = 0f; // 0 = no tea, 1 = full strength
    float sugarLevel = 0f; // 0 = no sugar, 1 = one spoon, 2 = two spoons
    float lemonlevel = 0f; // 0 = no lemon, 1 = max lemon

    [Header("References")]
    [SerializeField] private MeshRenderer cupRenderer; // Reference to the cup's material for visual updates
    [SerializeField] private ParticleSystem steamEffect; // Reference to a steam particle system for visual effect when hot
    [SerializeField] private AudioSource drinkAudioSource; // Reference to an audio source for drinking sound effects

    [Header("Pouring Rates")]
    [SerializeField] private float teaPouringRate = 0.05f; // Rate at which tea is poured into the cup
    [SerializeField] private float hotWaterPouringRate = 0.05f; // Rate at which hot water is poured into the cup


    [Header("Drinking Variables")]
    [SerializeField] private float drinkingThreshold = 0.1f; // Distance threshold for drinking
    [SerializeField] private float drinkingRate = 0.1f; // Rate at which the cup is consumed when drinking
    [SerializeField] private float sipCooldown = 0.75f; // Cooldown time between sips to prevent rapid drinking
    [SerializeField] private AudioClip sipSound; // Sound effect for sipping
    void Start()
    {
        dt = Time.deltaTime;
        UpdateVisuals(); // Initialize visuals based on starting variables
    }

    void Update()
    {
        dt = Time.deltaTime;
        HandlePourHotWater();
        HandlePourTea();
        HandleDrinkTea();
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ProcessIngredient(ingredient.ingredientName);
            ingredient.destroyIngredient();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            ProcessIngredient(ingredient.ingredientName);
        }
    }

    void ProcessIngredient(string name)
    {
        if (name == "TeaLiquid" && cupFillLevel < 1f)
        {
            cupFillLevel += teaPouringRate / 5f; // Increase fill level based on pouring rate
            teaStrength += teaPouringRate; // Increase tea strength as tea is poured in
            cupFillLevel = Mathf.Clamp(cupFillLevel, 0f, 1f); // Clamp between 0 and 1
            teaStrength = Mathf.Clamp(teaStrength, 0f, 1f); // Clamp between 0 and 1
            UpdateVisuals();
            Debug.Log("Pouring Tea: Fill Level = " + cupFillLevel + ", Tea Strength = " + teaStrength);
        }
        else if (name == "Sugar" && sugarLevel < 2f)
        {
            sugarLevel += 0.2f; // Each sugar addition increases the level by 0.2   
            Debug.Log("Sugar Added: Sugar Level = " + sugarLevel);
        }
        else if (name == "Lemon" && lemonlevel < 1f)
        {
            lemonlevel += 0.07f; // Each lemon addition increases the level by 0.1
            Debug.Log("Lemon Added: Lemon Level = " + lemonlevel);
        }
        else if (name == "HotWater" && cupFillLevel < 1f)
        {
            cupFillLevel += hotWaterPouringRate; // Increase fill level based on pouring rate
            cupFillLevel = Mathf.Clamp(cupFillLevel, 0f, 1f); // Clamp between 0 and 1
            UpdateVisuals();
            Debug.Log("Pouring Hot Water: Fill Level = " + cupFillLevel);
        }
        else
        {
            return;
        }
        UpdateVisuals();
    }

    void HandlePourHotWater()
    {
        cupFillLevel = Mathf.Clamp(cupFillLevel, 0f, 1f); // Clamp between 0 and 1
        UpdateVisuals();

    }
    void HandlePourTea()
    {
        cupFillLevel = Mathf.Clamp(cupFillLevel, 0f, 1f); // Clamp between 0 and 1
        teaStrength = Mathf.Clamp(teaStrength, 0f, 1f); // Clamp between 0 and 1
        UpdateVisuals();

    }

    void HandleDrinkTea()
    {
        if (isCloseToMouth(drinkingThreshold))
        {
            if (!checkUpright())
            {
                return;
            }
            cupFillLevel -= drinkingRate * dt; // Decrease fill level based on drinking rate
            teaStrength -= (drinkingRate / 2f) * dt; // Decrease tea strength as tea is consumed, but at a slower rate than fill level to simulate flavor lingering
            teaStrength = Mathf.Clamp(teaStrength, 0f, 1f); // Clamp between 0 and 1
            sugarLevel -= (drinkingRate / 3f) * dt; // Decrease sugar level as tea is consumed, but at a slower rate than fill level to simulate sweetness lingering
            sugarLevel = Mathf.Clamp(sugarLevel, 0f, 1f); // Clamp between 0 and 1
            lemonlevel -= (drinkingRate / 4f) * dt; // Decrease lemon level as tea is consumed, but at a slower rate than fill level to simulate tartness lingering
            lemonlevel = Mathf.Clamp(lemonlevel, 0f, 1f); // Clamp between 0 and 1
            if (cupFillLevel <= 0f)
            {
                cupFillLevel = 0f;
                teaStrength = 0f;
                sugarLevel = 0f;
                lemonlevel = 0f;
            }


            UpdateVisuals();
            if (!drinkAudioSource.isPlaying)
            {
                drinkAudioSource.PlayOneShot(sipSound); // Play sipping sound effect
            }
        }
    }
    bool checkUpright()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        return angle < 45f; // Consider the cup upright if it's within 45 degrees of vertical
    }
    bool isCloseToMouth(float threshold = 0.1f)
    {
        Camera mainCamera = Camera.main;
        float distanceToMouth = Vector3.Distance(transform.position, mainCamera.transform.position);
        return distanceToMouth < threshold; // Adjust the threshold as needed
    }

    void UpdateVisuals()
    {
        Material mat = cupRenderer.material;

        Vector3 fillAmount = (cupFillLevel > 0.05f) ? new Vector3(0, cupFillLevel * (0.58f - 0.47f) - 0.58f, 0f) : new Vector3(0f, -10f, 0f); // Only show fill when above a certain threshold to avoid visual glitches
        mat.SetVector("_FillAmount", fillAmount);
        Color teaColor = Color.Lerp(Color.cyan, new Color(0.8f, 0f, 0f), teaStrength / cupFillLevel); // Darker color for stronger tea
        teaColor = Color.Lerp(teaColor, Color.white, sugarLevel); // Add a lighter tint based on sugar level
        teaColor = Color.Lerp(teaColor, new Color(1f, 1f, 0.1f), lemonlevel); // Add a tint based on lemon level
        Color teaTint = new Color(0.5f, 0.3f, 0.1f) * lemonlevel; // Add a tint based on lemon level
        teaColor += teaTint; // Combine tea color with lemon tint
        mat.SetColor("_TopColor", teaColor);
        mat.SetColor("_BottomColor", teaColor * 0.5f); // Darker at the bottom for depth effect
        mat.SetColor("_FoamColor", Color.white * (sugarLevel * 0.5f)); // More sugar creates more foam
        mat.SetColor("_Rim_Color", Color.white * (sugarLevel * 0.5f)); // More sugar creates a brighter rim

    }
}
