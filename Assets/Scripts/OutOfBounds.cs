using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the player object
            other.gameObject.SetActive(false);
            Debug.Log("Player went out of bounds and has been disabled.");

            // Disable movement and dash scripts
            Mover mover = other.GetComponent<Mover>();
            DashAndKnockback dashAndKnockback = other.GetComponent<DashAndKnockback>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (mover != null)
            {
                mover.enabled = false;
            }

            if (dashAndKnockback != null)
            {
                dashAndKnockback.enabled = false;
            }

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }
    }
}
