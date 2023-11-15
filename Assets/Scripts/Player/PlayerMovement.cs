using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float move;
    private bool isGrounded;
    private bool hitInteractable;
    private bool wasGrounded = false;
    private bool wasOnInteractable = false;
    private Rigidbody2D rb;

    public Transform feetPosition;
    public float groundCheckCircleRadius;
    public float raycastRange;
    public LayerMask groundLayer;
    public LayerMask interactableObjectsLayer;
    
    public SpriteRenderer spriteRenderer;
    //public Animator animator;

    [SerializeField] private AudioClip floorCollidClip;
    [SerializeField] private AudioClip interactableCollidClip;

    private enum playerWalkState
    {
        Ground,
        WallOnRight,
        WallOnLeft,
        Ceiling
    }

    private playerWalkState currentWalkState = playerWalkState.Ground;

    // Start is called before the first frame update
    void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y); //retains previous rigidbody Y veloctiy
        //animator.SetFloat("speed", Mathf.Abs(move)); 

        if(move < 0) //flip sprite depending on which dir we move
        {
            spriteRenderer.flipX = false;
        }
        else if (move > 0) 
        {
            spriteRenderer.flipX = true;
        }

        bool hitLeft = Physics2D.Raycast(feetPosition.position, Vector2.left, raycastRange, LayerMask.GetMask("Wall")).collider != null;
        bool hitRight = Physics2D.Raycast(feetPosition.position, Vector2.right, raycastRange, LayerMask.GetMask("Wall")).collider != null;
        bool hitUp = Physics2D.Raycast(feetPosition.position, Vector2.up, raycastRange*4, LayerMask.GetMask("Ceiling")).collider != null;
        bool hitDown = Physics2D.Raycast(feetPosition.position, Vector2.down, raycastRange, LayerMask.GetMask("Ground")).collider != null;

        if (hitLeft)
        {
            currentWalkState = playerWalkState.WallOnLeft;
        }
        else if (hitRight)
        {
            currentWalkState = playerWalkState.WallOnRight;
        }
        else if (hitUp)
        {
            currentWalkState = playerWalkState.Ceiling;
        }
        else
        {
            currentWalkState = playerWalkState.Ground;
        }

        if (hitLeft || hitRight)
        {
            // If there's a wall on the left or right, allow vertical movement
            if ((Input.GetKeyUp("space"))){
                rb.velocity = Vector2.up * jumpForce;
            }

            if (Input.GetKey("up"))
            {
                rb.velocity = Vector2.up * speed; // Move up
            }
            else if (Input.GetKey("down"))
            {
                rb.velocity = Vector2.down * speed; // Move down
            }
        }
        //Debug.Log("player walk state is: " + currentWalkState);

        isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircleRadius, groundLayer);
        hitInteractable = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckCircleRadius, interactableObjectsLayer).collider != null;
        //Debug.Log("On interactable by raycast?" + hitInteractable);

        if ( (Input.GetKeyUp("space") && isGrounded ) || (hitInteractable && Input.GetKeyUp("space"))) 
        {
            //rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Vector2.up.y), ForceMode2D.Impulse);
            rb.velocity = Vector2.up * jumpForce;  //only change Y velocity while not changing the x velocty, cuase vector2.up
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        bool isCurrentlyGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircleRadius, groundLayer);
        bool isCurrentlyOnInteractable = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckCircleRadius, interactableObjectsLayer).collider != null;


        if (isCurrentlyGrounded && !wasGrounded)
        {
            // Play the sound effect only when transitioning from not grounded to grounded
            SoundFXManager.Instance.PlaySoundFXClip(floorCollidClip, transform, 0.6f);
        }

        if (isCurrentlyOnInteractable && !wasOnInteractable)
        {
            // Play the sound effect only when transitioning from not on interactable to on interactable
            SoundFXManager.Instance.PlaySoundFXClip(interactableCollidClip, transform, 0.6f);
        }

        wasGrounded = isCurrentlyGrounded;
        wasOnInteractable = isCurrentlyOnInteractable;
    }


    public string GetCurrentWalkState()
    {
        return currentWalkState.ToString();
    }

}
