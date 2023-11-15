using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    private playerState currentState = playerState.Neutral;
    private Rigidbody2D playerRigidbody;  // Rigidbody for the player.

    [SerializeField] private AudioClip magnetismClip;
    public Material lineMaterial;
    public float lineLifeTime = 0.5f;

    private enum playerState
    {
        NorthMode,
        SouthMode,
        Neutral
    }

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>(); // Assuming the player has the Rigidbody2D component on the same GameObject as this script.
    }

    void Update()// do not used fixed update no matter what here
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentState = playerState.NorthMode;
            SoundFXManager.Instance.PlaySoundFXClip(magnetismClip, transform, 0.6f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentState = playerState.SouthMode;
            SoundFXManager.Instance.PlaySoundFXClip(magnetismClip, transform, 0.6f);
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

                if (collider.CompareTag("NorthPolarity") || collider.CompareTag("SouthPolarity"))
                {
                    UpdateLine(collider.transform);
                }
            }
        }
        
    }

    // Instantiate or update the LineRenderer.
    private void UpdateLine(Transform target)
    {
        LineManager lineManager = target.GetComponent<LineManager>();

        if (lineManager == null)
        {
            GameObject lineObject = new GameObject("Line");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineManager = lineObject.AddComponent<LineManager>();
            lineManager.SetUpLine(new Transform[] { transform, target });

            // Set LineRenderer properties
            SetLineRendererProperties(lineRenderer, target.tag);
            Destroy(lineManager.gameObject, lineLifeTime);
        }
        else
        {
            lineManager.SetUpLine(new Transform[] { transform, target });
            Destroy(lineManager.gameObject, lineLifeTime);
        }
    }

    private void SetLineRendererProperties(LineRenderer lineRenderer, string polarityTag)
    {
        // Set width to 1/3rd of its normal size
        lineRenderer.startWidth = 0.20f;
        lineRenderer.endWidth = 0.20f;

        lineRenderer.material = lineMaterial;

        // Set color based on polarity
        Color startColor = polarityTag == "NorthPolarity" ? new Color(1, 0, 0, 0.5f) : new Color(0, 0, 1, 0.5f);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Gradient to 0 alpha

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }
}

