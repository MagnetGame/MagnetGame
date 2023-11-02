using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

public class ObjectPolarityHandler : MonoBehaviour
{
    public float polarityRange = 1;
    public float magnetForce = 2;

    private string polarityTag; // The tag for this object, set in the Unity inspector.
    private string colliderTag = "";
    private Rigidbody2D myRigidbody;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        polarityTag = gameObject.tag;

        //Boolean isInRange = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircleRadius, groundLayer);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, polarityRange);

        foreach (Collider2D collider in colliders)
        {
            colliderTag = collider.gameObject.tag;

            Vector2 direction = transform.position - collider.transform.position;

            if ((polarityTag == "NorthPolarity" && colliderTag == "NorthPolarity") || (polarityTag == "SouthPolarity" && colliderTag == "SouthPolarity")) // rejection
            {

            }
            else if ((polarityTag == "NorthPolarity" && colliderTag == "SouthPolarity") || (polarityTag == "SouthPolarity" && colliderTag == "NorthPolarity"))  //attraction 
            {

            }
            else
            {

            }
        }
    }
}