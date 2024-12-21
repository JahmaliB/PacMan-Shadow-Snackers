using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text player1ScoreText;
    public Text player1LivesText;
    public Text player2ScoreText; 
    public Text player2LivesText; 

    private void Start()
    {
        UpdateUI(0, 3, 0, 3);
    }

    public void UpdateUI(int player1Score, int player1Lives, int player2Score, int player2Lives)
    {
        player1ScoreText.text = "Player 1 Score: " + player1Score;
        player1LivesText.text = "Player 1 Lives: " + player1Lives;
        player2ScoreText.text = "Player 2 Score: " + player2Score;
        player2LivesText.text = "Player 2 Lives: " + player2Lives;
    }
}

