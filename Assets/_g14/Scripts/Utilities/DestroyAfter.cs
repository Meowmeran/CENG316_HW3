using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 2f; // Time in seconds before the object is d
    
    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}
