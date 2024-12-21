using UnityEngine;
using UnityEngine.SceneManagement;

public class IntermissionScript : MonoBehaviour
{
    public AudioSource intermissionSound;

    private bool hasPlayedSound = false;

    private void Update()
    {
        // Play the intermission sound only once
        if (!hasPlayedSound && intermissionSound != null)
        {
            intermissionSound.Play();
            hasPlayedSound = true;
        }

        // Check if Escape is pressed to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
        #endif
        }
        // If any other key is pressed, restart the game
        else if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("PacMan Game");
        }
    }
}


