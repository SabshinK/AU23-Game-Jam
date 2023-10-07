using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace King
{
    public interface IAction
    {
        void PerformAction();

        void Update();
    }
}
