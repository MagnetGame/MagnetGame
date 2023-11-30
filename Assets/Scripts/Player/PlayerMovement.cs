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

    public Transform headPosition;
    public float groundCheckCircleRadius;
    public float raycastRange;
    public LayerMask groundLayer;
    public LayerMask interactableObjectsLayer;
    private Vector2 spriteSize;

    public SpriteRenderer spriteRenderer;
    public Animator playerAnimator;
    public Animator doorAnimator;

    [SerializeField] private AudioClip collid;
    [SerializeField] private AudioClip interactiableCollid;

    private bool wasGrounded = false;
    public ParticleSystem dust;

    private bool hitLeft;
    private bool hitRight;
    private bool hitDown;
    private bool hitUp;

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
            spriteSize = spriteRenderer.bounds.size;
        }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxis("Horizontal"); 
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        playerAnimator.SetFloat("speed", Mathf.Abs(move));

        rb.velocity = new Vector2(move * speed, rb.velocity.y); //retains previous rigidbody Y veloctiy

        if (move < 0) //flip sprite depending on which dir we move
        {
            if (rb.velocity.x > 2)
            {
                CreateDust();
            }
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (move > 0) 
        {
            if (rb.velocity.x > 2)
            {
                CreateDust();
            }
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        isGrounded = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 90, Vector2.down, 1, groundLayer);
        hitInteractable = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 90, Vector2.down, 1, interactableObjectsLayer);


        hitLeft = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Horizontal, 0, Vector2.left, 2, LayerMask.GetMask("Wall"));
        hitRight = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Horizontal, 0, Vector2.right, 2, LayerMask.GetMask("Wall"));
        hitUp = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 0, Vector2.up, 2, LayerMask.GetMask("Ceiling"));
        hitDown = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 0, Vector2.down, 2, LayerMask.GetMask("Ground"));

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
        Debug.Log("player walk state is: " + currentWalkState);

        isGrounded = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 0, Vector2.down, 1, groundLayer);
        hitInteractable = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 0, Vector2.down, 1, interactableObjectsLayer);

        if ( (Input.GetKeyUp("space") && isGrounded ) || (hitInteractable && Input.GetKeyUp("space"))) 
        {
            CreateDust();
            rb.velocity = Vector2.up * jumpForce;  //only change Y velocity while not changing the x velocty, cuase vector2.up
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 0, Vector2.down, 1, groundLayer);
        hitInteractable = Physics2D.CapsuleCast(headPosition.position, spriteSize, CapsuleDirection2D.Vertical, 0, Vector2.down, 1, interactableObjectsLayer);

        if (hitInteractable && !wasGrounded)
        {
            SoundFXManager.Instance.PlaySoundFXClip(interactiableCollid, transform, 0.6f);
        }
        else if (isGrounded && !wasGrounded)
        {
            SoundFXManager.Instance.PlaySoundFXClip(collid, transform, 0.6f);
        }

        wasGrounded = isGrounded || hitInteractable;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Exit")
        {
            Debug.Log("found the exit");
            doorAnimator.SetBool("exiting", true);
            Debug.Log("animator exiting codition is" + doorAnimator.GetBool("exiting"));
         //   SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }


    public string GetCurrentWalkState()
    {
        return currentWalkState.ToString();
    }

    public void CreateDust()
    {
        dust.Play();
    }

}
