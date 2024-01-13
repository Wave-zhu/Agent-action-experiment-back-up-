using Cinemachine;
using Experiment.Combat;
using Game.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CharactorCombatBase
{
    private bool _isLocked = false;
    [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;
    [SerializeField] private GameObject _aimIcon;
    [SerializeField] private float _minDistance;// minimum distance to stop rotation if you get close to target
    [SerializeField] private Vector2 _targetLockOffset;
    [SerializeField] private Transform _followPoint;
    private GameObject _midpointObject;
    private Camera _camera;
    
    #region Lock
    protected void LockOn()
    {
        if (_isLocked)
        {
            _isLocked = false;
            _unit = null;
            _aimIcon.SetActive(false);
            _cinemachineFreeLook.LookAt = _followPoint;
            return;
        }
        GetUnits(out _unitsCount);
        if (_unitsCount == 0) return;
        if (_unit == null || _unit.transform != _units[0].transform )
        {
            _unit = _units[0].transform;
        }
        //_animator.SetFloat(AnimationID.LockID,1 - _animator.GetFloat(AnimationID.LockID));
        _cinemachineFreeLook.LookAt = _midpointObject.transform;
        _aimIcon.SetActive(true);
        _isLocked = true;
    }
    
    protected void LockUpdate()
    {
        if(GameInputManager.MainInstance.Lock || _isLocked && Vector3.Distance(_unit.transform.position,transform.position)>7f)
            LockOn();
        if (!_isLocked || !_unit) return;
        
        _midpointObject.transform.position = (_unit.transform.position + transform.position) / 2 + 0.7f * Vector3.up;
        if (_aimIcon)
            _aimIcon.transform.position = _camera.WorldToScreenPoint(_unit.position + Vector3.up * 0.7f);
    }
    #endregion
    
    protected void InFormat()
    {
        _animator.SetFloat(AnimationID.LockID,1 - _animator.GetFloat(AnimationID.LockID));
        if (_animator.GetFloat(AnimationID.LockID) > 0.5f) 
        {
            _charactorStatusScale.SetAttackScale("1.25,-1");
            _charactorStatusScale.SetDefenseScale("1.25,-1");
            _charactorStatusScale.SetSpeedScale("0.8,-1");
        } 
        else 
        {
            _charactorStatusScale.SetAttackScale("0.8,-1");
            _charactorStatusScale.SetDefenseScale("0.8,-1");
            _charactorStatusScale.SetSpeedScale("1.25,-1");
        }
    }
    protected override void UpdateInfo(int deal ,int drain)
    {
        if(deal > 0)
            BehaviorManager.MainInstance.DamageDebug(-deal);
        if(drain > 0)
            BehaviorManager.MainInstance.DamageDebug(drain);
    }
    protected override void UpdateComboInfo(bool value)
    {
        BehaviorManager.MainInstance.currentInput.Append(value ? "L" : "R");
    }

    private void PlayerEvadeInput()
    {
        if (_canAttackInput && GameInputManager.MainInstance.Evade) 
        {
            _canAttackInput = false;
            var temp = GameInputManager.MainInstance.Movement;
            
            if (Math.Abs(temp.y) > 0.5f || Math.Abs(temp.x)>0.5f) 
            {
                _animator.CrossFade("EvadeF", 0.025f, 0, 0f);
            } 
            else
            {    
                _animator.CrossFade("EvadeB", 0.025f, 0, 0f);
            }
            
            TimerManager.MainInstance.TryGetOneTimer(1f, () => {
                _canAttackInput = true;
            });
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }
    private void Start()
    {
        _aimIcon.SetActive(false);
        _midpointObject = new GameObject("MidpointObject");
    }
    protected override void Update()
    {
        if (GameInputManager.MainInstance.Format) 
        {
            InFormat();
        }
        LockUpdate();
        PlayerEvadeInput();
        base.Update();
    }
}
