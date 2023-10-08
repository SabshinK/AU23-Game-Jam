using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace King
{
    public class GameTurnManager : MonoBehaviour
    {
        public int turnCount = 30;

        //Replace GameObject with an interface which contains method "void StartTurn(GameTurnManager manager)"
        [SerializeField] List<GameObject> sentientObjects;
        private List<ISentient> npcSentients;
        private ISentient player;

        public int GlobalTurnCount 
        {
            get { return turnCount; } 
            private set { turnCount = value; }
        }

        int objectTurn = 0;

        private void Awake()
        {
            //// Gather all of the sentients and put them in a list
            //ISentient[] sentients = (ISentient[])FindObjectsOfType(typeof(ISentient));
            //npcSentients = sentients.ToList();

            //// Remove the player from the list
            //for (int i = 0; i < npcSentients.Count; i++)
            //{
            //    if (npcSentients[i] is PlayerController)
            //    {
            //        player = npcSentients[i];
            //        npcSentients.RemoveAt(i);
            //        break;
            //    }
            //}
        }

        private void Start()
        {
            //Start the first object's turn
            StartCoroutine(sentientObjects[objectTurn].GetComponent<ISentient>().StartTurn(this));
        }

        public void EndTurn()
        {
            objectTurn = (objectTurn + 1) % sentientObjects.Count;
            if (objectTurn == 0) turnCount--;
            StartCoroutine(sentientObjects[objectTurn].GetComponent<ISentient>().StartTurn(this));
        }

        //private IEnumerator TurnOrder()
        //{
        //    int turnsleft = turnCount;

        //    while (turnsleft > 0)
        //    {
        //        // Player's turn

        //        // Traps do their thing
        //    }
        //}
    }
}
