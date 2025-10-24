using Unity.Hierarchy;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // get the bits
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // give us some settings for speed
    [Header("Move Speed")]
    public float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // idiot proof some things
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // translate to game vector
        moveInput = new Vector2(moveX, moveY);

        // normalize to account for math issues with triangles
        if (moveInput.magnitude > 1 )
        {
            moveInput.Normalize();
        }
    }

    private void FixedUpdate()
    {
        // move the player
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
