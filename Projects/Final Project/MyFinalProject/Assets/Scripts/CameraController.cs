using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // player
    public Vector3 offset; // just in case

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
        // check if player exists
        if (target != null)
        {
            // compenstate for offset, just in case
            // keep z the same since 2D
            Vector3 newPosition = target.position + offset;
            newPosition.z = transform.position.z;

            // update
            transform.position = newPosition;
        }
    }
}
