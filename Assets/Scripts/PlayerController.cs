using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace King
{
    public class PlayerController : MonoBehaviour, ISentient
    {
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float movementSpeed = 5f;

        private InputAction horizontalAction;
        private InputAction verticalAction;
        private Dictionary<InputAction, Action> actions;
        public PlayerState CurrentState { get; private set; }

        private Vector3 rawRotation;
        private Quaternion rotation;

        private Vector3 nextSpace;
        private Vector3 previousSpace;

        private Animator anim;

        private Stack<Vector3> actionsTaken;

        #region Unity Callbacks

        private void Awake()
        {
            actions = new Dictionary<InputAction, Action>();

            horizontalAction = InputHandler.GetAction("Horizontal");
            verticalAction = InputHandler.GetAction("Vertical");
            actions.Add(InputHandler.GetAction("Movement"), Move);
            actions.Add(InputHandler.GetAction("Interact"), Interact);
            actions.Add(InputHandler.GetAction("Attack"), Attack);

            anim = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            rawRotation = Vector3.zero;
            rotation = transform.rotation;

            nextSpace = transform.position;
            previousSpace = transform.position;

            CurrentState = PlayerState.WaitingForTurn;
        }

        private void Update()
        {
            if (transform.rotation != rotation)
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        #endregion

        #region Input Callbacks

        private void CacheRotation(InputAction.CallbackContext context)
        {
            float axis = context.ReadValue<float>();
            Vector2 input;

            if (context.action == horizontalAction)
                input = new Vector2(axis, 0);
            else
                input = new Vector2(0, axis);

            // If the magnitude is zero we don't want to cache the value, it's useless and causes rotation issues
            if (input.magnitude > 0)
            {
                rawRotation = new Vector3(input.x, 0f, input.y);
                rotation = Quaternion.LookRotation(rawRotation, Vector3.up);
            }
        }

        private void PerformAction(InputAction.CallbackContext context)
        {
            InputAction actionType = context.action;

            // We are only allowed to commit to actions while the rotation keys are being pressed
            if (RotationIsPressed() && CurrentState != PlayerState.InAction && actions.ContainsKey(actionType))
            {
                CurrentState = PlayerState.InAction;
                actions[actionType].Invoke();
            }
        }

        #endregion

        #region Custom Methods

        private void Move()
        {
            StartCoroutine(Movement());
        }

        private void Interact()
        {
            // TODO
        }

        private void Attack()
        {
            // TODO
        }

        private IEnumerator Movement(float timeInSeconds = 1.0f)
        {
            previousSpace = nextSpace;
            nextSpace = previousSpace + rawRotation * 2;

            Debug.Log(rawRotation);

            anim.SetTrigger("Move");

            //for (float t = 0; t < 1f; t += Time.deltaTime / timeInSeconds)
            //{
            //    transform.position = Vector3.Lerp(transform.position, nextSpace, t);

            //    yield return null;
            //}

            while (transform.position != nextSpace)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextSpace, movementSpeed * Time.deltaTime);

                yield return null;
            }

            CurrentState = PlayerState.Deciding;
        }

        public bool RotationIsPressed()
        {
            return horizontalAction.IsPressed() || verticalAction.IsPressed();
        }


        private void EnableCharacterControls()
        {
            // When the map is enabled all the actions should be as well
            InputHandler.SetMapActive(true);

            horizontalAction.performed += CacheRotation;
            verticalAction.performed += CacheRotation;

            var keys = actions.Keys;
            foreach (var key in keys)
            {
                key.performed += PerformAction;
            }
        }
        private void DisableCharacterControls()
        {
            
            // When the map is disabled all the actions should be as well
            InputHandler.SetMapActive(false);

            horizontalAction.performed -= CacheRotation;
            verticalAction.performed -= CacheRotation;

            var keys = actions.Keys;
            foreach (var key in keys)
            {
                key.performed -= PerformAction;
            }
        }

        #endregion

        public IEnumerator StartTurn(GameTurnManager manager)
        {
            EnableCharacterControls();
            Debug.Log("Start Player Turn!");
            CurrentState = PlayerState.Deciding;

            while (CurrentState != PlayerState.InAction)
                yield return null;

            DisableCharacterControls();
            Debug.Log("End Player Turn!");
            manager.EndTurn();
        }


    }

    public enum PlayerState
    {
        WaitingForTurn,
        Deciding,
        InAction,
        Failed
    }
}
