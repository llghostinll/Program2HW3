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

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0; // Prevent health from going below 0
        }
        Debug.Log($"Player took {damage} damage. Current health: {CurrentHealth}/{MaxHealth}");
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If I walk into the exit. . .
        if (other.gameObject.CompareTag("Exit"))
        {
            //Win the game!
            SceneManager.LoadScene("You Win");
        }
    }
}