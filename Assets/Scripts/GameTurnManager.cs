using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class GameTurnManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> sentientObjects;
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