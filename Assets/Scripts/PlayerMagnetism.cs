using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnetism : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 10f;
    private bool isAttracting = false;
    private bool isRepelling = false;

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
                    Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 direction = transform.position - collider.transform.position;
                        rb.AddForce(direction.normalized * magnetForce, ForceMode2D.Impulse);
                    }
                }
                else if ((isAttracting && collider.CompareTag("SouthPolarity")) || (isRepelling && collider.CompareTag("NorthPolarity")))
                {
                    Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Debug.Log("South Mode");
                        Vector2 direction = collider.transform.position - transform.position;
                        rb.AddForce(direction.normalized * magnetForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
}
