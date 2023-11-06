using UnityEngine;
using UnityEngine.Playables;

public class PlayerMagnetism : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 2f;
    public float maxAttractionDistance = 1f; // Maximum distance for attraction.
    public float breakingForce = .1f; // Force applied to break attraction.
    public LineRenderer lineRendererPrefab;
    private LineRenderer currentLineRenderer;
    private playerState currentState = playerState.Neutral;
    private playerWalkState currentWalkState = playerWalkState.Ground;
    private Rigidbody2D playerRigidbody;  // Rigidbody for the player.
    public SpriteRenderer spriteRenderer; //sprite rendered for player

    private enum playerState
    {
        NorthMode,
        SouthMode,
        Neutral
    }

    private enum playerWalkState
    {
        Ground,
        WallRight,
        WallLeft,
        Ceiling
    }

    

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>(); // Assuming the player has the Rigidbody2D component on the same GameObject as this script.
    }

    void Update()
    {
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

        //Todo change/rotate sprite
        switch (currentWalkState)
        {
            case playerWalkState.Ground:
                break;
            case playerWalkState.WallRight:
                break;
            case playerWalkState.WallLeft:
                break;
            case playerWalkState.Ceiling:
                break;
        }


        
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

                    Vector2 force = direction.normalized * (magnetForce * 0.5f); // Half the force.
                    playerRigidbody.AddForce(force, ForceMode2D.Impulse);
                    //Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                    if (objectRigidbody != null)
                    {
                        objectRigidbody.AddForce(-force, ForceMode2D.Impulse); // Apply equal and opposite force to the attracted object.
                    }
                    //end of attraction code

                    //begin break force code
                    Vector2 breakDirectionVector = Vector2.zero; 
                    if (Input.GetKeyUp("space"))
                    {
                        // Set the breakDirection to the upward direction. //TODO change this logic by moving it above (first in else if) and use break
                        breakDirectionVector = new Vector2(0.0f, 1.0f); // Upwards direction

                        Debug.Log("Apply breaking force");
                        if (objectRigidbody != null)
                        {
                            Debug.Log("Using breaking force of " + breakingForce);
                            Vector2 breakForce = breakDirectionVector.normalized * breakingForce;
                            playerRigidbody.AddForce(breakForce, ForceMode2D.Impulse);
                        }
                    }
                    
                }

            }
        }
        else // in neutral draw distance circle?
        {
            if (currentLineRenderer != null)
            {
                Destroy(currentLineRenderer.gameObject);
            }
        }
    }
}
