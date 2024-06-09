using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesightCollider : MonoBehaviour
{
    public GolemBehaviour golemBehaviour; // Reference to the GolemBehaviour script
    public GameObject ProjectilePrefab; // Reference to the projectile prefab to shoot

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Shoot the projectile if the player collides with the eyesight
            if (ProjectilePrefab != null && golemBehaviour != null && golemBehaviour.shootPoint != null)
            {
                // Instantiate projectile at the shoot point position and rotation
                Instantiate(ProjectilePrefab, golemBehaviour.shootPoint.position, Quaternion.identity);
            }
        }
    }
}
