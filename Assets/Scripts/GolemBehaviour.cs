using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBehaviour : MonoBehaviour 
{
    public enum GolemState
    {
        Idle,
        Run
    }

    public enum GolemFacing
    {
        Up,
        Down,
        Left,
        Right
    }

    public GolemFacing facing;
    public GolemState currentState;

    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float knockbackForce = 10f; // Force applied to player when knocked back
    [SerializeField] private float knockbackDuration = 0.5f; // Duration of knockback

    [SerializeField] private float chaseDistanceThreshold = 3f;
    [SerializeField] private float attackDistanceThreshold = 0.8f;

    [SerializeField] private float attackDelay = 1f;
    private float passedTime = 1f;

    [SerializeField] private float movementSpeed = 3f;

    private GameObject player;
    private Rigidbody2D playerRigidbody;
    private bool hasLineOfSight = true;

    private float wanderTimer = 0f;
    private bool isMovingHorizontally = true; // Start by moving horizontally
    private Animator animator; // Reference to the Animator component
    public GameObject ProjectilePrefab;
    [SerializeField] public Transform shootPoint; // Reference to the shoot point object
    public Transform EyesightPrefab;
    public float shootRange = 5f;
    private Transform target;
    private Vector2 velocity;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (target == null)
        {
            FindTarget();
            return;
        }

        float distance = Vector2.Distance(player.transform.position, transform.position);

        // Check if the enemy has line of sight to the player
        if (hasLineOfSight && distance <= shootRange)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            // Set animation state based on movement direction
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    facing = GolemFacing.Right;
                else
                    facing = GolemFacing.Left;
            }
            else
            {
                if (direction.y > 0)
                    facing = GolemFacing.Up;
                else
                    facing = GolemFacing.Down;
            }

            currentState = GolemState.Run;

            // Call Move function when chasing
            Move(direction);

            // If within attack distance, instantiate projectile
            if (distance <= attackDistanceThreshold && passedTime >= attackDelay)
            {
                ShootProjectile();
                passedTime = 0f;
            }
        }
        else
        {
            // Wander around if no line of sight
            WanderAround();
            currentState = GolemState.Idle;
            hasLineOfSight = false;
        }

        animator.SetInteger("State", (int)currentState);
        animator.SetInteger("Facing", (int)facing);

        passedTime += Time.deltaTime; // Increment the timer
    }

    void ShootProjectile()
    {
        // Check if the shoot point is assigned
        if (shootPoint != null)
        {
            // Instantiate projectile
            Instantiate(ProjectilePrefab, shootPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Shoot point not assigned in GolemBehaviour!");
        }
    }

    private void FindTarget()
    {
        if (player == null) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            target = hit.transform;
            hasLineOfSight = true;
        }
        else
        {
            target = null;
            hasLineOfSight = false;
        }
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.CompareTag("Player");
            if (hasLineOfSight)
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
            }
        }
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

        Move(direction);
    }

    private void Move(Vector2 direction)
    {
        velocity = direction * movementSpeed;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Set animation parameters
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                facing = GolemFacing.Right;
            else
                facing = GolemFacing.Left;
        }
        else
        {
            if (direction.y > 0)
                facing = GolemFacing.Up;
            else
                facing = GolemFacing.Down;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                StartCoroutine(playerScript.KnockbackRoutine(knockbackDuration));
                playerScript.TakeDamage(10);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
