using Unity.Hierarchy;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // get the bits
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Animator animator;

    private Vector2 lastMoveInput;

    private SpriteRenderer spriteRenderer;

    // give us some settings for speed
    [Header("Move Speed")]
    public float moveSpeed = 5f;

    public bool canMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        // idiot proof some things
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {

        if (!canMove)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            // get input
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            // translate to game vector
            moveInput = new Vector2(moveX, moveY);

            // normalize to account for math issues with triangles
            if (moveInput.magnitude > 1)
            {
                moveInput.Normalize();
            }
        }

        if(moveInput.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if(moveInput.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }

        if (moveInput.magnitude > 0.1f)
        {
            lastMoveInput = moveInput;
        }

        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);

        animator.SetFloat("speed", moveInput.magnitude);

        animator.SetFloat("lastMoveX", lastMoveInput.x);
        animator.SetFloat("lastMoveY", lastMoveInput.y);
    }

    private void FixedUpdate()
    {
        // move the player
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
