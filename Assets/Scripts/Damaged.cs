using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Damaged : IAction
    {
        private GameTurnManager manager;
        private int damageTaken;

        public Damaged(GameTurnManager manager, int damageTaken)
        {
            this.manager = manager;
            this.damageTaken = damageTaken;

            manager.TurnCount -= damageTaken;
        }

        public void Undo()
        {
            manager.TurnCount += damageTaken;
        }
    }
}
