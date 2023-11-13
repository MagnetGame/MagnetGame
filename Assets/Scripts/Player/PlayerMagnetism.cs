using UnityEngine;
using UnityEngine.Playables;

public class PlayerMagnetism : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 2f;
    public float maxAttractionDistance = 1f; // Maximum distance for attraction.
    public float breakingForce = .1f; // Force applied to break attraction.
    public float raycastDistance = .5f;
    public float wallDetectDistane = .2f;
    public SpriteRenderer spriteRenderer; //sprite rendered for player

    public Sprite neutral, north, south;
    public Animator animator;
    public ParticleSystem northParticles;
    public ParticleSystem southParticles;

    public LineRenderer lineRendererPrefab;
    private LineRenderer currentLineRenderer;


    private playerState currentState = playerState.Neutral;
    private Rigidbody2D playerRigidbody;  // Rigidbody for the player.
    

    private enum playerState
    {
        NorthMode,
        SouthMode,
        Neutral
    }


    private playerState currentState = playerState.Neutral;
    private playerState previousState = playerState.Neutral;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>(); // Assuming the player has the Rigidbody2D component on the same GameObject as this script.
        animator.SetBool("isNeutral", true);
    }

    void Update()// do not used fixed update no matter what here
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentState = playerState.NorthMode;
            if(previousState != currentState)
            {
                northParticles.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentState = playerState.SouthMode;
            if (previousState != currentState)
            {
                southParticles.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentState = playerState.Neutral;
            if (previousState != currentState)
            {
                northParticles.Play();
                southParticles.Play();
            }
        }
        UpdateStateAnimation(currentState);
        previousState = currentState;

        //TODO change player sprite for Polarity here 
        switch (currentState)
        {
            case playerState.NorthMode:
                //spriteRenderer.sprite = Resources.Load<Sprite>("NorthPlayer"); // Change to "NorthPlayer" sprite.
                break;
            case playerState.SouthMode:
                //spriteRenderer.sprite = Resources.Load<Sprite>("SouthPlayer"); // Change to "SouthPlayer" sprite.
                break;
            case playerState.Neutral:
                // You can set a default sprite or leave it as is.
                break;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //checking pull or push objects
        if (currentState == playerState.NorthMode || currentState == playerState.SouthMode)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            foreach (Collider2D collider in colliders)
            {
                //TODO wall walking, hmm

                //regular polarity
                if ((currentState == playerState.NorthMode && collider.CompareTag("NorthPolarity")) || (currentState == playerState.SouthMode && collider.CompareTag("SouthPolarity"))) // handles push 
                {
                    Vector2 direction = transform.position - collider.transform.position;
                    float distance = direction.magnitude;

                    Vector2 force = direction.normalized * (magnetForce * 0.5f); // Half the force to player 
                    playerRigidbody.AddForce(force, ForceMode2D.Impulse);

                    Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                    if (objectRigidbody != null)
                    {
                        objectRigidbody.AddForce(-force, ForceMode2D.Impulse); // half hte force on object
                    }
                }
                else if ((currentState == playerState.SouthMode && collider.CompareTag("NorthPolarity")) || (currentState == playerState.NorthMode && collider.CompareTag("SouthPolarity"))) // handles pull
                {                    
                    Vector2 direction = collider.transform.position - transform.position;
                    float distance = direction.magnitude;
                    Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();

                    //begin break force code
                    Vector2 breakDirectionVector = Vector2.zero;
                    if (Input.GetKeyUp("space"))
                    {
                        breakDirectionVector = new Vector2(0.0f, 1.0f); // Upwards direction

                        //Debug.Log("Apply breaking force");
                        if (objectRigidbody != null)
                        {
                            Debug.Log("Using breaking force of " + breakingForce);
                            Vector2 breakForce = breakDirectionVector.normalized * breakingForce;
                            playerRigidbody.AddForce(breakForce, ForceMode2D.Impulse);
                            continue;
                        }
                    }

                    Vector2 force = direction.normalized * (magnetForce * 0.5f); // Half the force.
                    playerRigidbody.AddForce(force, ForceMode2D.Impulse);
                    //Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                    if (objectRigidbody != null)
                    {
                        objectRigidbody.AddForce(-force, ForceMode2D.Impulse); // Apply equal and opposite force to the attracted object.
                    }
                    //end of attraction code

                }

            }
        }
        
    }

    private void UpdateStateAnimation(playerState mode)
    {
        switch (mode)
        {
            case playerState.NorthMode:
                animator.SetBool("isNorth", true);
                animator.SetBool("isNeutral", false);
                animator.SetBool("isSouth", false);
                break;
            case playerState.Neutral:
                animator.SetBool("isNorth", false);
                animator.SetBool("isNeutral", true);
                animator.SetBool("isSouth", false);
                break;
            case playerState.SouthMode:
                animator.SetBool("isNorth", false);
                animator.SetBool("isNeutral", false);
                animator.SetBool("isSouth", true);
                break;
        }
    }
}
