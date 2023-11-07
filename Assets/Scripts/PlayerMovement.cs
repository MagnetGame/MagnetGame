using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float groundCheckCircleRadius;
    public LayerMask groundLayer;
    public LayerMask interactableObjectsLayer;
    
    public SpriteRenderer spriteRenderer;

    
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

        if(move < 0) //flip sprite depending on which dir we move
        {
            spriteRenderer.flipX = false;
        }
        else if (move > 0) 
        {
            spriteRenderer.flipX = true;
        }

        isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircleRadius, groundLayer);
        //isOnInteractable = Physics.OverlapCircle(feetPosition.position, groundCheckCircleRadius, interactableObjectsLayer); //old ver using ciricle

        hitInteractable = Physics2D.Raycast(feetPosition.position, Vector2.down, groundCheckCircleRadius, interactableObjectsLayer).collider != null;
        //Debug.Log("On interactable by raycast?" + hitInteractable);

        if ( (Input.GetKeyUp("space") && isGrounded ) || (hitInteractable && Input.GetKeyUp("space"))) //old ver  if ( (Input.GetKeyUp("space") && isGrounded ) || (isOnInteractable && Input.GetKeyUp("space"))) 
        {
            //rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Vector2.up.y), ForceMode2D.Impulse);
            rb.velocity = Vector2.up * jumpForce;  //only change Y velocity while not changing the x velocty, cuase vector2.up
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }


    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Grounded");
            isGrounded = true;
        } 
    } 

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Not grounded");
            isGrounded = false;
        }
    }
    */
}
