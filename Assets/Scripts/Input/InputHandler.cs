using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using King.Input;

namespace King
{
    public static class InputHandler
    {
        public static Inputs Inputs { get; private set; }

        static InputHandler()
        {
            Inputs = new Inputs();
        }

        public static InputAction GetAction(string name)
        {
            return Inputs.FindAction(name);
        }

        public static void SetMapActive(bool active)
        {
            if (active) 
                Inputs.Player.Enable();
            else 
                Inputs.Player.Disable();
        }
    }
}
