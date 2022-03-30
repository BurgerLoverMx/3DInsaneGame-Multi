using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public string PlayerName { get; private set; }
    public int PlayerHealth { get; private set; }

    public PlayerData(string playerName, int playerHealth) {
        PlayerName = playerName;
        PlayerHealth = playerHealth;
    }
}
