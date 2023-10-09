using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King
{
    public class Hurtbox : MonoBehaviour
    {
        [SerializeField] private int timeDamage = 1;
        public int TimeDamage => timeDamage;
    }
}
