using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class DoorReceiver : MonoBehaviour, IReceiver
    {
        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void Receive()
        {
            anim.SetTrigger("Open");
        }
    }
}
