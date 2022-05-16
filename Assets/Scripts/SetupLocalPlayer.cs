using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Netcode.Player
{
    public class SetupLocalPlayer : NetworkBehaviour
    {
        private void Start()
        {
            if (!IsLocalPlayer)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(GetComponentInChildren<PlayerMovement>());
                Destroy(GetComponentInChildren<PlayerJump>());
                Destroy(GetComponentInChildren<KRool.KRoolMoveset>());
            }
        }
    }
}
