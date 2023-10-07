using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class GameTurnManager : MonoBehaviour
    {
        //Replace GameObject with an interface which contains method "void StartTurn(GameTurnManager manager)"
        [SerializeField] List<GameObject> sentientObjects;
        [SerializeField] int turnCount = 30;
        public int GlobalTurnCount { get { return turnCount; } private set { turnCount = value; } }
        int objectTurn = 0;

        private void Awake()
        {
            //Start the first object's turn
            //sentientObjects[objectTurn].StartTurn(this);
        }

        public void EndTurn()
        {
            objectTurn = (objectTurn+1)%sentientObjects.Count;
            //sentientObjects[objectTurn].StartTurn(this);
        }
    }
}
