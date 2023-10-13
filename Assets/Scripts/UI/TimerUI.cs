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
        private Animator anim;

        private void Awake()
        {
            manager = FindObjectOfType<GameTurnManager>();
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            UpdateUI(manager.TurnCount);
        }

        private void OnEnable()
        {
            manager.onUpdateTurn += UpdateUI;
        }

        private void OnDisable()
        {
            manager.onUpdateTurn -= UpdateUI;
        }

        private void UpdateUI(int turn)
        {
            timeLeft.text = $"time_left: {new TimeSpan(0, 0, turn)}";
            anim.SetInteger("TimeLeft", turn);
            movesUsed.text = $"moves_used: {50 - turn}";
        }
    }
}
