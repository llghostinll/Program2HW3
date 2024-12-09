using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Player;

    public Rigidbody2D RB;
    public float Speed = 5;

    public int MaxHealth = 5; // Maximum health
    public int CurrentHealth; // Current health

    private enum PlayerState { Normal, Stunned }
    private PlayerState currentState;

    private SpriteRenderer spriteRenderer;
    private Color normalColor;
    public Color stunnedColor = Color.blue;
    public float stunDuration = 2f; // Duration of stun effect
    public float invincibilityDuration = 1f; // Invincibility duration after being damaged

    private bool isInvincible = false; // Check for invincibility

    private void Awake()
    {
        Player = this;
        CurrentHealth = MaxHealth; // Initialize current health
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;
        currentState = PlayerState.Normal; // Start in Normal state
    }

    void Update()
    {
        if (currentState == PlayerState.Normal)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        Vector2 vel = Vector2.zero;
        if (Input.GetKey(KeyCode.D))
            vel.x = Speed;
        else if (Input.GetKey(KeyCode.A))
            vel.x = -Speed;
        if (Input.GetKey(KeyCode.W))
            vel.y = Speed;
        else if (Input.GetKey(KeyCode.S))
            vel.y = -Speed;
        RB.velocity = vel;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            if (!isInvincible) // Only take damage if not invincible
            {
                TakeDamage(1); // Take 1 damage when hitting a hazard
                if (CurrentHealth <= 0)
                {
                    SceneManager.LoadScene("You Lose"); // Load lose scene if health is 0
                }
                else
                {
                    BecomeStunned();
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0; // Prevent health from going below 0
        }
        Debug.Log($"Player took {damage} damage. Current health: {CurrentHealth}/{MaxHealth}");
        StartCoroutine(InvincibilityFrames()); // Start invincibility frames
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth; // Prevent health from exceeding max health
        }
        Debug.Log($"Player healed {amount} health. Current health: {CurrentHealth}/{MaxHealth}");
    }

    private void BecomeStunned()
    {
        currentState = PlayerState.Stunned;
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < stunDuration)
        {
            spriteRenderer.color = stunnedColor; // Flash to blue
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = normalColor; // Reset to normal
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        currentState = PlayerState.Normal; // Return to normal state after stun duration
        spriteRenderer.color = normalColor; // Ensure color is reset
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true; // Set invincibility to true
        yield return new WaitForSeconds(invincibilityDuration); // Wait for the duration
        isInvincible = false; // Set invincibility to false
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for exit to win the game
        if (other.gameObject.CompareTag("Exit"))
        {
            SceneManager.LoadScene("You Win"); // Load win scene
        }

        // Check for next level transition
        if (other.gameObject.CompareTag("NextLevel"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // Assuming the next scene is the next one in the build settings
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}