using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace King
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float movementSpeed = 5f;

        private InputAction rotationAction;
        private Dictionary<InputAction, Action> actions;

        private Vector3 rawRotation;
        private Quaternion rotation;

        private Vector3 nextSpace;
        private Vector3 previousSpace;

        private Animator anim;

        #region Unity Callbacks

        private void Awake()
        {
            actions = new Dictionary<InputAction, Action>();

            rotationAction = InputHandler.GetAction("Rotation");
            actions.Add(InputHandler.GetAction("Movement"), Move);
            actions.Add(InputHandler.GetAction("Interact"), Interact);
            actions.Add(InputHandler.GetAction("Attack"), Attack);

            rawRotation = Vector3.zero;
            rotation = transform.rotation;

            nextSpace = transform.position;
            previousSpace = transform.position;

            anim = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            // When the map is enabled all the actions should be as well
            InputHandler.SetMapActive(true);

            rotationAction.performed += CacheRotation;

            var keys = actions.Keys;
            foreach (var key in keys)
            {
                key.performed += PerformAction;
            }
        }

        private void OnDisable()
        {
            // When the map is disabled all the actions should be as well
            InputHandler.SetMapActive(false);

            rotationAction.performed -= CacheRotation;
        }

        private void Update()
        {
            if (transform.rotation != rotation)
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, nextSpace, movementSpeed * Time.deltaTime);
        }

        #endregion

        #region Input Callbacks

        private void CacheRotation(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            rawRotation = new Vector3(input.x, 0f, input.y);
            rotation = Quaternion.LookRotation(rawRotation, Vector3.up);
        }

        private void PerformAction(InputAction.CallbackContext context)
        {
            InputAction actionType = context.action;

            // We are only allowed to commit to actions while the rotation keys are being pressed
            if (rotationAction.IsPressed() && actions.ContainsKey(actionType))
            {
                actions[actionType].Invoke();
            }
        }

        #endregion

        #region Custom Methods

        private void Move()
        {

        }

        private void Interact()
        {
            Debug.Log("Interaction!");
        }

        private void Attack()
        {
            Debug.Log("Attack!");
        }

        private IEnumerator Movement()
        {
            previousSpace = nextSpace;
            nextSpace = previousSpace + rawRotation * 2;

            anim.SetTrigger("Move");

            while (transform.position != nextSpace)
            {
                transform.position = Vector3.MoveTowards(nextSpace, transform.position, movementSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}
