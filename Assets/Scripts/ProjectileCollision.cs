using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [Tooltip("Knockback force for projectile hits")]
    [SerializeField] float knockbackForce = 100f; // Set knockback force

    [Tooltip("Maximum vertical knockback force")]
    [SerializeField] float maxVerticalKnockbackForce = 10f; // Adjust this value as needed

    private bool isActive = false;

    void Start()
    {
        StartCoroutine(ActivateAfterDelay(0.1f)); // Small delay before activation
    }

    IEnumerator ActivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isActive = true;
    }

    void OnParticleCollision(GameObject other)
    {
        if (isActive && (other.CompareTag("Player") || other.CompareTag("Dummy")))
        {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            Rigidbody otherRb = other.GetComponent<Rigidbody>();

            if (otherRb != null)
            {
                // Apply knockback force
                Vector3 knockbackForceVector = knockbackDirection * knockbackForce;

                // Clamp the y-axis value to prevent excessive vertical force
                knockbackForceVector.y = Mathf.Clamp(knockbackForceVector.y, -maxVerticalKnockbackForce, maxVerticalKnockbackForce);

                Debug.Log($"Applying knockback force: {knockbackForceVector}");
                otherRb.AddForce(knockbackForceVector, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Rigidbody component not found on the collided object.");
            }
        }
    }
}
