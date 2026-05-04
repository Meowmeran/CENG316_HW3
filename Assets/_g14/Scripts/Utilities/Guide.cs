
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] private GameObject hintCollider; // Reference to the collider that triggers the guide steps
    [SerializeField] private TextMeshPro guideText; // Reference to the TextMeshPro component for displaying instructions
    [SerializeField] private Transform[] textPositions; // Array of positions for the guide text to appear for each step
    [SerializeField] private string[] guideSteps; // Array of instructional steps to guide the player through the tea-making process
    [SerializeField] private GameObject[] stepHighlights; // Array of GameObjects to highlight for each step (e.g., arrows, outlines)
    [SerializeField] private int currentStep = 0; // Index to track the current step in the guide

    [SerializeField] private float upOffset = 2f; // Distance to move the guide text up from the target position


    private float textWriteProgress = 0f; // Progress of text writing effect
    [SerializeField] private float textWriteSpeed = 0.05f; // Speed at which text is revealed

    private float positionLerpProgress = 0f; // Progress of position lerping
    [SerializeField] private float positionLerpSpeed = 0.5f; // Speed at which the guide text moves to the new position


    private float dt = 0f; // Delta time for timing the text writing effect

    void Start()
    {
        if (guideSteps.Length == 0 || textPositions.Length == 0 || stepHighlights.Length == 0)
        {
            Debug.LogError("Guide: Please assign guide steps, text positions, and step highlights in the inspector.");
            return;
        }
        if (textPositions.Length != guideSteps.Length)
        {
            Debug.LogError("Guide: The number of text positions must match the number of guide steps.");
            return;
        }
        dt = Time.deltaTime;
        UpdateGuide();
        setAllHighlightsInactive();
        stepHighlights[currentStep].SetActive(true);
        MoveHintColliderToNextStep(); // Position the hint collider at the first step
    }

    void Update()
    {
        dt = Time.deltaTime;
        if (textWriteProgress < 1f)
        {
            textWriteProgress += textWriteSpeed * dt;
            int charactersToShow = Mathf.FloorToInt(textWriteProgress * guideSteps[currentStep].Length);
            guideText.text = guideSteps[currentStep].Substring(0, charactersToShow);
        }
        LarpGuidePosition();
        LookAtCamera();
    }

    public void NextStep()
    {
        if (currentStep < guideSteps.Length - 1)
        {
            positionLerpProgress = 0f; // Reset position lerp progress for the new step
            currentStep++;
            UpdateGuide();
        }
    }
    public void UpdateGuide()
    {
        textWriteProgress = 0f;
        guideText.text = guideSteps[currentStep];
        guideText.transform.position = textPositions[currentStep].position;
        for (int i = 0; i < stepHighlights.Length; i++)
        {
            stepHighlights[i].SetActive(i == currentStep); // Highlight only the current step
        }
    }
    void LarpGuidePosition()
    {
        if (positionLerpProgress < 1f)
        {
            positionLerpProgress += dt * positionLerpSpeed;
            guideText.transform.position = Vector3.Lerp(guideText.transform.position, getXAmountUp(textPositions[currentStep], upOffset), positionLerpProgress);
        }
        else
        {
            guideText.transform.position = getXAmountUp(textPositions[currentStep], upOffset); // Ensure it ends at the exact position
        }
    }

    Vector3 getXAmountUp(Transform target, float distance)
    {
        return target.position + Vector3.up * distance;
    }

    void LookAtCamera()
    {
        guideText.transform.LookAt(Camera.main.transform);
        guideText.transform.rotation = Quaternion.LookRotation(guideText.transform.position - Camera.main.transform.position);
    }

    void setAllHighlightsInactive()
    {
        foreach (var highlight in stepHighlights)
        {
            highlight.SetActive(false);
        }
    }

    public void OnHintTrigger() // Will be fired by XR Interaction when the player enters the hint collider. This method should be linked to the appropriate event in the XR Interaction system.
    {
        NextStep(); // Move to the next step in the guide
        MoveHintColliderToNextStep(); // Move the hint collider to the position of the next step   
    }
    void MoveHintColliderToNextStep()
    {
        if (currentStep < textPositions.Length)
        {
            hintCollider.transform.position = textPositions[currentStep].position; // Move the hint collider to the position of the next step
        }
    }


}

