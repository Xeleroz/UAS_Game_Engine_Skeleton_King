using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 direction;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        direction = (playerTransform.position - transform.position).normalized;
    }

    void Update()
    {
        // Update the direction to follow the player
        direction = (playerTransform.position - transform.position).normalized;

        // Move towards the player
        transform.position += direction * speed * Time.deltaTime;

        // Rotate towards the player
        RotateTowardsPlayer();
    }

    void RotateTowardsPlayer()
    {
        Vector3 targetDirection = playerTransform.position - transform.position;
        float step = rotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.up, targetDirection, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, newDirection);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to the player or perform any other action
            Destroy(gameObject);
        }
    }
}
