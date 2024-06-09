using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        idle,
        walk,
        run,
        attack
    }

    public enum PlayerFacing
    {
        up,
        down,
        left,
        right,
    }

    public PlayerFacing facing;

    public PlayerState currentState;

    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public Vector2 velocity;

    public Animator animator;

    public float radius;

    public Transform circleOrigin;

    public GameObject attackHitbox; // Reference to the attack hitbox object
    public Vector3 attackOffset = new Vector3(3f, 3f, 0f); // Offset for positioning the hitbox

    public int currentHP;
    public int maxHP = 100;

    private Stat health;
    public float rotationSpeed = 100f; // Adjust the speed of rotation

    private bool isRotatingHitbox = false;

    public bool isKnockedBack = false;
    public Vector3 position; // Public position field
    private UIManager uiManager;

    void Start()
    {
        currentHP = maxHP;
        GameObject healthObject = GameObject.FindWithTag("Health");
        if (healthObject != null)
        {
            health = healthObject.GetComponent<Stat>();
            health.MyMaxValue = maxHP; // Set maxHP directly
            health.MyCurrentValue = maxHP; // Set current HP to max initially
        }
        else
        {
            Debug.LogError("Health object not found! Make sure it is tagged 'Health'.");
        }

        GameObject uiManagerObject = GameObject.FindWithTag("UI");
        if (uiManagerObject != null)
        {
            uiManager = uiManagerObject.GetComponent<UIManager>();
        }
        else
        {
            Debug.LogError("UIManager object not found! Make sure it is tagged 'UIManager'.");
        }

        moveSpeed = 5f;
        currentState = PlayerState.idle;
        velocity = Vector2.down;
        currentHP = maxHP;
        position = transform.position;
    }

    void Update()
    {
        if (currentState == PlayerState.walk || currentState == PlayerState.attack || currentState == PlayerState.idle)
        {
            UpdateAnimation();
        }

        if (currentState != PlayerState.attack)
        {
            velocity = Vector2.zero;
            velocity.x = Input.GetAxisRaw("Horizontal");
            velocity.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Return) && currentState != PlayerState.attack)
            {
                StartCoroutine(Attack());
            }
            else if (velocity != Vector2.zero)
            {
                currentState = PlayerState.walk;
            }
            else
            {
                currentState = PlayerState.idle;
            }

            UpdateFacing();
        }

        // Position the attack hitbox
        if (currentState == PlayerState.attack)
        {
            Vector3 position = transform.position + attackOffset;
            attackHitbox.transform.position = position;
        }
        if (currentState == PlayerState.attack)
        {
            Vector3 position = transform.position + attackOffset;
            attackHitbox.transform.position = position;

            if (!isRotatingHitbox)
            {
                StartCoroutine(RotateHitbox()); // Rotate the hitbox if not already rotating
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("MainMenu");
        }
    }

    void UpdateAnimation()
    {
        if (currentState == PlayerState.attack)
        {
            animator.SetBool("Attack", true);
        }
        else if (velocity != Vector2.zero)
        {
            animator.SetBool("Running", true);
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
            animator.SetBool("Attack", false);
        }
        else
        {
            animator.SetBool("Running", false);
            animator.SetBool("Attack", false);
        }
    }

    IEnumerator RotateHitbox()
    {
        isRotatingHitbox = true;
        float rotationDuration = 2f; // Duration for which the hitbox rotates (adjust as needed)
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            // Rotate the hitbox around the enemy
            attackHitbox.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isRotatingHitbox = false;
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (velocity != Vector2.zero)
        {
            rb.MovePosition(rb.position + velocity * moveSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator Attack()
    {
        currentState = PlayerState.attack;
        animator.SetTrigger("Attack");

        // Assuming the attack animation is 0.5 seconds long
        yield return new WaitForSeconds(0.5f);

        // Check if any movement keys are pressed after the attack
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            currentState = PlayerState.walk;
        }
        else
        {
            currentState = PlayerState.idle;
        }
    }

    public void Attacking(Transform target)
    {
        if (currentState == PlayerState.attack && target != null)
        {
            EnemyAI enemy = target.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
        }
    }

    void UpdateFacing()
    {
        if (velocity.x > 0)
        {
            facing = PlayerFacing.right;
        }
        else if (velocity.x < 0)
        {
            facing = PlayerFacing.left;
        }
        else if (velocity.y > 0)
        {
            facing = PlayerFacing.up;
        }
        else if (velocity.y < 0)
        {
            facing = PlayerFacing.down;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = circleOrigin == null ? transform.position : circleOrigin.position;

        // Adjust the position based on the facing direction
        switch (facing)
        {
            case PlayerFacing.up:
                position += Vector3.up;
                break;
            case PlayerFacing.down:
                position += Vector3.down;
                break;
            case PlayerFacing.left:
                position += Vector3.left;
                break;
            case PlayerFacing.right:
                position += Vector3.right;
                break;
        }

        Gizmos.DrawWireSphere(position, radius);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
        health.MyCurrentValue = currentHP;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        health.MyCurrentValue = currentHP;
    }

    void Die()
    {
        // Handle player death (e.g., play animation, disable controls, etc.)
        Debug.Log("Player died");
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
        else
        {
            Debug.LogError("UIManager not assigned!");
        }
        Destroy(gameObject);
        Time.timeScale = 0;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public IEnumerator KnockbackRoutine(float duration)
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }
}
