using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{

    void Update() {
        if (!IsOwner) return;

        if (Input.GetKey(KeyCode.Backspace))
        {
            ConnectionManager.Instance.Leave();
        }
    }
    /*
    public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();

    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            Move();
    }

    public void Move() {
        if (NetworkManager.Singleton.IsServer) {
            var randomPosition = GetRandomPositionOnPlane();
            position.Value = randomPosition;
        }
        else {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default) {
        position.Value = GetRandomPositionOnPlane();
    }

    static Vector3 GetRandomPositionOnPlane() {
        return new Vector3(Random.Range(-3f, 3f), 1, Random.Range(-3f, 3f));
    }

    void Update()
    {
        if (!IsOwner) return;
        
        transform.position = position.Value;
    }
    */
}
