using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private Vector3 respawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private float maxHealth = 999.9f;
    [SerializeField] private float health = 0f;
    [SerializeField] private float launchOffset = 5f;
    private NetworkVariable<float> networkHealth = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Server);

    public float Health => health;
    public float LaunchOffset => launchOffset;

    private void Awake()
    {
        networkHealth.OnValueChanged = OnHealthUpdated;

        health = networkHealth.Value;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        health = networkHealth.Value;
    }

    private void OnHealthUpdated(float previousValue, float newValue)
    {
        health = networkHealth.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RiseHealthServerRpc(float damage)
    {
            if ((health + damage) < 0)
            {
                networkHealth.Value = 0f;
                health = 0f;
            }

            else if ((health + damage) > maxHealth)
            {
                networkHealth.Value = maxHealth;
                health = maxHealth;
            }

            else
            {
                networkHealth.Value += damage;
                health += damage;
        }
    }

    private void Die()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = respawnPosition;
        RiseHealthServerRpc(-999.9f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Death")
        {
            Die();
        }
    }
}
