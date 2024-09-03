using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DashAndKnockback))]
public class Mover : MonoBehaviour
{
    [Tooltip("Speed of the player movement")]
    [SerializeField] float moveSpeed = 10f;

    [Tooltip("Rotation speed of the player")]
    [SerializeField] float rotationSpeed = 20f; // Increased rotation speed

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PrintInstruction();
    }

    void Update()
    {
        MovePlayer();
    }

    void PrintInstruction()
    {
        Debug.Log("Welcome to the game");
        Debug.Log("Move your player with WASD or arrow keys");
        Debug.Log("Press Spacebar to dash");
        Debug.Log("Don't hit the walls!");
    }

    void MovePlayer()
    {
        float xValue = Input.GetAxis("Horizontal");
        float zValue = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xValue, 0, zValue);

        // Normalize the movement vector to ensure consistent speed in all directions
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        moveDirection *= moveSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(moveDirection, Space.World);
    }
}
