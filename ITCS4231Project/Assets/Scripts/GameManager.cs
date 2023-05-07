using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // GameManager will manage enemy and powerup spawns as well as the menu and difficulty.
    // Parameters to initialize.
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;
    public GameObject healingPrefab;
    private PlayerControl playerScript;

    // TODO: Title Screen and Difficulty Level int.
    public GameObject titleScreen;
    public int difficultyLevel = 1;
    // UI
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    // Wave number, size, text and delay.
    public int waveNumber;
    public int waveSize = 1;
    public float waveDelay;
    public TextMeshProUGUI waveText;
    // Game Over.
    public TextMeshProUGUI tipText;
    public bool gameOver = false;
    public GameObject gameOverScreen;
    // Bounds for spawn positions.
    public float westBound = -43.0f;
    public float eastBound = 43.0f;
    public float northBound = 43.0f;
    public float southBound = -43.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerControl>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Countdown for wave spawn.
    IEnumerator WaveCountdown() {
        // Delay of wave spawn.
        yield return new WaitForSeconds(waveDelay);
        // If the game is over, end the loop of WaveCountdowns.
        if(!gameOver) { 
        // Spawn new wave.
        SpawnEnemyWave(waveSize);
        // Spawn a Power Up every 8th wave.
        if(waveNumber % 8 == 0)SpawnPowerUp();
        // Spawn a healing item every 12th wave.
        if (waveNumber % 12 == 0) SpawnHealing();
        // Start Countdown for next wave.
        StartCoroutine(WaveCountdown());
        }
    }



    // Spawn a wave of enemies.
    public void SpawnEnemyWave(int enemiesToSpawn) {
        // Update Wave number and display.
        waveNumber++;
        waveText.text = "Wave: " + waveNumber;

        for (int i = 0; i < enemiesToSpawn; i++) {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }
    // Spawn a Power Up.
    public void SpawnPowerUp() {
        Instantiate(powerUpPrefab, GenerateSpawnPosition(), powerUpPrefab.transform.rotation);
    }
    // Spawn a Power Up.
    public void SpawnHealing()
    {
        Instantiate(healingPrefab, GenerateSpawnPosition(), healingPrefab.transform.rotation);
    }
    // Randomize enemy spawn position.
    private Vector3 GenerateSpawnPosition() {
        float spawnPosX = Random.Range(westBound, eastBound);
        float spawnPosZ = Random.Range(southBound, northBound);
        Vector3 randomPos = new Vector3(spawnPosX, 1, spawnPosZ);
        return randomPos;
    }
    // Start the Game.
    public void StartGame(int difficulty) {
        // Put all code after here in a GameStart function.
        // Initialize UI
        scoreText.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
        waveText.gameObject.SetActive(true);
        // Wave Size will be equivalent to the difficulty.
        difficultyLevel = difficulty;
        waveSize = difficultyLevel;
        // Initialize wave text and wave number.
        titleScreen.gameObject.SetActive(false);
        waveNumber = 0;
        waveText.text = "Wave: " + waveNumber;
        StartCoroutine(WaveCountdown());
        playerScript.PlayerStart();
    }
    // Game Over.
    public void GameOver() {
        // Create Tip Text depending on how the player died.
        if (playerScript.outOfHealth) tipText.text = "Tip: While ground pounds give double points, they are hard to land correctly, so be careful.";
        if(playerScript.fellOff) tipText.text = "Tip: You cannot change directions while attacking, so be sure you have space to attack.";
    // Sets game over to true and displays the text.
        gameOver = true;
        gameOverScreen.gameObject.SetActive(true);
    }
    // Restart Game when Restart button is clicked.
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
