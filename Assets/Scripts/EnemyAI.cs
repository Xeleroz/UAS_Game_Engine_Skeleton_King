using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Include this directive to use UnityEvent

public class EnemyAI : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput;
    public UnityEvent OnAttack;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float chaseDistanceThreshold = 3f;
    [SerializeField]
    private float attackDistanceThreshold = 0.8f;

    [SerializeField]
    private float attackDelay = 1f;
    private float passedTime = 1f;

    [SerializeField]
    private float movementSpeed = 3f;

    public int currentHP;
    public int maxHP = 50; // max HP for enemy

    private bool isRunningAway = false;
    
    private float wanderTimer = 0f;
    
    private bool isMovingHorizontally = true; // Start by moving horizontally

    void Start()
    {
        currentHP = maxHP;
    }

    private void Update()
{
    if (player == null)
        return;

    float distance = Vector2.Distance(player.position, transform.position);

    if (currentHP <= 10)
    {
        isRunningAway = true;
    }

    if (isRunningAway)
    {
        RunAwayFromPlayer();
    }
    else
    {
        if (distance < chaseDistanceThreshold)
        {
            if (distance <= attackDistanceThreshold)
            {
                 // Player is within attack range, invoke the attack event
                    OnMovementInput?.Invoke(Vector2.zero);
                    if (passedTime >= attackDelay)
                    {
                        OnAttack?.Invoke();
                        player.GetComponent<Player>().TakeDamage(5);
                        passedTime = 0f; // Reset the timer
                    }
            }
            else
            {
                // Chasing the player
                Vector2 direction = (player.position - transform.position).normalized;
                OnMovementInput?.Invoke(direction);
            }
        }
        else
        {
            WanderAround();
        }
    }
    passedTime += Time.deltaTime; // Increment the timer
}

    private void RunAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        OnMovementInput?.Invoke(direction * (movementSpeed / 2)); // Run away with half speed
    }

    private void WanderAround()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= 4f)
        {
            wanderTimer = 0f;
            isMovingHorizontally = !isMovingHorizontally; // Toggle between moving horizontally and vertically
        }

        Vector2 direction;
        if (isMovingHorizontally)
        {
            // Move left or right
            direction = new Vector2(Random.Range(-1f, 1f), 0f).normalized;
        }
        else
        {
            // Move up or down
            direction = new Vector2(0f, Random.Range(-1f, 1f)).normalized;
        }

        movementSpeed = 0.8f; // Reset the movement speed to default
        OnMovementInput?.Invoke(direction * movementSpeed);
    }   



    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle enemy death (e.g., play animation, disable controls, etc.)
        Debug.Log("Enemy died");
        Destroy(gameObject); // Example of destroying the enemy object
    }
}
