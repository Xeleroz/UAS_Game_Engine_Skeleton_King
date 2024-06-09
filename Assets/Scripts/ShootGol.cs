using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShootGol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform dragonRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firingPoint;


    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float bps = 1f; //bullet per second (speed)

    private Transform target;
    private float timeUntillFire;

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();
        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            timeUntillFire += Time.deltaTime; 

            if (timeUntillFire >= 1f / bps)
            {
                Shoot();
                timeUntillFire = 0f;
            }
        }
    } 

    private void Shoot()
    { 
        GameObject bulletObj = Instantiate(bulletPrefabs, firingPoint.position, Quaternion.identity); 
        Fire fireScript = bulletObj.GetComponent<Fire>();
        fireScript.SetTarget(target);

    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask); 

        if (hits.Length >0 )
        {
            target = hits[0].transform;
        }
    }  

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        dragonRotationPoint.rotation = Quaternion.RotateTowards(dragonRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime); 
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan; 
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
        
    }

}
