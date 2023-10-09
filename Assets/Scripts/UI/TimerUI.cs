using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace King
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeLeft;
        [SerializeField] private TMP_Text movesUsed;

        private GameTurnManager manager;

        private void Awake()
        {
            manager = FindObjectOfType<GameTurnManager>();
        }

        private void Update()
        {
            timeLeft.text = $"time_left: {new TimeSpan(0, 0, manager.turnCount).ToString()}";
            movesUsed.text = $"moves_used: {50 - manager.turnCount}";
        }
    }
}
