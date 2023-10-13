using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Spikes : MonoBehaviour, ISentient
    {
        [SerializeField] private GameObject spikeParent;

        //Window of time for the spikes to be enabled.
        [SerializeField] private int period = 2;
        //The specific turn number inside the period which the spikes are enabled.
        [SerializeField] private int offset = 0;

        [SerializeField] private AudioClip outClip;
        [SerializeField] private AudioClip retractedClip;
        private AudioSource audioSource;

        private bool spikesOut;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            spikesOut = offset == 0;
            spikeParent.SetActive(spikesOut);
        }

        public void StartTurn(GameTurnManager manager)
        {
            if (manager.TurnCount % period == offset)
            {
                spikesOut = true;
                spikeParent.SetActive(spikesOut);

                audioSource.PlayOneShot(outClip, 1.0f);
            }
            else if (spikesOut)
            {
                spikesOut = false;
                spikeParent.SetActive(spikesOut);

                audioSource.PlayOneShot(retractedClip, 1.0f);
            }

            manager.EndTurn();
        }
    }
}
