using King;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;

    private InputAction rotationAction;
    private InputAction movementAction;

    private Vector3 rawRotation;
    private Quaternion rotation;

    private void Awake()
    {
        rotationAction = InputHandler.GetAction("Rotation");
        movementAction = InputHandler.GetAction("Movement");

        rawRotation = Vector3.zero;
        rotation = transform.rotation;
    }

    private void OnEnable()
    {
        // Enable actions
        rotationAction.Enable();
        movementAction.Enable();

        rotationAction.performed += CacheRotation;
    }

    private void OnDisable()
    {
        // Disable actions
        rotationAction.Disable();
        movementAction.Disable();

        rotationAction.performed -= CacheRotation;
    }

    private void CacheRotation(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        rawRotation = new Vector3(input.x, 0f, input.y);
        rotation = Quaternion.LookRotation(rawRotation, Vector3.up);
    }

    private void Update()
    {
        if (transform.rotation != rotation)
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
