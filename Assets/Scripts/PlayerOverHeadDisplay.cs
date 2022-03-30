using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerOverHeadDisplay : NetworkBehaviour
{
    [SerializeField] 
    private TMP_Text displayNameText;
    [SerializeField] 
    private NetworkVariable<FixedString32Bytes> displayName = new NetworkVariable<FixedString32Bytes>();
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        PlayerData? playerData = ConnectionManager.GetPlayerData(OwnerClientId);

        if (playerData.HasValue) {
            displayName.Value = playerData.Value.PlayerName;
        }
    }

    void Start() 
    {
        if (displayNameText.text.Equals("[Default Value]")) 
        {
            HandleDisplayName("[Default Value]", displayName.Value);
        }
    }

    private void OnEnable() 
    {
        displayName.OnValueChanged += HandleDisplayName;
    }

    private void HandleDisplayName(FixedString32Bytes oldVal, FixedString32Bytes newVal) 
    {
        displayNameText.SetText(newVal.ToString());
    }
}
