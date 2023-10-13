using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Move : IAction
    {
        private PlayerController player;

        // We need to store the previous space before moving in case the player undoes a move
        Vector3 previousSpace;

        public Move(PlayerController player)
        {
            this.player = player;
            previousSpace = player.NextSpace;
        }

        public void Perform()
        {
            previousSpace = player.NextSpace;
        }

        public void Undo()
        {
            player.NextSpace = previousSpace;
        }
    }
}
