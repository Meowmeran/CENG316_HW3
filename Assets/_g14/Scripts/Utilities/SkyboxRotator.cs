using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Light directionalLight;
    private float dt = 0f;
    void Start()
    {
        if (skyboxMaterial == null)
        {
            Debug.LogError("Skybox material not assigned!");
            return;
        }
        if (directionalLight == null)
        {
            Debug.LogError("Directional light not assigned!");
            return;
        }
        UnityEngine.RenderSettings.skybox = skyboxMaterial;     
    }

    void Update()
    {
        dt = Time.deltaTime;
    }
    
    private void LateUpdate()
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Rotation", Time.time * rotationSpeed);
        }
        if (directionalLight != null)
        {
            directionalLight.transform.Rotate(Vector3.up, rotationSpeed * dt);
        }
    }
}
