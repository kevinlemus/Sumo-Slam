using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAndKnockback : MonoBehaviour
{
    [Tooltip("Speed of the dash movement")]
    [SerializeField] float dashSpeed = 8f;

    [Tooltip("Duration of the dash in seconds")]
    [SerializeField] float dashDuration = 0.2f;

    [Tooltip("Cooldown time between dashes in seconds")]
    [SerializeField] float dashCooldown = 1f;

    [Tooltip("Knockback force for regular walking bumps")]
    [SerializeField] float regularBumpKnockbackForce = 100f; // Adjusted for proportionality

    [Tooltip("Knockback force for dash attacks")]
    [SerializeField] float dashKnockbackForce = 200f;

    [Tooltip("Knockback force applied to the attacker during a dash")]
    [SerializeField] float attackerKnockbackForce = 300f; // Increased for stronger rebound

    private bool isDashing = false;
    private bool canDash = true;
    private Rigidbody rb;
    private Coroutine disableMovementCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (gameObject.tag != "Dummy" && canDash && !isDashing && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            float xValue = Input.GetAxis("Horizontal") * dashSpeed;
            float zValue = Input.GetAxis("Vertical") * dashSpeed;

            rb.velocity = new Vector3(xValue, 0, zValue);
            yield return null;
        }

        rb.velocity = Vector3.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Dummy"))
        {
            Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

            if (otherRb != null)
            {
                // Apply knockback force for regular walking bumps
                if (!isDashing)
                {
                    rb.AddForce(-knockbackDirection * regularBumpKnockbackForce, ForceMode.Impulse);
                }

                // Apply knockback force for dash attacks
                if (isDashing)
                {
                    Vector3 knockbackForce = knockbackDirection * dashKnockbackForce;
                    otherRb.AddForce(knockbackForce, ForceMode.Impulse);

                    // Apply knockback to the attacker during a dash, but with a stronger force
                    rb.velocity = Vector3.zero; // Stop the dash
                    Vector3 attackerKnockbackForceVector = -knockbackDirection * attackerKnockbackForce;
                    rb.AddForce(attackerKnockbackForceVector, ForceMode.Impulse);

                    // Ensure the previous coroutine is stopped before starting a new one
                    if (disableMovementCoroutine != null)
                    {
                        StopCoroutine(disableMovementCoroutine);
                    }
                    disableMovementCoroutine = StartCoroutine(DisableMovementAfterDash());
                }
            }
        }
    }

    IEnumerator DisableMovementAfterDash()
    {
        float originalDashSpeed = dashSpeed;
        dashSpeed = 0f; // Disable movement
        yield return new WaitForSeconds(1f); // Pause duration
        dashSpeed = originalDashSpeed; // Gradually allow movement again
        Debug.Log("Dash speed restored: " + dashSpeed);

        // Safeguard to ensure dashSpeed and canDash are restored
        if (dashSpeed == 0f)
        {
            dashSpeed = originalDashSpeed;
        }
        canDash = true;
    }
}