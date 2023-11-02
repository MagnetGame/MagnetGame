using UnityEngine;
using UnityEngine.Playables;

public class PlayerMagnetism : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 2f;
    public float maxAttractionDistance = 3f; // Maximum distance for attraction.
    public float breakingForce = 1f; // Force applied to break attraction.
    public LineRenderer lineRendererPrefab;
    private LineRenderer currentLineRenderer;
    private bool isAttracting = false;
    private bool isRepelling = false;
    private Rigidbody2D playerRigidbody;  // Rigidbody for the player.
    public SpriteRenderer spriteRenderer; //sprite rendered for player

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

        //TODO change sprites here
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

        if (currentState == playerState.NorthMode || currentState == playerState.SouthMode)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            foreach (Collider2D collider in colliders)
            {
                if ((currentState == playerState.NorthMode && collider.CompareTag("NorthPolarity")) || (currentState == playerState.SouthMode && collider.CompareTag("SouthPolarity"))) // handles push 
                {
                    Debug.Log("North Mode");

                    Vector2 direction = transform.position - collider.transform.position;
                    float distance = direction.magnitude;

                    if (distance > maxAttractionDistance)
                    {
                        // If the object is too far away, apply a breaking force to release it.
                        Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                        if (objectRigidbody != null)
                        {
                            Vector2 breakForce = direction.normalized * breakingForce;
                            objectRigidbody.AddForce(breakForce, ForceMode2D.Impulse);
                        }
                    }
                    else //out side max attraction distance then we try to apply force
                    {
                        Vector2 force = direction.normalized * (magnetForce * 0.5f); // Half the force to player 
                        playerRigidbody.AddForce(force, ForceMode2D.Impulse);

                        Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                        if (objectRigidbody != null)
                        {
                            objectRigidbody.AddForce(-force, ForceMode2D.Impulse); // half hte force on object
                        }
                    }
                }
                
                else if ((currentState == playerState.SouthMode && collider.CompareTag("NorthPolarity")) || (currentState == playerState.NorthMode && collider.CompareTag("SouthPolarity"))) // handles pull
                {
                    Debug.Log("South Mode");

                    Vector2 direction = collider.transform.position - transform.position;
                    float distance = direction.magnitude;
                    if (distance > maxAttractionDistance)
                    {
                        // If the object is too far away, apply a breaking force to release it.
                        Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                        if (objectRigidbody != null)
                        {
                            Vector2 breakForce = direction.normalized * breakingForce;
                            objectRigidbody.AddForce(breakForce, ForceMode2D.Impulse);
                        }
                    }
                    else
                    {
                        Vector2 force = direction.normalized * (magnetForce * 0.5f); // Half the force.
                        playerRigidbody.AddForce(force, ForceMode2D.Impulse);

                        Rigidbody2D objectRigidbody = collider.GetComponent<Rigidbody2D>();
                        if (objectRigidbody != null)
                        {
                            objectRigidbody.AddForce(-force, ForceMode2D.Impulse); // Apply equal and opposite force to the attracted object.
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
