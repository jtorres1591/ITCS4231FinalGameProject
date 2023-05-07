using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : MonoBehaviour
{
    // Parameters
    public float speed;
    private Rigidbody enemyRb;
    private GameObject player;
    private PlayerControl playerScript;
    private GameManager gameManagerScript;
    public float lowerBound = -1.0f;
    public GameObject deathParticle;
    private bool outOfBounds = false;
    private bool doublePoints = false;
    // Parameters to move x and z positions without moving y in any way other than gravity.
    private float xPos;
    private float zPos;
    // Constraints for the map edges. Enemies will fall if they go off the edge.
    public float westBound = -46.0f;
    public float eastBound = 46.0f;
    public float northBound = 46.0f;
    public float southBound = -46.0f;
    // Start is called before the first frame update
    void Start()
    {
        // Set Rigidbody and player to follow.
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        // Set up gameManagerScript and player Script.
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerControl>();

    }

    // Update is called once per frame
    void Update()
    {
        // Determine if an enemy is out of bounds.
        if (transform.position.x < westBound || transform.position.x > eastBound || transform.position.z > northBound || transform.position.z < southBound) outOfBounds = true;
        // Enemy movement. Will not happen if the game is over or out of bounds.
        if (!gameManagerScript.gameOver && !outOfBounds)
        {
            // Set xPos and zPos.
            xPos = player.transform.position.x - transform.position.x;
            zPos = player.transform.position.z - transform.position.z;
            // Apply x and z movement while keeping y neutral.
            Vector3 setDirection = new Vector3(xPos, 1, zPos);
            Vector3 lookDirection = (setDirection).normalized;

            enemyRb.AddForce(lookDirection * speed);
        }

        // Destroy the enemy if it falls out of bounds.
        if (transform.position.y < lowerBound) DestroyEnemy();
    }


    // Enemy side of player collision.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerScript.isAttacking || playerScript.isGroundPounding || playerScript.powerUpActive)
            {
                // Activate double points if a ground pound or powerup defeats the enemy.
                if (playerScript.isGroundPounding || playerScript.powerUpActive) doublePoints = true;
                // Destroy the enemy.
                DestroyEnemy();

            }
            else {
                playerScript.takeDamage();
            }
        }
    }

    // Function to destroy the enemy object.
    public void DestroyEnemy() {
        // Add an explosion effect.
        Instantiate(deathParticle, transform.position, transform.rotation);

        // Update Score to correspond with game difficulty.
        for (int i = 0; i < gameManagerScript.difficultyLevel; i++)
        {
            playerScript.score += playerScript.enemyScore;
        }
        // Do it again if double points is true.
        if (doublePoints) {
            for (int i = 0; i < gameManagerScript.difficultyLevel; i++)
            {
                playerScript.score += playerScript.enemyScore;
            }
        }
        playerScript.scoreText.text = "Score: " + playerScript.score;
        // Destroy the enemy.
        Destroy(gameObject);
    }

}
