using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the enemy moves

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Ensure to subscribe to the movement input event if using UnityEvent
        var enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.OnMovementInput.AddListener(SetMovementInput);
            enemyAI.OnAttack.AddListener(Attack);
        }
    }

    void FixedUpdate()
    {
        Move();
        UpdateAnimation();
    }

    public void SetMovementInput(Vector2 direction)
    {
        movementInput = direction;
    }

    public void Attack()
    {
        isAttacking = true;
        // You can add more logic here, like triggering an attack animation, sound effects, etc.
    }

    private void Move()
    {
        if (movementInput != Vector2.zero)
        {
            Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    private void UpdateAnimation()
    {
        if (movementInput != Vector2.zero)
        {
            // Set animation parameter to trigger walk or run animation
            animator.SetBool("Run", true);
            // Adjust animation speed based on movement speed
            animator.SetFloat("MoveSpeed", movementInput.magnitude * moveSpeed);
        }
        else
        {
            // Set animation parameter to trigger idle animation
            animator.SetBool("Run", false);
        }

        // Set animation parameter to trigger attack animation
        animator.SetBool("Attack", isAttacking);
        isAttacking = false; // Reset the attack state after triggering the animation
    }
}
