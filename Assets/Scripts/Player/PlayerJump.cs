using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Netcode.Player
{
    public class PlayerJump : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;

        [Header("Input")]
        [SerializeField] private InputActionReference jumpActionReference;

        [Header("Animations")]
        [SerializeField] private Animator anim;
        [SerializeField] private NetworkAnimator netAnim;
        private int animGrounded = Animator.StringToHash("Grounded");
        private int animJump = Animator.StringToHash("Jump");

        [Header("Ground Check")]
        [SerializeField] private float groundDistance;
        [SerializeField] private LayerMask groundLayer;
        private bool isGrounded = false;
        public bool IsGrounded => isGrounded;

        [Header("Ground Check")]
        private bool canJump = true;
        public bool CanJump { get { return canJump; } set { canJump = value; } }
        [SerializeField] private int maxJumpNumber;
        private int jumpNumber = 2;
        [SerializeField] private float jumpHeight;

        private void Start()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }

            jumpActionReference.action.performed += Action_performed;
        }

        private void FixedUpdate()
        {
            GroundCheck();
            RecoverJump();
        }

        private void GroundCheck()
        {
            if (Physics.Raycast(transform.position, Vector3.down, groundDistance, groundLayer))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            anim.SetBool(animGrounded, isGrounded);
        }

        private void RecoverJump()
        {
            if (isGrounded)
            {
                jumpNumber = maxJumpNumber;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void JumpOnServerRpc()
        {
            if ((jumpNumber > 0) && canJump)
            {
                netAnim.SetTrigger(animJump);
                CallJumpAnimationOnClientRpc();
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.velocity += new Vector3(0f, jumpHeight, 0f);
                jumpNumber--;
            }
        }

        [ClientRpc]
        private void CallJumpAnimationOnClientRpc()
        {
            anim.SetTrigger(animJump);
        }

        private void Action_performed(InputAction.CallbackContext obj)
        {
            JumpOnServerRpc();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, Vector3.down * groundDistance);
        }
    }
}
