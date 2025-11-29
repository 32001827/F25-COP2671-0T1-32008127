using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("The transform the camera should follow (e.g., the player).")]
    [SerializeField] private Transform target;

    [Tooltip("The offset from the target's position.")]
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        // check if player exists
        if (target != null)
        {
            Vector3 newPosition = target.position + offset;
            newPosition.z = transform.position.z; // Keep original Z-axis

            transform.position = newPosition;
        }
    }
}