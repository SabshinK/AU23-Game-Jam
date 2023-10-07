using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Move : IAction
    {
        private PlayerController controller;

        public Move(PlayerController controller)
        {

        }

        public void Performed()
        {
            //controller.PreviousSpace = controller.NextSpace;
            //controller.NextSpace = controller.PreviousSpace + controller.RawRotation * 2;
        }

        public void Update()
        {
        
        }

        public void Cancelled()
        {
            //controller.CurrentAction = null;
        }
    }
}
