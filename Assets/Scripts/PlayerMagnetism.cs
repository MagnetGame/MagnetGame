using UnityEngine;

public class PlayerMagnetism : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 2f;
    public LineRenderer lineRendererPrefab;
    private LineRenderer currentLineRenderer;
    private bool isAttracting = false;
    private bool isRepelling = false;
    private Rigidbody2D rb;
    

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
                   // DrawLine(collider.transform.position, Color.red);

                    rb = collider.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 direction = transform.position - collider.transform.position;
                        rb.AddForce(direction.normalized * magnetForce, ForceMode2D.Impulse);
                    }
                }
                else if ((isAttracting && collider.CompareTag("SouthPolarity")) || (isRepelling && collider.CompareTag("NorthPolarity")))
                {
                    Debug.Log("South Mode");
                  //  DrawLine(collider.transform.position, Color.blue);

                    rb = collider.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 direction = collider.transform.position - transform.position;
                        rb.AddForce(direction.normalized * magnetForce, ForceMode2D.Impulse);
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

    void DrawLine(Vector3 targetPosition, Color color)
    {
        if (currentLineRenderer == null)
        {
            currentLineRenderer = Instantiate(lineRendererPrefab, transform.position, Quaternion.identity);
        }

        currentLineRenderer.startColor = color;
        currentLineRenderer.endColor = color;
        currentLineRenderer.SetPosition(0, transform.position);
        currentLineRenderer.SetPosition(1, targetPosition);
    }
}
