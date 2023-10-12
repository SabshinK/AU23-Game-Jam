using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private GameObject receiverObj;
        private IReceiver receiver;

        [SerializeField] private Material activatedMaterial;

        private PlayerController player;

        private BoxCollider col;
        private Renderer rend;
        private ParticleSystem ps;
        private AudioSource audioSource;

        private void Awake()
        {
            receiver = receiverObj.GetComponent<IReceiver>();

            col = GetComponent<BoxCollider>();
            rend = GetComponent<Renderer>();
            ps = GetComponentInChildren<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // GetComponent is kinda inefficient but I don't feel like writing good code rn
                player = other.GetComponent<PlayerController>();
                player.onActionFinished += Activate;
            }
        }

        private void Activate(bool fromMove)
        {
            // The receiver should do stuff
            receiver.Receive();

            // Disable trigger callbacks
            col.enabled = false;

            // Visuals
            rend.material = activatedMaterial;
            ps.Play();

            // Audio
            audioSource.Play();

            /* 
             * This is only really used in one case but after a button is pressed, 
             * if there is a door in front of the button, check move again so that
             * the player doesn't have to look towards the door again.
             */
            player.CheckCanMove();

            // Unsubscribe this button so that it won't activate more than once
            player.onActionFinished -= Activate;
        }
    }
}
