using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Netcode
{
    public class GameStarter : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> playersCharacters = new List<GameObject>();
        [SerializeField] private List<GameObject> playersNamesText = new List<GameObject>();

        public void SpawnPlayer(ulong clientID)
        {
            StartCoroutine(SpawnPlayerAfterFrame(clientID));
        }

        private IEnumerator SpawnPlayerAfterFrame(ulong clientID)
        {
            yield return null;
            SpawnPlayerServerRpc(clientID);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ulong clientID)
        {
            GameObject playerGameObject;

            playerGameObject = Instantiate(playersCharacters[Convert.ToInt32(clientID)]);

            var networkObject = playerGameObject.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientID);
        }

        public void ChangePlayerName(ulong clientID, string playerName)
        {
            StartCoroutine(ChangePlayerNameAfterFrame(clientID, playerName));
        }

        private IEnumerator ChangePlayerNameAfterFrame(ulong clientID, string playerName)
        {
            yield return null;
            ChangePlayerNameServerRpc(clientID, playerName);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerNameServerRpc(ulong clientID, string playerName)
        {
            
            if (clientID == 0)
            {
                var playerText = playersNamesText[0];
                playerText.GetComponent<PlayerName>().SetPlayerName(playerName);
            }
            
            else if (clientID > 0)
            {
                var playerText = playersNamesText[1];
                playerText.GetComponent<PlayerName>().SetPlayerName(playerName);
            }

            
        }
    }
}
