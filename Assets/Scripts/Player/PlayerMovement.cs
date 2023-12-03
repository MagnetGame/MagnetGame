using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float move;
    private bool isGrounded;
    private bool isOnInteractable;
    private bool hitInteractable;
    private Rigidbody2D rb;

    public Transform feetPosition;
    public float groundCheckDistance;
    public float raycastRange;
    public LayerMask groundLayer;
    public LayerMask interactableObjectsLayer;
    
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public AudioClip collid;
    private AudioSource effectsAudioSource; 

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
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(move));
        rb.velocity = new Vector2(move * speed, rb.velocity.y); //retains previous rigidbody Y veloctiy

        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move = -1f; // Set move to -1 when the left arrow key is pressed
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move = 1f; // Set move to 1 when the right arrow key is pressed
        }
        else
        {
            move = 0f; // Set move to 0 when no arrow key is pressed
        }*/

        if (move < 0) //flip sprite depending on which dir we move
        {
            spriteRenderer.flipX = false;
        }
        else if (move > 0) 
        {
            spriteRenderer.flipX = true;
        }

        bool hitLeft = Physics2D.Raycast(feetPosition.position, Vector2.left, raycastRange, LayerMask.GetMask("Wall")).collider != null;
        bool hitRight = Physics2D.Raycast(feetPosition.position, Vector2.right, raycastRange, LayerMask.GetMask("Wall")).collider != null;
        bool hitUp = Physics2D.Raycast(feetPosition.position, Vector2.up, raycastRange * 4, LayerMask.GetMask("Ceiling")).collider != null;
        bool hitDown = Physics2D.Raycast(feetPosition.position, Vector2.down, raycastRange, LayerMask.GetMask("Ground")).collider != null;

        //Debug.Log("player walk state is: " + currentWalkState);

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

        if ((hitLeft || hitRight) )
        {
            // If there's a wall on the left or right, allow vertical movement
            

            if ((Input.GetKeyUp("space")))
            {
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

            //if not wall walking
            //isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckDistance, groundLayer); //old version 
            isGrounded = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckDistance, groundLayer).collider != null;  // NOTE PLAYER HAS TO BE ON GROUND LAYER!
            //Debug.Log("are we on ground? " + isGrounded);
            //Debug.DrawRay(feetPosition.position, Vector2.down * groundCheckDistance, Color.red);

            //isOnInteractable = Physics.OverlapCircle(feetPosition.position, groundCheckCircleRadius, interactableObjectsLayer); //old ver using ciricle
            hitInteractable = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckDistance, interactableObjectsLayer).collider != null;
            //Debug.Log("Interactable obj below" + hitInteractable);

            if((Input.GetKeyUp("space") && isGrounded) || (hitInteractable && Input.GetKeyUp("space"))) //old ver  if ( (Input.GetKeyUp("space") && isGrounded ) || (isOnInteractable && Input.GetKeyUp("space"))) 
            {
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Vector2.up.y), ForceMode2D.Impulse);
                //rb.velocity = Vector2.up * jumpForce;  //only change Y velocity while not changing the x velocty, cuase vector2.up 
            }
            else if ((hitInteractable && Input.GetKeyUp("space")))
            {
                //rb.velocity = Vector2.up * jumpForce;
            }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && isGrounded == false)
        {
            isGrounded = true;
            effectsAudioSource.PlayOneShot(collid);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Exit")
        {
            Debug.Log("found the exit");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Exit")
        {
            Debug.Log("stayed in exit zone");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

}
