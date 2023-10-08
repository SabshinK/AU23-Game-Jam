using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

namespace King
{
    public class KeyCollectable : MonoBehaviour
    {
        private LayerMask playerMask;

        private Animator anim;

        private void Awake()
        {
            playerMask = LayerMask.GetMask("Player");

            anim = GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((playerMask & (1 << other.gameObject.layer)) != 0)
            {
                var player = other.GetComponent<PlayerController>();
                //player.HasKey = true;

                anim.SetTrigger("Collect");

                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
