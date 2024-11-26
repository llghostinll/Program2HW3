using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    enum EnemyState
    {
        IDLE,
        CHASE
    }

    private EnemyState currentState = EnemyState.IDLE;

    public Transform player; // Reference to the player
    public float chaseRange = 10f; // Range for chasing
    public float speed = 2f; // Enemy speed
    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.IDLE:
                // Check if player is within chase range
                if (Vector3.Distance(transform.position, player.position) <= chaseRange)
                {
                    currentState = EnemyState.CHASE;
                }
                break;

            case EnemyState.CHASE:
                // Move towards the player
                MoveTowards(player.position);

                // If player is out of chase range, go back to idle
                if (Vector3.Distance(transform.position, player.position) > chaseRange)
                {
                    currentState = EnemyState.IDLE;
                }
                break;
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.velocity = direction * speed; 
    }
}
