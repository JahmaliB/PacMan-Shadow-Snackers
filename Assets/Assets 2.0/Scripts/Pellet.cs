using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pellet : MonoBehaviour
{
    public int points = 10;

    protected virtual void Eat(Pacman player)
    {
        GameManager.Instance.PelletEaten(player, this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object belongs to either Pacman1 or Pacman2 using layers
        if (other.gameObject.layer == LayerMask.NameToLayer("PacMan") ||
            other.gameObject.layer == LayerMask.NameToLayer("PacMan 2"))
        {
            // Get the Pacman component from the collider
            Pacman player = other.GetComponent<Pacman>();

            // Ensure it's a valid Pacman instance before calling Eat
            if (player != null)
            {
                Eat(player);
            }
        }
    }
}


