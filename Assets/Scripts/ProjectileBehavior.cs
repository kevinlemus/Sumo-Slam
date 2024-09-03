using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Tooltip("Knockback force applied to the opponent")]
    [SerializeField] float knockbackForce = 5f;

    private GameObject shooter;

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other != shooter && other.CompareTag("Player"))
        {
            // Apply knockback to the opponent
            Rigidbody otherRb = other.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                otherRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }

            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}
