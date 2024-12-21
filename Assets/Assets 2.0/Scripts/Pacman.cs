using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    public Movement movement { get; private set; }
    
    public bool isPlayerOne = true;

    private Vector3 startingPosition;

    // Lives and respawn position
    public int lives = 3;
    private GameManager gameManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        this.movement = GetComponent<Movement>();
        startingPosition = transform.position;
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {

        if (isPlayerOne)
        {
            HandlePlayerOneInput();
        }
        else
        {
            HandlePlayerTwoInput();
        }

        // Calculate the rotation based on movement direction
        float angle = Mathf.Atan2(this.movement.Direction.y, this.movement.Direction.x);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    // Handle input for Player 1 (WASD)
    private void HandlePlayerOneInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            this.movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            this.movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            this.movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            this.movement.SetDirection(Vector2.right);
        }
    }

    // Handle input for Player 2 (Arrow keys)
    private void HandlePlayerTwoInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.movement.SetDirection(Vector2.right);
        }
    }

    // Reset the Pacman state
    public void ResetPacmanState()
    {
        // Reset movement and other properties
        this.movement.ResetState();

        // Reset position to the stored starting position
        transform.position = startingPosition;

        // Make Pacman active again
        this.gameObject.SetActive(true);
    }
}

