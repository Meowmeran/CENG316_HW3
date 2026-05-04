using UnityEngine;

public class TeaHandler : MonoBehaviour
{
    [SerializeField] private GameObject teaPrefab;
    [SerializeField] private Transform spawnpoint;
    
    [SerializeField] private float dropDelay = 0.5f; 
    private float dropDelayTimer = 0f; // Timer to track how long since the last tea was spawned
    private float dt;

    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("TeaHandler requires a Rigidbody component for shake detection.");
        }
    }

    void Update()
    {
        dt = Time.deltaTime;
        CheckUpright();
        dropDelayTimer -= dt; // Decrease the drop delay timer over time
    }
    


    void SpawnTea()
    {
        Instantiate(teaPrefab, spawnpoint.position, spawnpoint.rotation);
    }

    void CheckUpright()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0.7f) // Check if the pot is tilted more than ~45 degrees
        {
            if (dropDelayTimer <= 0f)
            {
                SpawnTea();
                dropDelayTimer = dropDelay; // Reset the delay timer
            }
        }
    }


/*
    void detectShake()
    {
        if (rb == null) return;

        // Calculate the change in velocity
        float currentVelocity = rb.velocity.magnitude;
        float deltaVelocity = Mathf.Abs(currentVelocity - dVelocity);
        float acceleration = deltaVelocity / dt;
    
        // Check if the change in velocity exceeds the threshold
        if (acceleration > minAccelerationToSpawn)
        {
            SpawnTea();
        }

        // Update the previous velocity for the next frame
        dVelocity = currentVelocity;
    }
    */
}
