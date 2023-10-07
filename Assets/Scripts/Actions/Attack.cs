using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Attack : IAction
    {
        private PlayerController controller;

        public Attack(PlayerController controller)
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
