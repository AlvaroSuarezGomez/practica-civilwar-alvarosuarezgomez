using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;
using System;

namespace Netcode {
    public class PlayerName : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;

        private NetworkVariable<FixedString64Bytes> networkPlayerName = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            networkPlayerName.OnValueChanged = OnPlayerNameUpdated;
            playerNameText.text = networkPlayerName.Value.Value;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            playerNameText.text = networkPlayerName.Value.Value;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            playerNameText.text = networkPlayerName.Value.Value;
        }

        private void OnPlayerNameUpdated(FixedString64Bytes previousValue, FixedString64Bytes newValue)
        {
            playerNameText.text = networkPlayerName.Value.Value;
        }

        public void SetPlayerName(string playerName)
        {
            playerNameText.text = playerName;

            networkPlayerName.Value = playerName;
        }
    }
}
