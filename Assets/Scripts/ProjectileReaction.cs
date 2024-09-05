using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReaction : MonoBehaviour
{
    [Tooltip("Knockback force for projectile hits")]
    [SerializeField] float knockbackForce = 100f; // Set knockback force

    [Tooltip("Maximum vertical knockback force")]
    [SerializeField] float maxVerticalKnockbackForce = 10f; // Adjust this value as needed

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Projectile"))
        {
            Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Apply knockback force
                Vector3 knockbackForceVector = knockbackDirection * knockbackForce;

                // Clamp the y-axis value to prevent excessive vertical force
                knockbackForceVector.y = Mathf.Clamp(knockbackForceVector.y, -maxVerticalKnockbackForce, maxVerticalKnockbackForce);

                rb.AddForce(knockbackForceVector, ForceMode.Impulse);
            }
        }
    }
}
