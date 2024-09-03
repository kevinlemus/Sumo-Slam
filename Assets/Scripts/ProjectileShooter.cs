using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Tooltip("Reference to the Particle System for the projectile")]
    [SerializeField] ParticleSystem projectileParticleSystem;

    [Tooltip("Cooldown time between shots in seconds")]
    [SerializeField] float shotCooldown = 1f;

    [Tooltip("Speed of the projectile")]
    [SerializeField] float projectileSpeed = 20f;

    [Tooltip("Range for aim assist")]
    [SerializeField] float aimAssistRange = 5f;

    private bool canShoot = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot) // Left mouse button click
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectileParticleSystem != null)
        {
            // Calculate the input direction
            float xValue = Input.GetAxis("Horizontal");
            float zValue = Input.GetAxis("Vertical");
            Vector3 inputDirection = new Vector3(xValue, 0, zValue).normalized;

            // Find the nearest target within the aim assist range
            GameObject nearestTarget = FindNearestTarget();
            if (nearestTarget != null)
            {
                Vector3 targetDirection = (nearestTarget.transform.position - transform.position).normalized;
                inputDirection = Vector3.Lerp(inputDirection, targetDirection, 0.5f).normalized;
            }

            if (inputDirection != Vector3.zero)
            {
                // Set the direction and speed of the projectile
                var main = projectileParticleSystem.main;
                main.startSpeed = projectileSpeed;

                // Create a new particle system shape module and set the direction
                var shape = projectileParticleSystem.shape;
                shape.rotation = new Vector3(0, Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg, 0);

                // Set the reference to the player who fired the projectile
                var collision = projectileParticleSystem.collision;
                collision.sendCollisionMessages = true;

                // Set the shooter reference in the projectile
                var projectileBehavior = projectileParticleSystem.GetComponent<ProjectileBehavior>();
                if (projectileBehavior != null)
                {
                    projectileBehavior.SetShooter(gameObject);
                }

                projectileParticleSystem.Play();
                StartCoroutine(Cooldown());
            }
        }
    }

    GameObject FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestTarget = null;
        float minDistance = aimAssistRange;

        foreach (GameObject target in targets)
        {
            if (target != gameObject) // Exclude self
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTarget = target;
                }
            }
        }

        return nearestTarget;
    }

    IEnumerator Cooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
    }
}
