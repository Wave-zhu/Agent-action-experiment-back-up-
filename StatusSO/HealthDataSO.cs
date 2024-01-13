using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experiment.Health 
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Character/Health/New HealthData", order = 0)]
    public class HealthDataSO : ScriptableObject
    {
        [SerializeField] private float _maxHP;
        public float MaxHP => _maxHP;
    }
}


