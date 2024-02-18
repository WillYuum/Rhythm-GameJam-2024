using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [SerializeField] public Material roomTransitionMaterial; // Assign your RoomTransition material here
    [SerializeField] public float transitionSpeed = 0.5f;
    [SerializeField] public float scaleSpeed = 0.1f;

    private float time = 0.0f;

    void Update()
    {
        // Example: Progressively increase time based on speed
        time += Time.deltaTime * transitionSpeed;

        // Example: Oscillate scale between 0.9 and 1.1 based on speed
        float scale = Mathf.Sin(Time.time * scaleSpeed) * 0.1f + 1.0f;

        // Set the time and scale variables in the shader
        roomTransitionMaterial.SetFloat("_CustomTime", time);
        roomTransitionMaterial.SetFloat("_Scale", scale);
    }
}