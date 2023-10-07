using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Interact : IAction
    {
        private PlayerController controller;

        public Interact(PlayerController controller)
        {

        }

        public void Performed()
        {

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
