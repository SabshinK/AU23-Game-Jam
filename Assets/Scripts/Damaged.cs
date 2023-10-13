using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Damaged : IAction
    {
        private PlayerController player;
        private GameTurnManager manager;

        public Damaged(PlayerController player, GameTurnManager manager)
        {
            this.player = player;
            this.manager = manager;
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}
