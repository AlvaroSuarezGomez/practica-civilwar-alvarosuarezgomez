using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Netcode
{
    public class NetLogic : NetworkBehaviour
    {
        [SerializeField] private UnityTransport unityTransport;

        [SerializeField]
        private int maxPlayerCapacity = 2;
        
        private string playerName = "Player";

        private string ip = "127.0.0.1";

        [SerializeField]
        private TMP_InputField ipInputField;

        [SerializeField] private TextMeshProUGUI playerNameInputField;

        [SerializeField] private GameObject clientRoom;

        [SerializeField]
        private GameStarter gameStarter;

        private ulong networkClientId;

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

            if (gameStarter == null)
            {
                gameStarter = GetComponent<GameStarter>();
            }

            ipInputField.onValueChanged.AddListener(delegate { ip = ipInputField.text; });
        }

        private void Singleton_OnClientDisconnectCallback(ulong clientId)
        {
            gameStarter.ChangePlayerName(clientId, "");
        }

        private void Singleton_OnClientConnectedCallback(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                networkClientId = clientId;
                if (playerName != "")
                {
                    gameStarter.ChangePlayerName(clientId, playerName);
                }
                else
                {
                    gameStarter.ChangePlayerName(clientId, "Player " + clientId);
                }
            }
        }

        public void StartAsHost()
        {
            playerName = playerNameInputField.text;
            unityTransport.ConnectionData.Address = ip;
            
            NetworkManager.Singleton.StartHost();
        }

        public void StartAsClient()
        {
            playerName = playerNameInputField.text;
            unityTransport.ConnectionData.Address = ip;

            NetworkManager.Singleton.StartClient();

            clientRoom.SetActive(true);
        }

        public void StartGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("FinalDestination", LoadSceneMode.Single);
            StartCoroutine(OnLevelWasLoaded(1));
        }

        public void EndConnection()
        {
            NetworkManager.Singleton.Shutdown();
        }

        private IEnumerator OnLevelWasLoaded(int level)
        {
            if (level == 1)
            {
                gameStarter.SpawnPlayer(networkClientId);
                yield return null;
            }
        }
    }
}
