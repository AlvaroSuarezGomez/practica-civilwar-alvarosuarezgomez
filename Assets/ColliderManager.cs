using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Netcode.Player
{
    public class ColliderManager : NetworkBehaviour
    {
        [SerializeField]
        private Rigidbody rb;
        private Collider enemyCollider;
        [SerializeField]
        private PlayerHealth playerHealth;

        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            if (playerHealth == null)
            {
                playerHealth = GetComponent<PlayerHealth>();
            }
        }

        [ClientRpc]
        public void ApplyForceClientRpc(Vector3 force)
        {
            Debug.Log("Hit");
            rb.AddForce(force);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                enemyCollider = collision.gameObject.GetComponent<Collider>();
                DisablePlayerCollision();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Hitbox")
            {
                if (other.gameObject.GetComponent<TriggerHitBox>() != null)
                {
                    TriggerHitBox hitbox = other.gameObject.GetComponent<TriggerHitBox>();
                    playerHealth.RiseHealthServerRpc(hitbox.AttackDamage);
                    ApplyForceClientRpc(hitbox.AttackDirection * hitbox.AttackForce * playerHealth.Health/playerHealth.LaunchOffset);
                }
            }
        }

        private void DisablePlayerCollision()
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), enemyCollider);
        }
    }
}
