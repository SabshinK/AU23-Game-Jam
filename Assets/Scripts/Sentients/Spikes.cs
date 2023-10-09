using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Spikes : MonoBehaviour, ISentient
    {
        [SerializeField] GameObject spikeParent;

        //Window of time for the spikes to be enabled.
        [SerializeField] int period = 2;
        //The specific turn number inside the period which the spikes are enabled.
        [SerializeField] int offset = 0;

        bool spikesOut;

        private void Start()
        {
            spikesOut = offset == 0;
            spikeParent.SetActive(spikesOut);
        }

        public IEnumerator StartTurn(GameTurnManager manager)
        {
            if (manager.turnCount % period == offset)
            {
                spikesOut = true;
                spikeParent.SetActive(spikesOut);
            }
            else if (spikesOut)
            {
                spikesOut = false;
                spikeParent.SetActive(spikesOut);
            }

            yield return null;

            manager.EndTurn();
        }
    }
}
