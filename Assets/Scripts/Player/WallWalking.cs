using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWalking : MonoBehaviour
{
    public float forceMagnitude = 10f;

    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyConstantForce(Vector2 direction)
    {
        rb.AddForce(direction * forceMagnitude);


        // Calculate the angle between the direction and the force direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rb.freezeRotation = false; 
        // Apply rotation to the player based on the force direction
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.freezeRotation = true;
    }
}
