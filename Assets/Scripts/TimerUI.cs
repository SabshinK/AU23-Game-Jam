using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace King
{
    public class TimerUI : MonoBehaviour
    {
        private GameTurnManager manager;
        private TMP_Text text;

        private void Awake()
        {
            manager = FindObjectOfType<GameTurnManager>();
            text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            text.text = new TimeSpan(0, 0, manager.turnCount).ToString();
        }
    }
}
