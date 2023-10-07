using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Spikes : MonoBehaviour, ISentient
    {
        [SerializeField] GameObject spikeParent;
        [SerializeField] bool alternating = true;
        bool spikesOut = true;
        

        public void StartTurn(GameTurnManager manager)
        {
            if (alternating)
            {
                spikesOut = !spikesOut;
                spikeParent.SetActive(spikesOut);
            }

            manager.EndTurn();
        }
    }
}
