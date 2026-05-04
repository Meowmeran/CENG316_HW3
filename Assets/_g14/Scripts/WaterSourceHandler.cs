using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourceHandler : MonoBehaviour
{
    private bool isPouringWater = false; // Whether water is being poured from this source
    [SerializeField] private GameObject waterSourceObject; // Reference to the water source object (e.g., kettle or tap)
    void Start()
    {
        isPouringWater = false;
        if (waterSourceObject != null)
        {
            waterSourceObject.SetActive(false); // Ensure water source is initially inactive
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cup"))
        {
            CupHandler cup = other.GetComponent<CupHandler>();
            if (cup != null)
            {
                togglePouringWater(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cup"))
        {
            CupHandler cup = other.GetComponent<CupHandler>();
            if (cup != null)
            {
                togglePouringWater(false);
            }
        }
    }

    public void togglePouringWater()
    {
        togglePouringWater(!isPouringWater);
    }
    
    public void togglePouringWater(bool isPouring)
    {
        isPouringWater = isPouring;
        if (isPouringWater)
        {
            waterSourceObject.SetActive(true);
            Debug.Log("Started pouring water from " + waterSourceObject.name);
        }
        else
        {
            waterSourceObject.SetActive(false);
            Debug.Log("Stopped pouring water from " + waterSourceObject.name);
        }
    }
    
}
