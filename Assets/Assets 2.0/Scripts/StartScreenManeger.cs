using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public AudioSource gameStartSound;
    private bool hasStarted = false; 

    private void Update()
    {
        // Play the sound once if it hasn't been played yet
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if (!hasStarted && Input.anyKeyDown)
        {
            gameStartSound.Play();
            hasStarted = true;
            StartGame();
        }
    }

    private void StartGame()
    {
        // Load the main gameplay scene
        SceneManager.LoadScene("PacMan Game");
    }

    private void QuitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit(); // Quit the application in the build
    #endif
    }
}


