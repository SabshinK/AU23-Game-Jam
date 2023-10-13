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

        [SerializeField] private LayerMask hurtboxMask;

        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip undoClip;

        private InputAction horizontalAction;
        private InputAction verticalAction;
        private Dictionary<InputAction, Action> actions;
        public PlayerState CurrentState { get; private set; }

        private Vector3 rawRotation;
        public Vector3 RawRotation => rawRotation;
        private Quaternion rotation;

        public Vector3 NextSpace { get; set; }

        private AudioSource audioSource;
        private Animator anim;
        /* 
         * I wanted this to be a bool but for some reason you can't set bool property values
         * inside animation events, even though ints and even enums can be set?? Idk man
         */
        public int Animating { get; set; }

        private GameTurnManager manager;

        private Stack<IAction> actionsTaken;

        private bool canMove;
        public bool CanMove => canMove;

        public delegate void OnActionFinished();
        public event OnActionFinished onActionFinished;

        #region Unity Callbacks

        private void Awake()
        {
            actions = new Dictionary<InputAction, Action>();
            actionsTaken = new Stack<IAction>();

            horizontalAction = InputHandler.GetAction("Horizontal");
            verticalAction = InputHandler.GetAction("Vertical");
            actions.Add(InputHandler.GetAction("Movement"), Move);
            actions.Add(InputHandler.GetAction("Interact"), Interact);
            actions.Add(InputHandler.GetAction("Attack"), Attack);
            actions.Add(InputHandler.GetAction("Undo"), Undo);

            audioSource = GetComponent<AudioSource>();
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

            onActionFinished += CheckHurtbox;
            onActionFinished += ActionFinishedAudio;
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

            onActionFinished -= CheckHurtbox;
            onActionFinished -= ActionFinishedAudio;
        }

        private void Start()
        {
            rawRotation = Vector3.zero;
            rotation = transform.rotation;

            NextSpace = transform.position;

            CurrentState = PlayerState.Deciding;
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
            if (RotationIsPressed() && canMove && manager.TurnCount > 0)
            {
                // Modifying turn counter must go before the player is "In Action"
                manager.TurnCount--;

                CurrentState = PlayerState.InAction;

                // Cache move
                IAction move = new Move(this);
                actionsTaken.Push(move);

                // Assign next space
                Vector3 previousSpace = NextSpace;
                NextSpace = previousSpace + rawRotation * 2;

                // Set audio clip
                audioSource.clip = moveClip;

                /* 
                 * Start the animations and such. I am passing a bool that lets downstream methods
                 * know what action we just came from. It's not my preferred method but it works.
                 * The more ideal version of this method would be IActions that get passed to the
                 * event and operate in conjunction with the PlayerState enum. It would offload code
                 * into separate classes.
                 */
                StartCoroutine(SetSpace());
            }
        }

        private void Undo()
        {
            if (actionsTaken.Count > 0)
            {
                manager.TurnCount++;

                CurrentState = PlayerState.InAction;

                // Check if the action is a Damaged instance, we want to clear this out first
                if (actionsTaken.Peek() is Damaged)
                {
                    IAction damagedAction = actionsTaken.Pop();
                    damagedAction.Undo();
                }

                /*
                 * There is one condition here that isn't being checked, that being whether the stack is
                 * empty after clearing the damaged actions. This case is basically impossible though 
                 * because it would require the player taking a bunch of damage at the beginning of the
                 * level without having made any moves, which won't feasibly happen.
                 */

                // Undo the last action
                IAction lastAction = actionsTaken.Pop();
                lastAction.Undo();

                // Set the audio clip
                audioSource.clip = undoClip;

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

        public void StartTurn(GameTurnManager manager)
        {
            StartCoroutine(PlayerTurn(manager));
        }
        
        public IEnumerator PlayerTurn(GameTurnManager manager)
        {
            /*
             * Make sure the player finished their previous turn before they start their next one, give 
             * time for the animations to finish, etc. On Start the player is deciding so this should 
             * be fine
             */
            yield return new WaitUntil(() => { return CurrentState != PlayerState.InAction; });

            InputHandler.SetMapActive(true);
            //Debug.Log("Start Player Turn!");

            CurrentState = PlayerState.Deciding;

            // Wait until player takes an action
            yield return new WaitUntil(() => { return CurrentState == PlayerState.InAction; });

            InputHandler.SetMapActive(false);
            //Debug.Log("End Player Turn!");

            // End the player turn while they are doing animations so the world can update
            manager.EndTurn();
        }

        private IEnumerator SetSpace()
        {
            anim.SetTrigger("Move");

            while (transform.position != NextSpace)
            {
                transform.position = Vector3.MoveTowards(transform.position, NextSpace, movementSpeed * Time.deltaTime);

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

        public void CheckCanMove()
        {
            canMove = !Physics.Raycast(NextSpace, rawRotation * 2, out RaycastHit hitInfo, 2f) && manager.TurnCount > 0;

            Color color;
            if (canMove)
                color = Color.green;
            else
                color = Color.red;

            Debug.DrawRay(NextSpace, rawRotation * 2, color, 1f);
        }

        private void CheckHurtbox()
        {
            foreach (Collider col in Physics.OverlapBox(NextSpace - new Vector3(0, 1, 0), new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, hurtboxMask))
            {
                if (col.TryGetComponent(out Hurtbox hurtbox))
                {
                    // Cache damage
                    IAction damaged = new Damaged(manager, hurtbox.TimeDamage);
                    actionsTaken.Push(damaged);
                }
            }
        }

        private void ActionFinishedAudio()
        {
            audioSource.Play();
        }

        #endregion
    }

    public enum PlayerState
    {
        Deciding,
        InAction,
        Failed
    }
}
