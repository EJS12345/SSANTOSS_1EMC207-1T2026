using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setup")]
    [Tooltip("Drag your Sneeker object here")]
    public Transform target;

    [Header("Camera Smoothing")]
    [Tooltip("How quickly the camera catches up to the player. Higher is faster.")]
    public float smoothSpeed = 5f;

    private Vector3 offset;

    void Start()
    {
        // This automatically calculates the exact distance and angle 
        // you set in the Unity Editor before pressing Play.
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    // LateUpdate is crucial for cameras. It ensures the camera moves 
    // strictly AFTER the NavMeshAgent has finished moving for the frame.
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            // Smoothly glide the camera to the new position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}