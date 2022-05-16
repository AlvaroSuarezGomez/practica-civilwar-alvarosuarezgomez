using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Netcode.Player
{
    public class TriggerHitBox : NetworkBehaviour
    {
        [SerializeField] private Collider col;

        [SerializeField] private float attackForce;
        public float AttackForce => attackForce;

        [SerializeField] private float attackDamage;
        public float AttackDamage => attackDamage;

        private Vector3 attackDirection = new Vector3(0, 10, 0);
        
        public Vector3 AttackDirection
        {
            get { return attackDirection; }
            set { attackDirection = value; }
        }

        private ColliderManager colliderManager;
        private PlayerHealth playerHealth;

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.tag == "Player") && (other.gameObject.GetComponent<Rigidbody>() != null) && (GetComponentInParent<PlayerHealth>().gameObject != other.gameObject))
            {
                colliderManager = other.gameObject.GetComponent<ColliderManager>();
                playerHealth = other.gameObject.GetComponent<PlayerHealth>();
                //HitServerRpc();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.gameObject.tag == "Player") && (other.gameObject.GetComponent<Rigidbody>() != null) && (GetComponentInParent<PlayerHealth>().gameObject != other.gameObject))
            {
                colliderManager = null;
                playerHealth = null;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetEnabledColliderServerRpc(bool enabled)
        {
            col.enabled = enabled;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetAttackDirectionServerRpc(Vector3 direction)
        {
            attackDirection = direction;
        }

        [ServerRpc(RequireOwnership = false)]
        private void HitServerRpc()
        {
            Debug.Log("Hit");
            playerHealth.RiseHealthServerRpc(attackDamage);
            //colliderManager.ApplyForce(attackDirection * attackForce * playerHealth.Health / playerHealth.LaunchOffset);
        }
    }
}
