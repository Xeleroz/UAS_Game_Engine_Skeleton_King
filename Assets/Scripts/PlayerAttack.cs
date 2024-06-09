using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Check if the collided object has the "Enemy" tag
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>(); // Get the EnemyAI component from the collided object
            if (enemy != null)
            {
                Player player = GetComponent<Player>();
                player.Attacking(other.transform);
                enemy.TakeDamage(10);
            }
        }
    }
}
