using Experiment.Combat;
using Game.Tool;
using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : CharactorCombatBase
{
    [SerializeField] protected Transform _targetTransform;
    protected GPTMove _brain;
    protected StringBuilder _achievedInfo = new StringBuilder();
    private bool _lock;
    private float _time;
    

    #region Input
    protected float _lAttack;
    protected float _rAttack;
    protected float _evade;
    protected bool _parry;
    protected override bool LInput()
    {
        return _lAttack > 0;
    }
    protected override bool RInput()
    {
        return _rAttack > 0;
    }
    protected bool EvadeInput()
    {
        return _evade > 0;
    }
    public void SetLAttack()
    {
        _lAttack = 1f;
        _parry = false;
    }
    public void SetRAttack()
    {
        _rAttack = 1f;
        _parry = false;
    }
    public void SetEvade()
    {
        _evade = 1f;
        _parry = false;
    }
    private void UpdateAIInput(float deltaTime)
    {
        if (LInput()) 
        {
            _lAttack -= deltaTime;
        } 
        else 
        {
            _lAttack = 0f;
        }
        
        if (RInput()) 
        {
            _rAttack -= deltaTime;
        }
        else 
        {
            _rAttack = 0f;
        }
        if (EvadeInput()) 
        {
            _evade -= deltaTime;
        }
        else
        {
            _evade = 0f;
        }
    }
    public void SetParry()
    {
        _parry = true;
    }
    protected bool ParryInput()
    {
        return _parry;
    } 
    #endregion
    protected void EnemyParry()
    {
        if (_animator.AnimationAtTag("Hit") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.25f) return;
        _animator.SetBool(AnimationID.ParryID,ParryInput());
    }
    private void EnemyEvade()
    {
        if (_canAttackInput && EvadeInput()) {
            _canAttackInput = false;
            if (_animator.GetFloat(AnimationID.VerticalID) > 0.2f) 
            {
                _animator.CrossFade("EvadeF", 0.025f, 0, 0f);
            }
            if (_animator.GetFloat(AnimationID.VerticalID) < -0.2f) 
            {
                _animator.CrossFade("EvadeB", 0.025f, 0, 0f);
            }
            else if (_animator.GetFloat(AnimationID.HorizontalID) > 0) 
            {
                _animator.CrossFade("EvadeR", 0.025f, 0, 0f);
            } 
            else 
            {
                _animator.CrossFade("EvadeL", 0.025f, 0, 0f);
            }
            TimerManager.MainInstance.TryGetOneTimer(1f, () => {
                _canAttackInput = true;
            });
        }
    }
    public string CurrentPositionInfo()
    {
        return "The target position: " + _targetTransform.position.ToString() + ".\n" +
               "Your position: " + transform.position.ToString() + ".\n";
        //"Distance is: " + Vector3.Distance(_targetTransform.position, transform.position);
    }
    public string ReportAchievedInfo()
    {
        var temp = _achievedInfo.ToString();
        _lock = true;
        _achievedInfo.Clear();
        _lock = false;
        return temp;
    }
    protected override void UpdateInfo(int deal, int drain)
    {
       _brain.SetDamageDeal(deal);
       _brain.SetDamageDrain(drain);
    }
    protected override void AchievedInfo(string describe)
    {
        while(_lock){}
        _achievedInfo.Append("Skill achieved, effect: "+describe+ "\n");
    }
    protected override void UpdateComboInfo(bool value)
    {
        //_brain.AddCombo(value);
    }
    protected override void Awake()
    {
        base.Awake();
        _brain = GetComponent<GPTMove>();
    }
    protected override void Update()
    {
        UpdateAIInput(Time.deltaTime);
        EnemyParry();
        EnemyEvade();
        base.Update();
    }
    
}
