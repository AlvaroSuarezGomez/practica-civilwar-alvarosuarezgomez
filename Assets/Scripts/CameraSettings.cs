using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Transform lookObject;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool look;
    [SerializeField] private InputActionAsset input;
    private InputAction moveAction;

    private void Awake()
    {
        input.Enable();
        moveAction = input.FindAction("Move");
    }

    private void Update()
    {
        transform.position = parent.position + (transform.rotation * offset);
        Vector2 moveDir = moveAction.ReadValue<Vector2>();

        transform.RotateAround(transform.position, Vector3.up, -moveDir.x * rotationSpeed * Time.deltaTime);
    }
}
