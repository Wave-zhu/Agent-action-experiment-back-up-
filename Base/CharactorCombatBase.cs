using Game.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Experiment.Health;
using Experiment.Status;

namespace Experiment.Combat
{
    public abstract class CharactorCombatBase : MonoBehaviour
    {
        protected Animator _animator;
        protected Transform _unit;
        protected bool _isJumping;
        protected float _jumpHeight;
        protected CharactorStatusScaleBase _charactorStatusScale;
        protected CharactorHealthBase _charactorHealth;
       
        

        [SerializeField,Header("AttackInfo")] private AttackBaseSO _lAttackSO;
        [SerializeField] private AttackBaseSO _rAttackSO;
        [SerializeField] private AttackBaseSO _jumpLAttackSO;
        [SerializeField] private AttackBaseSO _jumpRAttackSO;
        

        protected AttackBaseSO _attackSO;
        protected bool _isHeavyAttack;

        [SerializeField, Header("AttackDetection")] private float _attackRadius;
        [SerializeField] private LayerMask _unitLayer;
        private float _matchRange = 0;

        [SerializeField] protected float _attackColdDuration;
        protected bool _canAttackInput;

        private bool _hasPreviousAttack=false;

        protected Collider[] _units;
        protected int _unitsCount;
        
        protected virtual bool LInput()
        {
            return GameInputManager.MainInstance.LAttack;
        }
        
        protected virtual bool RInput()
        {
            return GameInputManager.MainInstance.RAttack;
        }
        
        
        #region modify location

        protected void MatchPosition()
        {
            if (_unit == null) return;
            if (_animator == null) return;

            if (_animator.AnimationAtTag("Attack"))
            {
                MatchingProcess(_attackSO, 0, 0.2f);
            }
        }
        protected void MatchingProcess(AttackBaseSO currentAttack, float startTime = 0f, float endTime = 0.01f)
        {
            if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))
            {
                var timer = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (timer > 0.35f) return;
                if (Vector3.Dot(DevelopmentToos.DirectionForTarget(transform, _unit.transform), transform.forward) < .7f) return;
                if (DevelopmentToos.DistanceForTarget(_unit, transform) > _matchRange) return;
                transform.rotation = Quaternion.LookRotation(-_unit.forward);
                _animator.MatchTarget(_unit.position +
                    (-transform.forward) * currentAttack.AttackOffset,
                    Quaternion.identity, AvatarTarget.Body, new MatchTargetWeightMask(Vector3.one, 0f),
                   startTime, endTime);
            }
        }
        protected void LookAtTargetOnAttack()
        {
            if (_unit == null) return;
            if (DevelopmentToos.DistanceForTarget(transform, _unit) > 3f) return;
            if (_animator.AnimationAtTag("Attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                transform.Look(_unit.position, 50f);
            }
        }

        #endregion

        #region Damage/Debuff trigger
        protected Collider[] GetUnits(out int number)
        {
            number=Physics.OverlapSphereNonAlloc(
                transform.position + transform.up * 0.7f, 
                _attackRadius, 
                _units, 
                _unitLayer, 
                QueryTriggerInteraction.Ignore
            );
            return _units;
        }
        private void TriggerDamage(int index)
        {
            GetUnits(out _unitsCount);
            if(_unitsCount == 0) return;
            if (_unit == null || _unit.transform != _units[0].transform)
            {
                _unit = _units[0].transform;
            }
            index = Mathf.Min(index, _attackSO.AttackDamageInfo.Count - 1);
            
            for(int i = 0; i<_unitsCount; i++) {
                var unit = _units[i];
                if (Vector3.Dot(DevelopmentToos.DirectionForTarget(transform, unit.transform), transform.forward) < .7f) continue;
                if (DevelopmentToos.DistanceForTarget(transform,unit.transform)>_matchRange) continue;
                if (unit.TryGetComponent(out IDamage damage)) {
                    var dealDamage = Mathf.FloorToInt(_attackSO.AttackDamageInfo[index].damage * _charactorStatusScale.GetAttackStatus());
                    
                    dealDamage = damage.TakeDamage(
                        _attackSO.AttackDamageInfo[index].hitName,
                        _attackSO.AttackDamageInfo[index].parryName,
                        dealDamage,
                        _attackSO.AttackDamageInfo[index].damageType, transform);
                    
                    //TODO multiplier value test:
                    var drainRecover = _charactorStatusScale.GetDrainable() ? Mathf.FloorToInt(dealDamage * 0.3f) : 0;
                    _charactorHealth.RestoreHP(drainRecover);
                    UpdateInfo(dealDamage,drainRecover);
                }
            }
        }
        protected virtual void TriggerDebuff(string info)
        {
            GetUnits(out _unitsCount);
            if(_unitsCount == 0) return;
            if (_unit == null || _unit.transform != _units[0].transform)
            {
                _unit = _units[0].transform;
            }
            for(int i = 0; i<_unitsCount; i++) {
                var unit = _units[i];
                if (Vector3.Dot(DevelopmentToos.DirectionForTarget(transform, unit.transform), transform.forward) < .7f) continue;
                if (DevelopmentToos.DistanceForTarget(transform,unit.transform)>_matchRange) continue;
                if (unit.TryGetComponent(out CharactorStatusScaleBase charactorStatusScaleBase)) {
                   charactorStatusScaleBase.SetDefenseScale(info);
                }
            }
        }
        protected virtual void UpdateInfo(int deal, int drain)
        {
            
        }
        protected virtual void UpdateComboInfo(bool value)
        {

        }
        protected virtual void AchievedInfo(string describe)
        {
            
        }

        #endregion

        #region Normal Attack
        private void AttackExecution()
        {
            _matchRange = _attackSO.AttackRange;
            _attackColdDuration = _attackSO.AttackColdDuration;
            if (_isJumping)
            {
                _jumpHeight = transform.position.y;
            }
            _animator.CrossFade(_attackSO.AttackName, 0.025f, 0, 0f);
            TimerManager.MainInstance.TryGetOneTimer(_attackColdDuration, 
                delegate () { _canAttackInput = true; });
            _canAttackInput = false;
        }
        private void SetAttack(AttackBaseSO attackData)
        {
            if (attackData != null)
            {
                _attackSO = attackData;
            }
        }
        
        private void PlayerCombatInput()
        {
            if (_canAttackInput)
            {
                if (LInput())
                {
                    if (_hasPreviousAttack)
                    {
                        if (!_isHeavyAttack)
                        {
                            if (_attackSO.HasNextAttackInfo()) 
                            {
                                UpdateComboInfo(true);
                                SetAttack(_attackSO.NextAttackInfo);
                                AttackExecution();
                            }
                            else
                            {
                                ResetAttack();
                                UpdateComboInfo(true);
                                _isHeavyAttack = false;
                                _attackSO = _lAttackSO;
                                _hasPreviousAttack = true;
                                AttackExecution();
                            }
                            
                        }
                        else if (_attackSO.HasChildAttackInfo())
                        {
                            UpdateComboInfo(true);
                            SetAttack(_attackSO.ChildAttackInfo);
                            AttackExecution();                            
                        }
                        else
                        {
                            ResetAttack();
                            UpdateComboInfo(true);
                            _isHeavyAttack = false;
                            _attackSO = _lAttackSO;
                            _hasPreviousAttack = true;
                            AttackExecution();
                        }
                    }
                    else
                    {
                        UpdateComboInfo(true);
                        _isHeavyAttack = false;
                        _attackSO = _isJumping ? _jumpLAttackSO : _lAttackSO;
                        _hasPreviousAttack = true;
                        AttackExecution();
                    }
                }
                else if (RInput())
                {
                    if (_hasPreviousAttack) 
                    {
                        if (_isHeavyAttack)
                        {
                            if (_attackSO.HasNextAttackInfo())
                            {
                                UpdateComboInfo(false);
                                SetAttack(_attackSO.NextAttackInfo);
                                AttackExecution();
                            }
                            else
                            {
                                ResetAttack();
                                UpdateComboInfo(false);
                                _isHeavyAttack = true;
                                _attackSO = _rAttackSO;
                                _hasPreviousAttack = true;
                                AttackExecution();
                            }
                        }
                        else if (_attackSO.HasChildAttackInfo())
                        {
                            UpdateComboInfo(false);
                            SetAttack(_attackSO.ChildAttackInfo);
                            AttackExecution();
                        }
                        else 
                        { 
                            ResetAttack();
                            UpdateComboInfo(false);
                            _isHeavyAttack = true;
                            _attackSO = _rAttackSO;
                            _hasPreviousAttack = true;
                            AttackExecution();
                        }
                    }
                    else
                    {
                        UpdateComboInfo(false);
                        _isHeavyAttack = true;
                        _attackSO = _isJumping ? _jumpRAttackSO : _rAttackSO;
                        _hasPreviousAttack = true;
                        AttackExecution();
                    }
                }
            }
        }

        public void ResetAttack()
        {
            BehaviorManager.MainInstance.ClearAll();
            BehaviorManager.MainInstance.currentInput.Clear();
            _isHeavyAttack = false;
            _hasPreviousAttack = false;
            _isJumping = false;
        }
        #endregion


        #region notify

        public void WhenAttack(int index)
        {
            TriggerDamage(index);
        }
        public void AttackEffect(int idx)
        {
            switch (idx) 
            {
                case 0:
                    _charactorStatusScale.SetSpeedScale("1.2,5");
                    break;
                case 1:
                    _charactorHealth.RestoreHP(200);
                    break;
                case 2:
                    TriggerDebuff("0.7,10");
                    break;
                case 3:
                    _charactorStatusScale.SetAttackScale("2,5");
                    break;
                case 4:
                    _charactorStatusScale.SetAttackScale("1.5,5");
                    _charactorStatusScale.SetDefenseScale("1.5,5");
                    break;
            }
        }
        #endregion
        

        #region Jump Attack
        public void AbleJumpAttack()
        {
            _isJumping=true;
        }
        private void UpdateJump()
        {
            transform.position = new Vector3(transform.position.x, _jumpHeight, transform.position.z);
        }

        #endregion
        protected virtual void Awake()
        {
            _charactorStatusScale = GetComponent<CharactorStatusScaleBase>();
            _charactorHealth = GetComponent<CharactorHealthBase>();
            _animator = GetComponent<Animator>();
            _units = new Collider[5];
            _canAttackInput = true;
        }
        protected virtual void OnEnable()
        {
           
        }
        protected virtual void OnDisable()
        {

        }
        protected virtual void Update()
        {
            if (_isJumping)
            {
                UpdateJump();
            }
            MatchPosition();
            LookAtTargetOnAttack();
            PlayerCombatInput();
        }
        
    }
        
}
