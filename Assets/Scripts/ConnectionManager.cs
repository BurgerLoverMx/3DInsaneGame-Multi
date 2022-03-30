using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour 
{
        public static ConnectionManager Instance => instance;
        private static ConnectionManager instance;

        [SerializeField] private GameObject connectionUI;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Slider healthSlider;

        [SerializeField] private Camera m_MainCamera;

        private static Dictionary<ulong, PlayerData> clientData;


        void Awake() 
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update() 
        {
            if (NetworkManager.Singleton.ShutdownInProgress) 
            {
                Debug.Log("Server shutting down.");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                m_MainCamera.gameObject.SetActive(true);
                connectionUI.SetActive(true);
            }
                
        }

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        public void Host()
        {
            Debug.LogFormat("Hosting started on client with ID {0}.", NetworkManager.Singleton.LocalClientId);
            clientData = new Dictionary<ulong, PlayerData>();

            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.NetworkConfig.ConnectionData = CreatePayload();
            NetworkManager.Singleton.StartHost();
        }

        public void Client()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = CreatePayload();
            NetworkManager.Singleton.StartClient();
        }

        public void Leave()
        {
            NetworkManager.Singleton.Shutdown();

            if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Host has left.");
                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            m_MainCamera.gameObject.SetActive(true);
            connectionUI.SetActive(true);
        }

        private byte[] CreatePayload() 
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerName = nameInputField.text,
                playerHealth = (int)healthSlider.value
            });

            return Encoding.ASCII.GetBytes(payload);
        }

        public static PlayerData? GetPlayerData(ulong clientId)
        {
            if (clientData.TryGetValue(clientId, out PlayerData playerData))
            {
                return playerData;
            }

            return null;
        }

        private void HandleClientConnected(ulong clientId)
        {
            Debug.LogFormat("Client with ID {0} just joined.", clientId);
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                m_MainCamera.gameObject.SetActive(false);
                connectionUI.SetActive(false);
            }
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            Debug.LogFormat("Client with ID {0} just disconnected.", clientId);
            if (NetworkManager.Singleton.IsServer)
            {
                Debug.LogFormat("Server removing client {0} from dictionnary.", clientId);
                clientData.Remove(clientId);
            }

            // Are we the client that is disconnecting?
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                m_MainCamera.gameObject.SetActive(true);
                connectionUI.SetActive(true);
            }
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            string payload = Encoding.ASCII.GetString(connectionData);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

            bool approveConnection = true;
            Vector3 spawnPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
            Quaternion spawnRot = Quaternion.identity;

            if (approveConnection)
            {

                clientData[clientId] = new PlayerData(connectionPayload.playerName, connectionPayload.playerHealth);
            }

            callback(true, null, approveConnection, spawnPos, spawnRot);
        }
    }