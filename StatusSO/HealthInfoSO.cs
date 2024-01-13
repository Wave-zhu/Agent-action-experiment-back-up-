using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Experiment.Health
{
    [CreateAssetMenu(fileName = "HealthInfo", menuName = "Character/Health/New HealthInfo", order = 0)]
    public class HealthInfoSO : ScriptableObject
    {
        private float _currentHP;
        private float _maxHP;
        private bool _isDie => _currentHP <= 0;
        [SerializeField]
        private HealthDataSO _characterHealthBaseData;

        public float CurrentHP => _currentHP;
        public float MaxHP => _maxHP;
        public bool IsDie => _isDie;

        public void InitCharacterHealthInfo()
        {
            _maxHP = _characterHealthBaseData.MaxHP;
            _currentHP = _maxHP;
        }
        public void Damage(float damage)
        {
            _currentHP = Clamp(_currentHP, damage, 0, _maxHP, false);
        }
        public void RestoreHP(float hp)
        {
            _currentHP = Clamp(_currentHP, hp, 0, _maxHP, true);
        }
        private float Clamp(float value, float offset, float min, float max, bool add)
        {
            return Mathf.Clamp(add ? value + offset : value - offset, min, max);
        }
    }
}

