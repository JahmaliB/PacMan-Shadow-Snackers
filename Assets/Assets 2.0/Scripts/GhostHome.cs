using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;
    private Coroutine bounceCoroutine;

    private void OnEnable()
    {
        StopAllCoroutines();
        bounceCoroutine = StartCoroutine(MoveUpDownInHome());
    }

    private void OnDisable()
    {
        StopCoroutine(bounceCoroutine);

        // Check for active self to prevent error when object is destroyed
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ExitTransition());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reverse direction every time the ghost hits a wall to create the
        // effect of the ghost bouncing around the home
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            ghost.movement.SetDirection(-ghost.movement.Direction);
        }
    }

    private IEnumerator ExitTransition()
    {
        // Turn off movement while we manually animate the position
        ghost.movement.SetDirection(Vector2.up, true);
        ghost.movement.GetComponent<Rigidbody2D>().isKinematic = true;
        ghost.movement.enabled = false;

        Vector3 position = transform.position;

        float duration = 0.5f;
        float elapsed = 0f;

        // Animate to the starting point inside the home
        while (elapsed < duration)
        {
            // Move towards the inside position
            transform.position = Vector3.Lerp(position, inside.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Animate exiting the ghost home
        while (elapsed < duration)
        {
            // Move from inside position to outside position
            transform.position = Vector3.Lerp(inside.position, outside.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pick a random direction (left or right) and re-enable movement
        ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1f : 1f, 0f), true);
        ghost.movement.GetComponent<Rigidbody2D>().isKinematic = false;
        ghost.movement.enabled = true;
    }

    private IEnumerator MoveUpDownInHome()
    {
        // Define the upward and downward directions
        Vector2 upDirection = Vector2.up;
        Vector2 downDirection = Vector2.down;

        // Define the distance to move up and down (adjust as needed)
        float moveDistance = 2f;
        float moveSpeed = 7f;

        Vector2 startPosition = ghost.transform.position;

        Vector2 targetUpPosition = startPosition + upDirection * moveDistance;
        Vector2 targetDownPosition = startPosition + downDirection * moveDistance;

        while (enabled)
        {
            // Move up to the target position
            while (Vector2.Distance(ghost.transform.position, targetUpPosition) > 0.01f)
            {
                ghost.transform.position = Vector2.MoveTowards(ghost.transform.position, targetUpPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Move down to the target position
            while (Vector2.Distance(ghost.transform.position, targetDownPosition) > 0.01f)
            {
                ghost.transform.position = Vector2.MoveTowards(ghost.transform.position, targetDownPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

}

