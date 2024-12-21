using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman1;
    public Pacman pacman2;
    public UIManager uiManager;
    public Transform pellets;
    public static GameManager Instance { get; private set; }
    public int ghostMultiplier { get; private set; }
    public int pacMan1Score { get; private set; }
    public int pacMan2Score { get; private set; }
    public int pacMan1Lives { get; private set; }
    public int pacMan2Lives { get; private set; }
    public int currentMunch = 0;
    public float damageCooldown = 1.0f;
    private bool pacman1RecentlyDamaged = false;
    private bool pacman2RecentlyDamaged = false;

    public AudioSource siren1;
    public AudioSource mmunch1;
    public AudioSource mmunch2;
    public AudioSource frightSound;
    public AudioSource fruitEatSound;

    public GameObject[] fruitPrefabs;
    public Transform[] spawnPoints;
    public float fruitSpawnInterval = 30.0f;
    [SerializeField] private float fruitDespawnTime = 10f;
    private List<GameObject> activeFruits = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        NewGame();
        StartCoroutine(FruitSpawner());
    }

    private IEnumerator FruitSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(fruitSpawnInterval);

            // Spawn a random fruit
            if (activeFruits.Count == 0 && fruitPrefabs.Length > 0 && spawnPoints.Length > 0)
            {
                int randomFruitIndex = Random.Range(0, fruitPrefabs.Length);
                int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

                GameObject fruit = Instantiate(
                    fruitPrefabs[randomFruitIndex],
                    spawnPoints[randomSpawnIndex].position,
                    Quaternion.identity
                );

                activeFruits.Add(fruit);
                StartCoroutine(DespawnFruitAfterTime(fruit));
            }
        }
    }

    private IEnumerator DespawnFruitAfterTime(GameObject fruit)
    {
        yield return new WaitForSeconds(fruitDespawnTime);

        // Ensure the fruit hasn't been collected before despawning
        if (activeFruits.Contains(fruit))
        {
            activeFruits.Remove(fruit);
            Destroy(fruit);
        }
    }

    private void Update()
    {
        //if both pacmen are dead then load intermission screen
        if (pacMan1Lives <= 0 && pacMan2Lives <= 0)
        {
            SceneManager.LoadScene("Intermission");
            return;
        }
        
        //decides what audio to play
        if (!mmunch1.isPlaying && !mmunch2.isPlaying && !siren1.isPlaying)
        {
            siren1.Play();
        }
        else if (mmunch1.isPlaying || mmunch2.isPlaying)
        {
            siren1.Stop();
        }
    }

    private void NewGame()
    {
        SetScorePacman(0, 1);
        SetScorePacman(0, 2);
        SetLivesPacman(3, 1);
        SetLivesPacman(3, 2);

        NewRound();

        uiManager.UpdateUI(pacMan1Score, pacMan1Lives, pacMan2Score, pacMan2Lives);
    }

    private void NewRound()
    {
        foreach (Transform pellet in pellets)
        {
            if (!pellet.gameObject.activeSelf)
            {
                pellet.gameObject.SetActive(true);
            }
        }
        ResetGameState();
    }

    private void EndRound()
    {
        pacman1.gameObject.SetActive(false);
        pacman2.gameObject.SetActive(false);
        NewRound();
    }

    private void ResetGameState()
    {
        ResetGhostMultiplier();
        foreach (Ghost ghost in ghosts)
        {
            ghost.ResetState();
        }
        if (pacMan1Lives > 0)
        {
            pacman1.ResetPacmanState();
        }

        if (pacMan2Lives > 0)
        {
            pacman2.ResetPacmanState();
        }
    }

    public void SetScorePacman(int score, int player)
    {
        if (player == 1)
        {
            pacMan1Score = score;
        }
        else if (player == 2)
        {
            pacMan2Score = score;
        }
        uiManager.UpdateUI(pacMan1Score, pacMan1Lives, pacMan2Score, pacMan2Lives); // Update UI
    }

    public void SetLivesPacman(int lives, int player)
    {
        if (player == 1)
        {
            pacMan1Lives = lives;
        }
        else if (player == 2)
        {
            pacMan2Lives = lives;
        }
        uiManager.UpdateUI(pacMan1Score, pacMan1Lives, pacMan2Score, pacMan2Lives); // Update UI
    }

    public void GhostEaten(Pacman player, Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;

        if (player == pacman1)
        {
            SetScorePacman(pacMan1Score + points, 1);
        }
        else if (player == pacman2)
        {
            SetScorePacman(pacMan2Score + points, 2);
        }

        ghostMultiplier++;
    }

    public void PacmanEaten(Pacman player)
    {
        if (player == pacman1 && !pacman1RecentlyDamaged)
        {
            pacman1RecentlyDamaged = true;
            StartCoroutine(DamageCooldown(1));

            if (pacMan1Lives > 0)
            {
                SetLivesPacman(pacMan1Lives - 1, 1);

                if (pacMan1Lives > 0)
                {
                    StartCoroutine(DelayedReset(pacman1));
                }
                else
                {
                    pacman1.gameObject.SetActive(false);
                }
            }
        }
        else if (player == pacman2 && !pacman2RecentlyDamaged)
        {
            pacman2RecentlyDamaged = true;
            StartCoroutine(DamageCooldown(2));

            if (pacMan2Lives > 0)
            {
                SetLivesPacman(pacMan2Lives - 1, 2);

                if (pacMan2Lives > 0)
                {
                    StartCoroutine(DelayedReset(pacman2));
                }
                else
                {
                    pacman2.gameObject.SetActive(false);
                }
            }
        }
    }
    
    //waits 3 seconds before respawnning pacman
    private IEnumerator DelayedReset(Pacman pacman)
    {
        pacman.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        pacman.ResetPacmanState();
    }

    public void PelletEaten(Pacman player, Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        // Play the munch sound
        PlayMunchSound();

        if (player == pacman1)
        {
            SetScorePacman(pacMan1Score + pellet.points, 1);
        }
        else if (player == pacman2)
        {
            SetScorePacman(pacMan2Score + pellet.points, 2);
        }

        if (!HasRemainingPellets())
        {
            EndRound();
        }
    }

    public void PowerPelletEaten(PowerPellet pellet, Pacman player)
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.frightened.Enable(pellet.duration);
        }

        PelletEaten(player, pellet);
        frightSound.loop = true;
        PlayFrightSound();

        CancelInvoke();
        Invoke(nameof(StopFrighteningSound), pellet.duration);
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private void PlayFrightSound()
    {
        // Ensure only the frightening sound plays during this time
        if (!frightSound.isPlaying)
        {
            siren1.Stop(); // Optional: stop siren sound during frightened state
            frightSound.Play();
        }
    }
    
    public void PlayFruitEatSound () 
    {
        if (!fruitEatSound.isPlaying) 
        {
            siren1.Stop();
            fruitEatSound.Play();
        }
    
    }

    private void StopFrighteningSound()
    {
        frightSound.loop = false;
        frightSound.Stop();
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
   
    public int GetLives(Pacman pacman)
    {
        if (pacman == pacman1)
        {
            return pacMan1Lives; // Assuming you have a variable tracking Player 1's lives
        }
        else if (pacman == pacman2)
        {
            return pacMan2Lives; // Assuming you have a variable tracking Player 2's lives
        }

        return 0; // Default case if no match
    }

    //counters pacman losing multiple lives if ghost are stacked up
    private IEnumerator DamageCooldown(int player)
    {
        yield return new WaitForSeconds(damageCooldown);

        if (player == 1)
        {
            pacman1RecentlyDamaged = false; // Reset Pacman 1's lock
        }
        else if (player == 2)
        {
            pacman2RecentlyDamaged = false; // Reset Pacman 2's lock
        }
    }

    private void PlayMunchSound()
    {
        // Check which sound was last played and play the other one
        if (!mmunch1.isPlaying && currentMunch == 0) // If mmunch1 isn't playing, play it
        {
            mmunch1.Play();
            currentMunch = 1; // Update state to indicate mmunch1 was played
        }
        else if (!mmunch2.isPlaying && currentMunch == 1) // If mmunch2 isn't playing, play it
        {
            mmunch2.Play();
            currentMunch = 0; // Update state to indicate mmunch2 was played
        }
    }
    
    public void RemoveActiveFruit(GameObject fruit)
    {
        if (activeFruits.Contains(fruit))
        {
            activeFruits.Remove(fruit);
        }
    }

}


