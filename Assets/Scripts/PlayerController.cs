using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float magnetRadius = 5f;
    public float magnetForce = 2f;
    public float maxAttractionDistance = 1f;
    public float breakingForce = 0.1f;
    public float raycastDistance = 0.5f;

    public LineRenderer lineRendererPrefab;
    private LineRenderer currentLineRenderer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool hitInteractable;

    private playerState currentState = playerState.Neutral;
    private playerWalkState currentWalkState = playerWalkState.Ground;

    private enum playerState
    {
        NorthMode,
        SouthMode,
        Neutral
    }

    private enum playerWalkState
    {
        Ground,
        WallOnRight,
        WallOnLeft,
        Ceiling
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent < SpriteRenderer();
    }

    private void Update()
    {
        // Handle player state changes...
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentState = playerState.NorthMode;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentState = playerState.SouthMode;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentState = playerState.Neutral;
        }

        // Handle player movement...
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (move < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (move > 0)
        {
            spriteRenderer.flipX = true;
        }

        // Handle player jump...
        isGrounded = Physics2D.OverlapCircle(transform.position, raycastDistance, LayerMask.GetMask("Ground"));

        if ((Input.GetKeyUp("space") && isGrounded) || (hitInteractable && Input.GetKeyUp("space")))
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        // Handle magnetism...
        RaycastHit hitInfo;
        int layerMask = LayerMask.GetMask("Wall", "Ceiling");

        Debug.DrawRay(transform.position, Vector3.right * raycastDistance, Color.red);
        if (Physics.Raycast(transform.position, Vector3.right, out hitInfo, raycastDistance, layerMask))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                currentWalkState = playerWalkState.WallOnRight;
            }
            else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ceiling"))
            {
                currentWalkState = playerWalkState.Ceiling;
            }
        }

        // Handle magnetism logic...
        if (currentState == playerState.NorthMode || currentState == playerState.SouthMode)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            foreach (Collider2D collider in colliders)
            {
                if ((currentState == playerState.NorthMode && collider.CompareTag("NorthPolarity")) ||
                    (currentState == playerState.SouthMode && collider.CompareTag("SouthPolarity")))
                {
                    // Handle push logic...
                }
                else if ((currentState == playerState.SouthMode && collider.CompareTag("NorthPolarity")) ||
                         (currentState == playerState.NorthMode && collider.CompareTag("SouthPolarity")))
                {
                    // Handle pull logic...
                }
            }
        }
        else // In neutral state
        {
            if (currentLineRenderer != null)
            {
                Destroy(currentLineRenderer.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    }
}
