using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 nextDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = initialDirection;
    }

    private void Start()
    {
        direction = initialDirection;
    }

    private void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 translation = direction * speed * speedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 newDirection, bool forced = false)
    {
        if (forced || !Occupied(newDirection))
        {
            direction = newDirection;
            nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = newDirection;
        }
    }

    public bool Occupied(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0f, dir, 1f, obstacleLayer);
        return hit.collider != null;
    }

    public Vector2 Direction
    {
        get { return direction; }
    }

    public void ResetState()
    {
        direction = initialDirection;
        speedMultiplier = 1.0f;
    }
}
