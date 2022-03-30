using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] 
    private float m_Speed = 12;
    
    [SerializeField]
    private float jumpHeight = 3f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundDistance = 0.4f;

    [SerializeField]
    private LayerMask groundMask;

    private Vector3 velocity;

    private bool isGrounded;

    private CharacterController m_CharController;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        enabled = IsClient;
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        m_CharController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
    }

    void Movement() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        Vector3 moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        m_CharController.Move(moveDirection * Time.deltaTime * m_Speed);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        m_CharController.Move(velocity * Time.deltaTime);
    }
}
