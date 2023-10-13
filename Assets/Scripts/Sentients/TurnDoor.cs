using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class TurnDoor : MonoBehaviour, ISentient
    {
        [SerializeField] GameObject doorCollider;
        [SerializeField] int minTurnCount = 0;

        private void Awake()
        {
            doorCollider.SetActive(minTurnCount==0);
        }

        public void StartTurn(GameTurnManager manager)
        {
            doorCollider.SetActive(minTurnCount >= manager.TurnCount);
            manager.EndTurn();
        }
    }
}
