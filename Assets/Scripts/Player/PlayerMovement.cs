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
    private Rigidbody2D rb;

    public Transform feetPosition;
    public float groundCheckCircleRadius;
    public LayerMask groundLayer;
    public LayerMask interactableObjectsLayer;
    
    public SpriteRenderer spriteRenderer;
    public Animator animator;

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

        if (move < 0) //flip sprite depending on which dir we move
        {
            spriteRenderer.flipX = false;
        }
        else if (move > 0) 
        {
            spriteRenderer.flipX = true;
        }

        isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircleRadius, groundLayer);
        //isOnInteractable = Physics.Raycast(feetPosition.position, Vector2.down, groundCheckCircleRadius, interactableObjectsLayer); //todo change this to circle 
        isOnInteractable = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircleRadius, interactableObjectsLayer);

        if ( (Input.GetKeyUp("space") && isGrounded ) || (isOnInteractable && Input.GetKeyUp("space"))) //TODO here we can jump while magnetized
        {
            //Debug.Log("Got Space and not grounded");
            //rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Vector2.up.y), ForceMode2D.Impulse);
            rb.velocity = Vector2.up * jumpForce;
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