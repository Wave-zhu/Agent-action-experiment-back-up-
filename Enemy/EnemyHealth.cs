using Experiment.Status;
using Game.Tool;
using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace Experiment.Health 
{
    public class EnemyHealth : CharactorHealthBase
    {
        private GameObject _healthInstance;
        private Image _virtualBlood;
        private Image _blood;
        private float _buffTime = 3f;
        private Coroutine _running;
        private CharactorStatusScaleBase _enemyStatus;
        private GPTMove _brain;
        

        public override void Damage(int damage)
        {
            base.Damage(damage);
            _brain.SetDamageReceived(damage);
            if (_running != null)
            {
                StopCoroutine(_running);
            }
            UpdateBlood();
            if (Mathf.FloorToInt(_currentHP) == 0) 
            {
                BehaviorManager.MainInstance.currentGame = BehaviorManager.GameProcess.PLAYER_WIN;
            }
            StartHpEffect();
        }
        public override void RestoreHP(float hp)
        {
            base.RestoreHP(hp);
            _brain.SetDamageDrain(Mathf.FloorToInt(hp));
        }
        protected void UpdateBlood() 
        {
            _virtualBlood.fillAmount = _blood.fillAmount;
            _blood.fillAmount = _currentHP / _maxHP;
        }
        protected void StartHpEffect()
        {
            _running = StartCoroutine(UpdateHpEffect()); 
        }
        private IEnumerator UpdateHpEffect()
        {
            float effectLength = _virtualBlood.fillAmount - _blood.fillAmount;

            float elapsedTime = 0f; 

            while (elapsedTime < _buffTime && effectLength != 0)
            {
                elapsedTime += Time.deltaTime;
                _virtualBlood.fillAmount = Mathf.Lerp(_blood.fillAmount + effectLength, _blood.fillAmount, elapsedTime / _buffTime); //classic "elapseTime/allTime"
                _enemyStatus.SetDrainable(true);
                yield return null;
            }
            _enemyStatus.SetDrainable(false);
            _virtualBlood.fillAmount = _blood.fillAmount; //end to be same
        } 
        
        public string EnemyHealthInfo()
        {
            return "Your current HP: " + _currentHP + ". Attack HP Drainable: " + _enemyStatus.GetDrainable()+".\n";
        }
        protected override void Awake()
        {
            base.Awake();
            _brain = GetComponent<GPTMove>();
            _enemyStatus = GetComponent<CharactorStatusScaleBase>();
        }
        protected override void Start()
        {
            base.Start();
            _healthInstance = Instantiate(_healthBar, transform.position + Vector3.up * 2f, Quaternion.identity);
            _healthInstance.transform.SetParent(transform);
            var temp= _healthInstance.GetComponent<HealthBar>();
            _virtualBlood = temp.virtualBlood;
            _blood= temp.blood;
        }
        protected override void Update()
        {
            base.Update();
            _healthInstance.transform.rotation = Quaternion.LookRotation(_healthInstance.transform.position - Camera.main.transform.position);
            UpdateBlood();  
        }

    }
}

