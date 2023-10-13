using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public interface ISentient
    {
        /*Nothing turn-related should happen outside of the start turn method*/
        public void StartTurn(GameTurnManager manager);
    }
}
