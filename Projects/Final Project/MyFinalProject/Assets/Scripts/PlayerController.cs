using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move Speed")]
    [SerializeField] private float moveSpeed = 5f;

    /// <summary>
    /// Public property to control whether the player can move.
    /// </summary>
    public bool CanMove { get; set; } = true;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 lastMoveInput;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (!CanMove)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(moveX, moveY);

            if (moveInput.magnitude > 1)
            {
                moveInput.Normalize();
            }
        }

        if (moveInput.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x > 0.1f)
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
        rb.linearVelocity = moveInput * moveSpeed;
    }
}