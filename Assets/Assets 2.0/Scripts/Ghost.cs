using Unity.VisualScripting;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }
    public GhostBehavior initialBehavior;
    public Transform target; 
    public Transform player1;
    public Transform player2; 
    public bool isPlayer1Alive = true;
    public bool isPlayer2Alive = true; 
    public int points = 200;

    private Vector3 homePosition;
    private bool isEaten;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
        homePosition = transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        isEaten = false;
        this.gameObject.SetActive(true);
        transform.position = homePosition;
        this.movement.ResetState();
        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();

        if (this.home != this.initialBehavior)
        {
            this.home.Disable();
        }
        if (this.initialBehavior != null)
        {
            this.initialBehavior.Enable();
        }

        // Sync alive status with Pacman lives
        isPlayer1Alive = GameManager.Instance.GetLives(GameManager.Instance.pacman1) > 0 && player1.gameObject.activeSelf;
        isPlayer2Alive = GameManager.Instance.GetLives(GameManager.Instance.pacman2) > 0 && player2.gameObject.activeSelf;

        // Reset target
        target = null;
    }

    private void Update()
    {
        // Continuously update the target
        UpdateTarget();

        // Always move toward the current target if it exists
        if (target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void UpdateTarget()
    {
        // Ensure both Pacman instances are valid
        bool player1Valid = player1 != null && GameManager.Instance.GetLives(GameManager.Instance.pacman1) > 0 && player1.gameObject.activeSelf;
        bool player2Valid = player2 != null && GameManager.Instance.GetLives(GameManager.Instance.pacman2) > 0 && player2.gameObject.activeSelf;

        isPlayer1Alive = player1Valid;
        isPlayer2Alive = player2Valid;

        if (home.enabled)
        {
            // If ghost is in the home, reset the target
            target = null;
            return;
        }

        // Determine target based on valid players
        if (player1Valid && player2Valid)
        {
            float distanceToPlayer1 = Vector2.Distance(transform.position, player1.position);
            float distanceToPlayer2 = Vector2.Distance(transform.position, player2.position);

            // Target the closer alive player
            target = distanceToPlayer1 < distanceToPlayer2 ? player1 : player2;
        }
        else if (player1Valid)
        {
            target = player1;
        }
        else if (player2Valid)
        {
            target = player2;
        }
        else
        {
            // No valid targets
            target = null;
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            movement.SetDirection(direction); // Update the movement component to chase the target
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PacMan"))
        {
            Pacman pacman = collision.gameObject.GetComponent<Pacman>();

            if (this.frightened.enabled)
            {
                GameManager.Instance.GhostEaten(pacman, this);
            }
            else
            {
                GameManager.Instance.PacmanEaten(pacman);
                HandlePacmanEaten(pacman); // Handle target switching
            }
        }
    }

    private void HandleGhostEaten(Pacman pacman)
    {
        if (isEaten) return; // Prevent multiple triggers

        isEaten = true;

        // Notify GameManager to award points and handle scoring
        GameManager.Instance.GhostEaten(pacman, this);

        // Reset target and enable home behavior
        target = null;
        home.Enable();

        // Move ghost to home position
        transform.position = homePosition;

        // Stop any ongoing behavior
        frightened.Disable();
        chase.Disable();
        scatter.Disable();

        // Reactivate the ghost to start bouncing in the home
        this.gameObject.SetActive(true);
    }

    private void HandlePacmanEaten(Pacman pacman)
    {
        if (pacman == null) return;

        // Check if the eaten Pacman has remaining lives
        if (pacman == GameManager.Instance.pacman1)
        {
            if (GameManager.Instance.GetLives(pacman) <= 0)
            {
                isPlayer1Alive = false;
            }
        }
        else if (pacman == GameManager.Instance.pacman2)
        {
            if (GameManager.Instance.GetLives(pacman) <= 0)
            {
                isPlayer2Alive = false;
            }
        }

        // Re-evaluate the target based on the alive status
        if (isPlayer1Alive || isPlayer2Alive)
        {
            UpdateTarget();
        }
    }
}