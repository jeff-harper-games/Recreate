using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Mouse Look")]
    public Camera fpsCamera;
    public float mouseSensitivity = 100.0f;
    private float xRotation = 0.0f;

    [Header("Movement")]
    public CharacterController controller;
    public float speed = 12.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;
    public KeyCode jumpKey = KeyCode.Space;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        fpsCamera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        controller.transform.Rotate(Vector3.up * mouseX);
        #endregion

        #region Movement
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2.0f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.fixedDeltaTime);

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);

        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
        #endregion
    }
}
