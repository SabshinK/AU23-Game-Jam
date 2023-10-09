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

        private Animator anim;
        /* 
         * I wanted this to be a bool but for some reason you can't set bool property values
         * inside animation events, even though ints and even enums can be set?? Idk man
         */
        public int Animating { get; set; }

        private GameTurnManager manager;

        private Stack<Vector3> actionsTaken;

        private bool canMove;

        public delegate void OnActionFinished();
        public event OnActionFinished onActionFinished;

        #region Unity Callbacks

        private void Awake()
        {
            actions = new Dictionary<InputAction, Action>();
            actionsTaken = new Stack<Vector3>();

            horizontalAction = InputHandler.GetAction("Horizontal");
            verticalAction = InputHandler.GetAction("Vertical");
            actions.Add(InputHandler.GetAction("Movement"), Move);
            actions.Add(InputHandler.GetAction("Interact"), Interact);
            actions.Add(InputHandler.GetAction("Attack"), Attack);
            actions.Add(InputHandler.GetAction("Undo"), Undo);

            anim = GetComponentInChildren<Animator>();
            manager = FindObjectOfType<GameTurnManager>();
        }

        private void OnEnable()
        {
            horizontalAction.performed += CacheRotation;
            verticalAction.performed += CacheRotation;

            var keys = actions.Keys;
            foreach (var key in keys)
            {
                key.performed += PerformAction;
            }
        }

        private void OnDisable()
        {
            horizontalAction.performed -= CacheRotation;
            verticalAction.performed -= CacheRotation;

            var keys = actions.Keys;
            foreach (var key in keys)
            {
                key.performed -= PerformAction;
            }
        }

        private void Start()
        {
            rawRotation = Vector3.zero;
            rotation = transform.rotation;

            nextSpace = transform.position;

            CurrentState = PlayerState.Waiting;
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

            CheckCanMove();
        }

        private void PerformAction(InputAction.CallbackContext context)
        {
            InputAction actionType = context.action;

            // We are only allowed to commit to actions while the rotation keys are being pressed
            if (CurrentState != PlayerState.InAction && actions.ContainsKey(actionType))
            {
                actions[actionType].Invoke();
            }
        }

        #endregion

        #region Custom Methods

        private void Move()
        {
            if (RotationIsPressed() && canMove)
            {
                // Modifying turn counter must go before the player is "In Action"
                manager.turnCount--;

                CurrentState = PlayerState.InAction;

                // Assign next space
                Vector3 previousSpace = nextSpace;
                nextSpace = previousSpace + rawRotation * 2;

                // Cache move
                actionsTaken.Push(previousSpace);                

                // Start the animations and such
                StartCoroutine(SetSpace());
            }
        }

        private void Undo()
        {
            if (actionsTaken.Count > 0)
            {
                // Same stuff as move but turning back the clock
                manager.turnCount++;

                CurrentState = PlayerState.InAction;
               
                nextSpace = actionsTaken.Pop();                

                StartCoroutine(SetSpace());
            }
        }

        private void Interact()
        {
            // TODO
        }

        private void Attack()
        {
            // TODO
        }

        private IEnumerator SetSpace(float timeInSeconds = 1.0f)
        {
            anim.SetTrigger("Move");

            while (transform.position != nextSpace)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextSpace, movementSpeed * Time.deltaTime);

                yield return null;
            }

            // Wait until the movement animation is finished
            yield return new WaitUntil(() => { return Animating == 0; });

            CurrentState = PlayerState.Deciding;

            onActionFinished?.Invoke();
        }

        public bool RotationIsPressed()
        {
            return horizontalAction.IsPressed() || verticalAction.IsPressed();
        }
        
        public IEnumerator StartTurn(GameTurnManager manager)
        {
            InputHandler.SetMapActive(true);
            Debug.Log("Start Player Turn!");

            CurrentState = PlayerState.Deciding;

            while (CurrentState != PlayerState.InAction)
                yield return null;

            InputHandler.SetMapActive(false);
            Debug.Log("End Player Turn!");

            manager.EndTurn();
        }

        public void CheckCanMove()
        {
            canMove = !Physics.Raycast(nextSpace, rawRotation * 2, out RaycastHit hitInfo, 2f);

            Color color;
            if (canMove)
                color = Color.green;
            else
                color = Color.red;

            Debug.DrawRay(nextSpace, rawRotation * 2, color, 1f);
        }

        #endregion
    }

    public enum PlayerState
    {
        Waiting,
        Deciding,
        InAction,
        Failed
    }
}
