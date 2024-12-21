using UnityEngine;

public class Fruit : MonoBehaviour
{
    // Enum to define different fruit types
    public enum FruitType
    {
        Cherry,
        Orange,
        Strawberry,
        Apple,
        Melon,
        Bell
    };

    // Public variable to set the fruit type in the inspector
    public FruitType fruitType;

    // Points corresponding to each fruit type
    public int points;

    private void Start()
    {
        // Assign points based on the fruit type
        switch (fruitType)
        {
            case FruitType.Cherry:
                points = 100;
                break;
            case FruitType.Strawberry:
                points = 200;
                break;
            case FruitType.Orange:
                points = 300;
                break;
            case FruitType.Apple:
                points = 400;
                break;
            case FruitType.Melon:
                points = 500;
                break;
            case FruitType.Bell:
                points = 1000;
                break;
            default:
                points = 0;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider's GameObject is one of the Pacmen by name
        if (other.name == "PacMan")
        {
            GameManager.Instance.SetScorePacman(GameManager.Instance.pacMan1Score + points, 1);
        }
        else if (other.name == "PacMan 2")
        {
            GameManager.Instance.SetScorePacman(GameManager.Instance.pacMan2Score + points, 2);
        }

        GameManager.Instance.PlayFruitEatSound();
        Destroy(gameObject);
    }

}



