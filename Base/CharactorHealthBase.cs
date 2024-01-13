using Game.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Experiment.Health
{
    public interface IDamage 
    {
        int TakeDamage(string hitName, string parryName, int damage, DamageType damageType, Transform attacker);
    }
    public interface IHealth
    {
        void IsDead();
    }

    public abstract class CharactorHealthBase : MonoBehaviour, IDamage, IHealth
    {
        protected Animator _animator;
        protected Transform _currentAttacker;
        protected bool _immune;

        #region HealthInfo
        protected float _currentHP;
        protected float _maxHP;
        [SerializeField]protected GameObject _healthBar;
        protected bool _isDie => _currentHP <= 0;
        [SerializeField]
        protected HealthDataSO _characterHealthBaseData;

        public void InitCharacterHealthInfo()
        {
            _maxHP = _characterHealthBaseData.MaxHP;
            _currentHP = _maxHP;
        }
        public virtual void Damage(int damage)
        {
            _currentHP = Clamp(_currentHP, damage, 0, _maxHP, false);
        }
        public virtual void RestoreHP(float hp)
        {
            _currentHP = Clamp(_currentHP, hp, 0, _maxHP, true);
        }
        private float Clamp(float value, float offset, float min, float max, bool add)
        {
            return Mathf.Clamp(add ? value + offset : value - offset, min, max);
        }
        #endregion


        #region Immune

        public void WhenEvade(float second)
        {
            _immune =true;
            TimerManager.MainInstance.TryGetOneTimer(second, () => {
                _immune = false;
            });
        }

        #endregion
        public void IsDead()
        {
            throw new System.NotImplementedException();
        }

        public int TakeDamage(string hitName, string parryName, int damage, DamageType damageType, Transform attacker)
        {
            SetAttacker(attacker);
            if (_immune)
                return 0;
            if (_animator.GetBool(AnimationID.ParryID))
            {
                _animator.Play(parryName, 0, 0f);
                var dealDamage = Mathf.FloorToInt(damage * 0.3f);
                Damage(dealDamage);
                return dealDamage;
            }
            else
            {
                _animator.Play(hitName, 0, 0f);
                Damage(damage);
                return damage;
            }
        }
        private void SetAttacker(Transform attacker)
        {
            if (_currentAttacker == null || _currentAttacker != attacker)
            {
                _currentAttacker = attacker;
            }
        }
        private void OnHitLookAtAttacker()
        {
            if (_currentAttacker == null) return;
            if ((_animator.AnimationAtTag("Hit") || _animator.AnimationAtTag("Parry")) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                transform.Look(_currentAttacker.position, 50f);
            }
        }

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        protected virtual void Start()
        {
            InitCharacterHealthInfo();
        }
        protected virtual void Update()
        {
            OnHitLookAtAttacker();
        }
    }
}

