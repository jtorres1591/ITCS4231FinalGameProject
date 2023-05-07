using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    // Animator
    public Animator animator;
    // Player movement and collisions.
    public Transform playerTransform;
    public Rigidbody playerRb;
    // GameManager reference.
    private GameManager gameManager;
    // Player stats and booleans.
    private float playerRotation = 0f;
    public float playerSpeed = 30f;
    private float groundHeight = 1f;
    public float jumpSpeed = 60f;
    private float gravity = 0f;
    public float fallSpeed = 0.2f;
    public bool isJumping = false;
    public float groundPoundForce = 0.6f;
    private bool canGroundPound = true;
    public bool isGroundPounding = false;
    public bool jumpStart = false;
    public bool isWalking = false;
    // playerActive will activate once the game begins. Will be a requirement for all controls.
    public bool playerActive = false;
    // Constraints for the map edges. Player will fall if they go off the edge.
    public float westBound = -46.0f;
    public float eastBound = 46.0f;
    public float northBound = 46.0f;
    public float southBound = -46.0f;
    // lowerBound stops player from jumping while underneath the block within constraints.
    public float lowerBound = -1.0f;
    public bool fellOff = false;
    // Score Variable
    public int score;
    public TextMeshProUGUI scoreText;
    // Score given for killing an enemy.
    public int enemyScore = 10;
    // Health Variable, display and damage cooldown.
    public int playerHealth = 3;
    private bool damageCooldown = false;
    public float damageCooldownDuration = 0.3f;
    public bool outOfHealth = false;
    public TextMeshProUGUI healthText;
    // Power Up parameters.
    public bool powerUpActive = false;
    public float powerUpDuration = 4.0f;
    public GameObject powerUpIndicator;
    public Vector3 indicatorOffset = new Vector3(0, 6, 0);
    // Attack parameters.
    public float attackSpeed = 45f;
    public bool isAttacking = false;
    public float attackDuration = 0.5f;
    // Action Cooldown variables. Will be true if the cooldown is active.
    private bool attackCooldown = false;
    private bool jumpCooldown = false;
    // Cooldown Durations. attackCooldownDuration is for both attack and ground pound.
    public float attackCooldownDuration = 0.3f;
    public float jumpCooldownDuration = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize GameManager.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Set up score display.
        score = 0;
        scoreText.text = "Score: " + score;
        // Set up health display.
        healthText.text = "Health: " + playerHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Offset the PowerUp Indicator's position to the player.
        powerUpIndicator.transform.position = transform.position + indicatorOffset;

        Vector3 currentRotation = new Vector3(0, 0, 0);
    // Reference Variables for movement.
    float currentX = playerTransform.position.x;
        float currentY = playerTransform.position.y;
        float currentZ = playerTransform.position.z;


        Vector3 movementVector = Vector3.zero;

        // Set fellOff to true if player goes below lowerBound and activate game over.
        if (playerTransform.position.y <= lowerBound) { 
            fellOff = true;
            gameManager.GameOver();
        }
        // Start Directional Movement.
        // Walking Animation will happen regardless of direction.
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("isWalking", true);
        }
        else {
            animator.SetBool("isWalking", false);
        }
        // Prevent turning while dead, attacking or falling.
        if (!isAttacking && !gameManager.gameOver && playerActive)
        {
            // Up/Down Input and accounting for diagonals.
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    // Up/Left.
                    playerTransform.position = new Vector3(currentX - (playerSpeed * Time.deltaTime), currentY, currentZ + (playerSpeed * Time.deltaTime));
                    playerRotation = 315f;
                    playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    // Up/Right.
                    playerTransform.position = new Vector3(currentX + (playerSpeed * Time.deltaTime), currentY, currentZ + (playerSpeed * Time.deltaTime));
                    playerRotation = 45f;
                    playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
                }
                else
                {
                    // Up.
                    playerTransform.position = new Vector3(currentX, currentY, currentZ + (playerSpeed * Time.deltaTime));
                    playerRotation = 0f;
                    playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    // Down/Left.
                    playerTransform.position = new Vector3(currentX - (playerSpeed * Time.deltaTime), currentY, currentZ - (playerSpeed * Time.deltaTime));
                    playerRotation = 225f;
                    playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    // Down/Right.
                    playerTransform.position = new Vector3(currentX + (playerSpeed * Time.deltaTime), currentY, currentZ - (playerSpeed * Time.deltaTime));
                    playerRotation = 135f;
                    playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
                }
                else
                {
                    // Down.
                    playerTransform.position = new Vector3(currentX, currentY, currentZ - (playerSpeed * Time.deltaTime));
                    playerRotation = 180f;
                    playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
                }
            }
            // Left/Right Input without vertical input.
            // Left.
            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                playerTransform.position = new Vector3(currentX - (playerSpeed * Time.deltaTime), currentY, currentZ);
                playerRotation = 270f;
                playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
            }
            // Right.
            else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                playerTransform.position = new Vector3(currentX + (playerSpeed * Time.deltaTime), currentY, currentZ);
                playerRotation = 90f;
                playerTransform.transform.eulerAngles = new Vector3(0, playerRotation, 0);
            }

            // End Directional Movement
        }
        // Attack Input.
        if (Input.GetKey(KeyCode.C) && !isJumping && !attackCooldown && !gameManager.gameOver && playerActive) {
            DoAttack();
        }
        // Attack
        if (isAttacking) {
            
            // Case for each rotation.
            // Up.
            if (playerRotation == 0) {
                playerTransform.position = new Vector3(currentX, currentY, currentZ + (attackSpeed * Time.deltaTime));
            }
            // Up/Right.
            else if (playerRotation == 45){
                playerTransform.position = new Vector3(currentX + (attackSpeed * Time.deltaTime), currentY, currentZ + (attackSpeed * Time.deltaTime));
            }
            // Right.
            else if (playerRotation == 90)
            {
                playerTransform.position = new Vector3(currentX + (attackSpeed * Time.deltaTime), currentY, currentZ);
            }
            // Down/Right.
            else if (playerRotation == 135)
            {
                playerTransform.position = new Vector3(currentX + (attackSpeed * Time.deltaTime), currentY, currentZ - (attackSpeed * Time.deltaTime));
            }
            // Down.
            else if (playerRotation == 180)
            {
                playerTransform.position = new Vector3(currentX, currentY, currentZ - (attackSpeed * Time.deltaTime));
            }
            // Down/Left.
            else if (playerRotation == 225)
            {
                playerTransform.position = new Vector3(currentX - (attackSpeed * Time.deltaTime), currentY, currentZ - (attackSpeed * Time.deltaTime));
            }
            // Left.
            else if (playerRotation == 270)
            {
                playerTransform.position = new Vector3(currentX - (attackSpeed * Time.deltaTime), currentY, currentZ);
            }
            // Up/Left.
            else if (playerRotation == 315)
            {
                playerTransform.position = new Vector3(currentX - (attackSpeed * Time.deltaTime), currentY, currentZ + (attackSpeed * Time.deltaTime));
            }

        }
        // Jump. Accounts for input and making sure you aren't already dead, jumping, attacking, haven't instantly landed, and stand on solid ground.
        if (Input.GetKey(KeyCode.X) && !isJumping && !isAttacking && !jumpCooldown && playerTransform.position.x > westBound && playerTransform.position.x < eastBound && playerTransform.position.z < northBound && playerTransform.position.z > southBound && !gameManager.gameOver && playerActive) {
            DoJump();
            jumpCooldown = true;
        }
        // Ground Pound
        if (!canGroundPound)
        {
            canGroundPound = false;
            gravity -= groundPoundForce * Time.deltaTime;
            playerTransform.position += new Vector3(0f, gravity, 0f);
            if (PlayerIsOnGround())
            {
                canGroundPound = true;
                isGroundPounding = false;
                // Start Cooldown once player hits the ground.
                StartCoroutine(AttackCooldown());
            }
        }
        // Jump actions
        else if (isJumping)
        {
            // Gravity.
            gravity -= fallSpeed;
            playerTransform.position += new Vector3(0f, gravity, 0f);
            // Upward movement, which will be impossible if the player is below the map.
            if (!fellOff)
            {
                playerTransform.position += new Vector3(0f, jumpSpeed * Time.deltaTime, 0f);
            }
            // Check for return to ground.
            if (PlayerIsOnGround())
            {
                isJumping = false;
                StartCoroutine(JumpCooldown());
                playerTransform.position = new Vector3(playerTransform.position.x, groundHeight, playerTransform.position.z);
            }
            // Ground Pound.
            if (Input.GetKey(KeyCode.C) && canGroundPound && !gameManager.gameOver && playerActive)
            {
                DoGroundPound();
            }

        }
        // Case of falling.
        else if(PlayerIsOnGround() == false) {
            // Gravity.
            gravity -= fallSpeed;
            playerTransform.position += new Vector3(0f, gravity, 0f);
        }
        
        // END OF UPDATE METHOD
    }
    // Set Jump Parameters.
    private void DoJump() {
        gravity = 0f;
        isJumping = true;
        // Start Jump Animation.
        animator.SetBool("isJumping", true);
        StartCoroutine(JumpStart());
    }
    // Set Ground Pound Parameters
    private void DoGroundPound() {
        canGroundPound = false;
        isGroundPounding = true;
    }
    // Set Attack Parameters.
    private void DoAttack() {
        isAttacking = true;
        // Attack Animation.
        animator.SetBool("isAttacking", true);
        StartCoroutine(AttackProcess());
    }
    // Returns if the player is on the ground.
    private bool PlayerIsOnGround() {
        return playerTransform.position.y <= groundHeight && playerTransform.position.y >= -2 && playerTransform.position.x > westBound && playerTransform.position.x < eastBound && playerTransform.position.z < northBound && playerTransform.position.z > southBound;
    }
    // Activate PowerUp if collided with.
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PowerUp")) {
            // Trigger PowerUp
            StartCoroutine(StartPowerUp());
            Destroy(other.gameObject);
            // Heal the player if collided with.
        } else if (other.CompareTag("Healing"))
        {
            // Don't let the player heal beyond 3 health.
            if (playerHealth > 0 && playerHealth < 3) playerHealth++;
            // Update healthText.
            healthText.text = "Health: " + playerHealth;

            Destroy(other.gameObject);
        }
    }

    // Trigger PowerUp
    IEnumerator StartPowerUp() {
        powerUpActive = true;
        powerUpIndicator.gameObject.SetActive(true);
        yield return new WaitForSeconds(powerUpDuration);
        powerUpActive = false;
        powerUpIndicator.gameObject.SetActive(false);
    }
    // Defeat enemies that are attacked or deduct health if attacked. NOTE: This entire script worked better in EnemyAction, so it was moved there.
    /*
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            if (isAttacking || isGroundPounding)
            {
                // Destroy the enemy.
                
                
            }
            else {
                // Deal Damage
                //takeDamage();

            }
            
        }
    }
    */

    // Take Damage if damageCooldown is over.
    public void takeDamage() {
        if (!damageCooldown)
        {
            
            if(playerHealth > 0) playerHealth--;
            // Update healthText.
            healthText.text = "Health: " + playerHealth;
            if (playerHealth <= 0)
            {
                outOfHealth = true;
                // Death Animation.
                animator.SetBool("outOfHealth", true);
                gameManager.GameOver();
            }
            else
            {
                StartCoroutine(DamageCooldown());
            }
        }
    }
    // Attack Duration
    IEnumerator AttackProcess() {
        yield return new WaitForSeconds(attackDuration);
        // End attack animation
        animator.SetBool("isAttacking", false);
        isAttacking = false;
        
        StartCoroutine(AttackCooldown());
    }
    // Action Cooldowns
    // Attack & Ground Pound Cooldown
    IEnumerator AttackCooldown()
    {
        attackCooldown = true;
        yield return new WaitForSeconds(attackCooldownDuration);
        attackCooldown = false;
    }
    // Jump Cooldown
    IEnumerator JumpCooldown()
    {
        // Jump Animation End.
        animator.SetBool("isJumping", false);
        yield return new WaitForSeconds(jumpCooldownDuration);
        jumpCooldown = false;
    }
    // Make sure the player only jumps once.
    IEnumerator JumpStart() {
        jumpStart = true;
        yield return new WaitForSeconds(jumpCooldownDuration);
        jumpStart = false;
    }
    // Invincibility frames.
    IEnumerator DamageCooldown() {
        damageCooldown = true;
        yield return new WaitForSeconds(damageCooldownDuration);
        damageCooldown = false;
    }
    // Begins the game.
    public void PlayerStart() {
        playerActive = true;
    }
}
