using UnityEngine;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI statusText;

    [Header("Recipe State")]
    private bool hasCoffee = false;
    private bool hasWater = false;
    private bool isMixed = false;

    private void Start()
    {
        statusText.text = "Add Instant Coffee";
    }

    // This is triggered when an ingredient gets close to the cup
    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ProcessIngredient(ingredient.ingredientName);
        }
    }

    void ProcessIngredient(string name)
    {
        if (name == "InstantCoffee" && !hasCoffee)
        {
            hasCoffee = true;
            statusText.text = "Great! Now add Hot Water.";
            Debug.Log("Coffee Added");
        }
        else if (name == "Water" && hasCoffee && !hasWater)
        {
            hasWater = true;
            statusText.text = "Now use the Spoon to Stir.";
            Debug.Log("Water Added");
        }
        else if (name == "Spoon" && hasWater && !isMixed)
        {
            isMixed = true;
            statusText.text = "Coffee is Ready! Enjoy!";
            Debug.Log("Mixed");
        }
    }
}