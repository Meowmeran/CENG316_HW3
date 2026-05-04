using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string ingredientName; // e.g., "InstantCoffee", "Water", "Sugar"
    [SerializeField] private bool willDestroyOnContact = false; // Whether the ingredient should be destroyed when it interacts with the pot
    [SerializeField] private float amount = 0.25f;
    public void destroyIngredient()
    {
        if (willDestroyOnContact)
        {
            Destroy(gameObject);
        }
    }

    public void setAmount(float newAmount)
    {
        amount = newAmount;
    }

    public float getAmount()
    {
        return amount;
    }
}
