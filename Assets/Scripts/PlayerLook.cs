using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField] private Transform playerBody;

    [SerializeField] 
    private float mouseSens = 250;

    [SerializeField]
    private Camera m_Camera;

    private float xRotation = 0f;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        enabled = IsClient;

        if (!IsOwner)
        {
            m_Camera.gameObject.SetActive(false);
            enabled = false;
            return;
        }
    }

    void Update()
    {
        Look();
    }

    private void Look()
    {
        playerBody.Rotate(0, (Input.GetAxis("Mouse X")) * Time.deltaTime * mouseSens, 0);

        xRotation -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSens;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
