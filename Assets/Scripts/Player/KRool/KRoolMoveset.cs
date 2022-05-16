using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Netcode.Player.KRool
{
    public class KRoolMoveset : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerJump playerJump;

        [Header("Input")]
        [SerializeField] private InputActionReference principalAttackInputAction;
        [SerializeField] private InputActionReference secondaryAttackInputAction;

        [Header("Animation")]
        [SerializeField] private Animator anim;
        private int attackIndex = Animator.StringToHash("AttackIndex");

        [Header("Hitboxes")]
        [SerializeField] private TriggerHitBox forwardTiltHitbox;
        [SerializeField] private TriggerHitBox dashAttackHitbox;
        [SerializeField] private TriggerHitBox forwardAirHitbox;
        [SerializeField] private TriggerHitBox neutralAirHitbox;
        [SerializeField] private TriggerHitBox downAirHitbox;

        [Header("Attack Times")]
        private bool canAttack = true;
        [SerializeField] private float forwardTiltStartTime;
        [SerializeField] private float forwardTiltEndTime;
        [SerializeField] private float dashAttackStartTime;
        [SerializeField] private float dashAttackEndTime;
        [SerializeField] private float forwardAirStartTime;
        [SerializeField] private float forwardAirEndTime;
        [SerializeField] private float neutralAirStartTime;
        [SerializeField] private float neutralAirEndTime;
        [SerializeField] private float downAirStartTime;
        [SerializeField] private float downAirEndTime;

        private void Awake()
        {
            principalAttackInputAction.action.performed += Principal_Attack_performed;
            secondaryAttackInputAction.action.performed += Secondary_Attack_performed;
        }

        private void Secondary_Attack_performed(InputAction.CallbackContext obj)
        {
            SecondaryAttackServerRpc();
        }

        private void Principal_Attack_performed(InputAction.CallbackContext obj)
        {
            PrimaryAttackServerRpc();
        }

        private IEnumerator ForwardTiltAttack()
        {
            canAttack = false;
            playerMovement.CanMove = false;
            playerMovement.Velocity = new Vector3(0f, playerMovement.Velocity.y, 0f);
            playerJump.CanJump = false;
            anim.SetInteger(attackIndex, 3);
            yield return new WaitForSeconds(forwardTiltStartTime);
            forwardTiltHitbox.SetEnabledColliderServerRpc(true);
            forwardTiltHitbox.SetAttackDirectionServerRpc(transform.forward + (transform.up * 0.75f));
            anim.SetInteger(attackIndex, 0);
            yield return new WaitForSeconds(forwardTiltEndTime);
            forwardTiltHitbox.SetEnabledColliderServerRpc(false);
            canAttack = true;
            playerMovement.CanMove = true;
            playerJump.CanJump = true;
        }

        private IEnumerator DashAttack()
        {
            canAttack = false;
            playerMovement.CanMove = false;
            playerJump.CanJump = false;
            anim.SetInteger(attackIndex, 4);
            yield return new WaitForSeconds(dashAttackStartTime);
            dashAttackHitbox.SetEnabledColliderServerRpc(true);
            dashAttackHitbox.SetAttackDirectionServerRpc(transform.forward + (transform.up * 0.9f));
            anim.SetInteger(attackIndex, 0);
            yield return new WaitForSeconds(dashAttackEndTime);
            dashAttackHitbox.SetEnabledColliderServerRpc(false);
            canAttack = true;
            playerMovement.CanMove = true;
            playerJump.CanJump = true;
        }

        private IEnumerator ForwardAirAttack()
        {
            playerJump.CanJump = false;
            canAttack = false;
            anim.SetInteger(attackIndex, 1);
            yield return new WaitForSeconds(forwardAirStartTime);
            forwardAirHitbox.SetEnabledColliderServerRpc(true);
            forwardAirHitbox.SetAttackDirectionServerRpc(transform.forward + (transform.up * 0.75f));
            anim.SetInteger(attackIndex, 0);
            yield return new WaitForSeconds(forwardAirEndTime);
            forwardAirHitbox.SetEnabledColliderServerRpc(false);
            canAttack = true;
            playerJump.CanJump = true;

        }

        private IEnumerator NeutralAirAttack()
        {
            canAttack = false;
            playerJump.CanJump = false;
            anim.SetInteger(attackIndex, 2);
            yield return new WaitForSeconds(neutralAirStartTime);
            neutralAirHitbox.SetEnabledColliderServerRpc(true);
            neutralAirHitbox.SetAttackDirectionServerRpc(transform.forward + (transform.up * 0.4f));
            anim.SetInteger(attackIndex, 0);
            yield return new WaitForSeconds(neutralAirEndTime);
            neutralAirHitbox.SetEnabledColliderServerRpc(false);
            playerJump.CanJump = true;
            canAttack = true;
        }
        private IEnumerator DownAirAttack()
        {
            canAttack = false;
            playerJump.CanJump = false;
            anim.SetInteger(attackIndex, 5);
            yield return new WaitForSeconds(downAirStartTime);
            downAirHitbox.SetEnabledColliderServerRpc(true);
            downAirHitbox.SetAttackDirectionServerRpc(Vector3.down + (transform.forward * 0.25f));
            anim.SetInteger(attackIndex, 0);
            yield return new WaitForSeconds(downAirEndTime);
            downAirHitbox.SetEnabledColliderServerRpc(false);
            playerJump.CanJump = true;
            canAttack = true;
        }

        private void PrimaryAttackServerRpc()
        {
            if (canAttack)
            {
                if ((playerMovement.Direction.sqrMagnitude != 0) && !playerJump.IsGrounded)
                {
                    StartCoroutine(ForwardAirAttack());
                }
                else if ((Mathf.Round(playerMovement.Direction.sqrMagnitude) == 0) && !playerJump.IsGrounded)
                {
                    StartCoroutine(NeutralAirAttack());
                }
                else if ((Mathf.Round(playerMovement.Direction.sqrMagnitude) == 0) && playerJump.IsGrounded)
                {
                    StartCoroutine(ForwardTiltAttack());
                }
                else if ((playerMovement.Direction.sqrMagnitude != 0) && playerJump.IsGrounded)
                {
                    StartCoroutine(DashAttack());
                }
            }
        }

        private void SecondaryAttackServerRpc()
        {
            if (canAttack)
            {
                if (!playerJump.IsGrounded)
                {
                    StartCoroutine(DownAirAttack());
                }
            }
        }
    }
}
