using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position + offset;
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
    }
}
