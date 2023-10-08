using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class GameTurnManager : MonoBehaviour
    {
        //Replace GameObject with an interface which contains method "void StartTurn(GameTurnManager manager)"
        List<GameObject> sentientObjects;
        [SerializeField] int turnCount = 30;
        public int GlobalTurnCount { get { return turnCount; } private set { turnCount = value; } }
        int objectTurn = 0;

        private void Start()
        {
            sentientObjects = FindObjectsByType<ISentient>();
            //Start the first object's turn
            StartCoroutine(sentientObjects[objectTurn].GetComponent<ISentient>().StartTurn(this));
        }

        public void EndTurn()
        {
            objectTurn = (objectTurn+1)%sentientObjects.Count;
            if (objectTurn == 0) turnCount--;
            StartCoroutine(sentientObjects[objectTurn].GetComponent<ISentient>().StartTurn(this));
        }
    }
}
