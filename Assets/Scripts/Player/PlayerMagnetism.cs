using System;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMagnetism : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 2f;
    public float maxAttractionDistance = 1f; // Maximum distance for attraction.
    public float breakingForce = .1f; // Force applied to break attraction.
    public string targetLayer = "Ground";
    
    public SpriteRenderer spriteRenderer; //sprite rendered for player
    public Sprite neutral, north, south;

    public LineRenderer lineRendererPrefab;
    private LineRenderer currentLineRenderer;

    private Rigidbody2D playerRigidbody;  // Rigidbody for the player.

    private enum playerState
    {
        NorthMode,
        SouthMode,
        Neutral
    }

    private playerState currentState = playerState.Neutral;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>(); // Assuming the player has the Rigidbody2D component on the same GameObject as this script.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("North Mode");
            currentState = playerState.NorthMode;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("South Mode");
            currentState = playerState.SouthMode;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Neutral Mode");
            currentState = playerState.Neutral;
        }

        //TODO change sprites/animation here
        switch (currentState)
        {
            case playerState.NorthMode:
                //spriteRenderer.sprite = north;  // Change to "NorthPlayer" sprite.
                break;
            case playerState.SouthMode:
                //spriteRenderer.sprite = south; // Change to "SouthPlayer" sprite.
                break;
            case playerState.Neutral:
                //spriteRenderer.sprite = neutral;
                break;
        }


        
        if (currentState == playerState.NorthMode || currentState == playerState.SouthMode)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            GameObject nearestObject = FindNearestObject(colliders);
            foreach (Collider2D collider in colliders)
            {
                //TODO wall walking, hmm
                if (nearestObject != null)
                {
                    WallWalking forceScript = nearestObject.GetComponent<WallWalking>();
                    if (forceScript != null)
                    {
                        Vector2 direction = GetCardinalDirection(nearestObject.transform.position - transform.position);
                        forceScript.ApplyConstantForce(direction);
                    }
                }


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
                    Vector2 breakDirectionVector = Vector2.zero; // Initialize breakDirection to zero initially.
                    if (Input.GetKeyUp("space"))
                    {
                        // Set the breakDirection to the upward direction.
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

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.up, direction.normalized);
        if (angle > -45 && angle <= 45)
        {
            return Vector2.up; // North
        }
        else if (angle > 45 && angle <= 135)
        {
            return Vector2.right; // East
        }
        else if (angle > 135 || angle <= -135)
        {
            return Vector2.down; // South
        }
        else
        {
            return Vector2.left; // West
        }
    }

    private GameObject FindNearestObject(Collider2D[] colliders)
    {
        GameObject nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if ((currentState == playerState.NorthMode && collider.CompareTag("SouthPolarity") && collider.gameObject.layer == LayerMask.NameToLayer("Ground")) ||
                (currentState == playerState.SouthMode && collider.CompareTag("NorthPolarity") && collider.gameObject.layer == LayerMask.NameToLayer("Ground")))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = collider.gameObject;
                }
            }
        }
        return nearestObject;
    }
}
