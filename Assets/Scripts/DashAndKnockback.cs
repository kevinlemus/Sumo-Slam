using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAndKnockback : MonoBehaviour
{
    [Tooltip("Speed of the dash movement")]
    [SerializeField] float dashSpeed = 20f;

    [Tooltip("Duration of the dash in seconds")]
    [SerializeField] float dashDuration = 0.2f;

    [Tooltip("Cooldown time between dashes in seconds")]
    [SerializeField] float dashCooldown = 2f;

    [Tooltip("Knockback force for regular walking bumps")]
    [SerializeField] float regularBumpKnockbackForce = 5f;

    [Tooltip("Knockback force for dash attacks")]
    [SerializeField] float dashKnockbackForce = 15f;

    [Tooltip("Knockback force applied to the attacker during a dash")]
    [SerializeField] float attackerKnockbackForce = 7.5f;

    [Tooltip("Additional knockback force for critical hits")]
    [SerializeField] float criticalHitKnockbackForce = 5f;

    [Tooltip("Maximum knockback force for food-based knockback")]
    [SerializeField] float maxFoodKnockbackForce = 25f;

    private bool isDashing = false;
    private bool canDash = true;
    private Rigidbody rb;
    private int foodCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (canDash && !isDashing && Input.GetKeyDown(KeyCode.Space))
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
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

            if (otherRb != null)
            {
                float appliedKnockbackForce = isDashing ? dashKnockbackForce : regularBumpKnockbackForce;

                // Apply critical hit knockback if hit from front or back
                if (isDashing && (Vector3.Dot(knockbackDirection, transform.forward) > 0.8f || Vector3.Dot(knockbackDirection, transform.forward) < -0.8f))
                {
                    appliedKnockbackForce += criticalHitKnockbackForce;
                }

                // Apply food-based knockback
                appliedKnockbackForce += foodCount * (maxFoodKnockbackForce / 5);

                // Clamp the knockback force to ensure it doesn't exceed the specified value
                Vector3 knockbackForce = knockbackDirection * appliedKnockbackForce;
                if (knockbackForce.magnitude > dashKnockbackForce)
                {
                    knockbackForce = knockbackForce.normalized * dashKnockbackForce;
                }

                otherRb.AddForce(knockbackForce, ForceMode.Impulse);

                // Apply knockback to the attacker during a dash, but with a smaller force
                if (isDashing)
                {
                    Vector3 attackerKnockbackForceVector = -knockbackDirection * attackerKnockbackForce;
                    if (attackerKnockbackForceVector.magnitude > attackerKnockbackForce)
                    {
                        attackerKnockbackForceVector = attackerKnockbackForceVector.normalized * attackerKnockbackForce;
                    }
                    rb.AddForce(attackerKnockbackForceVector, ForceMode.Impulse);
                }
                else
                {
                    rb.AddForce(-knockbackDirection * appliedKnockbackForce, ForceMode.Impulse);
                }
            }
        }
    }

    public void CollectFood()
    {
        foodCount = Mathf.Clamp(foodCount + 1, 0, 5);
    }
}
