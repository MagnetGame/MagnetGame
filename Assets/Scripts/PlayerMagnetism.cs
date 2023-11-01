using UnityEngine;

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
    } //TODO swap this over to states

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>(); // Assuming the player has the Rigidbody2D component on the same GameObject as this script.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isAttracting = true;
            isRepelling = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isAttracting = false;
            isRepelling = true;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            isAttracting = false;
            isRepelling = false;
        }

        if (isAttracting || isRepelling)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            foreach (Collider2D collider in colliders)
            {
                if ((isAttracting && collider.CompareTag("NorthPolarity")) || (isRepelling && collider.CompareTag("SouthPolarity")))
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
                else if ((isAttracting && collider.CompareTag("SouthPolarity")) || (isRepelling && collider.CompareTag("NorthPolarity")))
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
        else
        {
            if (currentLineRenderer != null)
            {
                Destroy(currentLineRenderer.gameObject);
            }
        }
    }
}
