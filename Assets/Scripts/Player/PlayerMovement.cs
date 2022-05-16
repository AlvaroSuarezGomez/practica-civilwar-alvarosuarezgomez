 using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Netcode.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 direction;
        public Vector3 Direction => direction;
        private Vector3 velocity;
        public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
        [SerializeField] private float speed = 10f;
        [SerializeField] private float rotationSpeed = 90f;

        [Header("Animation")]
        [SerializeField] private Animator anim;
        //[SerializeField] private string idleAnimString;
        private int animSpeed = Animator.StringToHash("Speed");

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;
        private InputAction moveAction;

        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Camera cam;
        //[SerializeField] private GameObject enemyPlayer;

        [Header("Booleans")]
        private bool canMove = true;
        public bool CanMove { get { return canMove; } set { canMove = value; } }

        private void Awake()
        {
            //Physics.IgnoreCollision(GetComponent<Collider>(), enemyPlayer.GetComponent<Collider>());
        }
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

            inputActionAsset.Enable();
            moveAction = inputActionAsset.FindAction("Move");
        }

        private void FixedUpdate()
        {
            MoveCharacter();
        }

        private void MoveCharacter()
        {
            Vector2 moveDir = moveAction.ReadValue<Vector2>();
            direction = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f) * new Vector3(moveDir.x, 0f, moveDir.y);

            if ((canMove) && (direction.sqrMagnitude != 0f))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, transform.up), rotationSpeed * Time.fixedDeltaTime);
                velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
                rb.velocity = velocity;
                anim.SetFloat(animSpeed, speed * direction.sqrMagnitude);
            }
            else
            {
                anim.SetFloat(animSpeed, 0f);
            }
        }
    }
}
