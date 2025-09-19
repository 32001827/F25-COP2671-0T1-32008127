using UnityEngine;

public class MoveForward : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public float speed = 40.0f;

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.forward * Time.deltaTime * speed); 

    }
}
