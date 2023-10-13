using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace King
{
    public class GameTurnManager : MonoBehaviour
    {
        public delegate void OnUpdateTurn(int turn);
        public event OnUpdateTurn onUpdateTurn;

        [SerializeField] private int turnCount = 30;
        public int TurnCount
        {
            get { return turnCount; }
            set
            {
                turnCount = value;
                onUpdateTurn?.Invoke(turnCount);
            }
        }

        private ISentient[] sentients;

        int objectTurn = 0;

        private void Awake()
        {
            // Gather all of the sentients and put them in a list
            MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();

            /* 
             * How this query works: we declare sentients as the things we want and we will be searching the array of monoBehaviours.
             * We use the method GetType() to extract the type from our objects, then GetInterfaces() provides arrays of all the interfaces
             * being implemented for each object, and finally we use Any() to check if the object implements the interface we're looking
             * for. After all these checks sentients should be filled with the MonoBehaviours that implement T
             */
            sentients = (from sentients in monoBehaviours where sentients.GetType().GetInterfaces().Any(k => k == typeof(ISentient)) select (ISentient)sentients).ToArray();
        }

        private void Start()
        {
            //Start the first object's turn
            sentients[objectTurn].StartTurn(this);
        }

        public void EndTurn()
        {
            objectTurn = (objectTurn + 1) % sentients.Length;
            //if (objectTurn == 0) turnCount--;
            sentients[objectTurn].StartTurn(this);
        }
    }
}
