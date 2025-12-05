using UnityEngine;

/// <summary>
/// Handles grid-based player movement and input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Map Boundaries")]
    [SerializeField] private bool useMapBounds = true;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    /// <summary>
    /// Public property to control whether the player can move.
    /// </summary>
    public bool CanMove { get; set; } = true;

    private bool isMoving = false;
    private Vector2 input;
    private Vector3 targetPosition;
    private Vector2 lastMoveInput;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        if (isMoving)
        {
            MovePlayer();
        }
        else if (CanMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                targetPosition = transform.position + new Vector3(input.x, input.y, 0);

                if (IsWalkable(targetPosition))
                {
                    isMoving = true;
                    lastMoveInput = input;

                    if (input.x < 0) spriteRenderer.flipX = true;
                    if (input.x > 0) spriteRenderer.flipX = false;
                }
            }
        }

        UpdateAnimator();
    }

    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (useMapBounds)
        {
            if (targetPos.x < minBounds.x || targetPos.x > maxBounds.x ||
                targetPos.y < minBounds.y || targetPos.y > maxBounds.y)
            {
                return false;
            }
        }

        if (Physics2D.OverlapCircle(targetPos, 0.2f, obstacleLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void UpdateAnimator()
    {
        float currentSpeed = isMoving ? 1f : 0f;

        animator.SetFloat("moveX", input.x);
        animator.SetFloat("moveY", input.y);
        animator.SetFloat("speed", currentSpeed);
        animator.SetFloat("lastMoveX", lastMoveInput.x);
        animator.SetFloat("lastMoveY", lastMoveInput.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (useMapBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}