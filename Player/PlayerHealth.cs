using Experiment.Health;
using Experiment.Status;
using Game.Tool;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : CharactorHealthBase
{
    private Image _virtualBlood;
    private Image _blood;
    private float _buffTime = 2f;
    private Coroutine _running;
    private CharactorStatusScaleBase _playerStatus;
    public override void Damage(int damage)
    {
        base.Damage(damage);
        if (_running != null)
        {
            StopCoroutine(_running);
        }
        UpdateBlood();
        if (Mathf.FloorToInt(_currentHP) == 0) 
        {
            BehaviorManager.MainInstance.currentGame = BehaviorManager.GameProcess.YOU_WIN;
        }
        StartHpEffect();
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
            _playerStatus.SetDrainable(true);
            yield return null;
        }
        _playerStatus.SetDrainable(false);
        _virtualBlood.fillAmount = _blood.fillAmount; //end to be same
    }
    
    
    protected void RecoverHP()
    {
        RestoreHP((_playerStatus.GetRecoverStatus()-1f) );
    }

    
    private void PlayerParryInput()
    { 
        if (_animator.AnimationAtTag("Hit") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.25f) return;
        _animator.SetBool(AnimationID.ParryID,GameInputManager.MainInstance.Parry);
    }
    public string PlayerHealthInfo()
    {
        return "Player current HP: " + _currentHP + ". Current recover multiplier: " + _playerStatus.GetRecoverStatus()+".\n";
    }
    protected override void Awake()
    {
        base.Awake();
        _playerStatus = GetComponent<CharactorStatusScaleBase>();
    }
    protected override void Start()
    {
        base.Start();
        var temp= _healthBar.GetComponent<HealthBar>();
        _virtualBlood = temp.virtualBlood;
        _blood= temp.blood;
    }
    protected override void Update()
    {
        base.Update();
        PlayerParryInput();
        RecoverHP();
        UpdateBlood();
    }
}
