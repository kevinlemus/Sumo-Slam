using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] ParticleSystem projectileParticleSystem;
    [SerializeField] float projectileSpeed = 20f;
    [SerializeField] float cooldownTime = 0.5f; // Cooldown duration
    [SerializeField] float aimAssistRange = 200f; // Range within which aim assist is applied
    [SerializeField] float aimAssistIntensity = 0.5f; // Intensity of aim assist (0 to 1)

    private bool canShoot = true;

    void Update()
    {
        if (gameObject.tag != "Dummy" && canShoot && Input.GetButtonDown("Fire1"))
        {
            ShootProjectile();
        }
    }

    public void ShootProjectile()
    {
        Vector3 shootDirection = GetAimAssistDirection();
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = shootDirection * projectileSpeed;
        projectileParticleSystem.Emit(emitParams, 1);
        StartCoroutine(ShootCooldown());
    }

    Vector3 GetAimAssistDirection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aimAssistRange);
        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Player") || hitCollider.gameObject.CompareTag("Dummy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = hitCollider.transform;
                }
            }
        }

        if (nearestTarget != null)
        {
            Vector3 directionToTarget = (nearestTarget.position - transform.position).normalized;
            Vector3 adjustedDirection = Vector3.Lerp(transform.forward, directionToTarget, aimAssistIntensity);
            return adjustedDirection.normalized;
        }
        else
        {
            return transform.forward;
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }
}
