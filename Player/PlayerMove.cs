using Game.Tool;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Experiment.Move 
{
    public class PlayerMove : CharactorMoveBase
    {
        private bool _canMoveInput;
        private float _rotationAngle;
        private float _angleVelocity = 0f;
        [SerializeField] private float _rotationSmoothTime;
        private Transform _mainCamera;
        
        public void ResetPlayer()
        {
            _animator.Play("Idle");
            _animator.SetFloat(AnimationID.MovementID, 0f);
            _animator.SetBool(AnimationID.HasInputID, false);
            enabled = false;
        }
        private void CharacterRotationControl()
        {
            if (!_characterIsOnGround) return;
            if (_animator.GetBool(AnimationID.HasInputID))
            {
                _rotationAngle = Mathf.Atan2(GameInputManager.MainInstance.Movement.x, GameInputManager.MainInstance.Movement.y)
               * Mathf.Rad2Deg+ _mainCamera.eulerAngles.y;

            }
            if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))
            {
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle,
                    ref _angleVelocity, _rotationSmoothTime);
            }
        }

        private void UpdateAnimation()
        {
            if (!_characterIsOnGround) return;
            _animator.SetBool(AnimationID.JumpID, GameInputManager.MainInstance.Jump);
            _animator.SetBool(AnimationID.HasInputID, GameInputManager.MainInstance.Movement != Vector2.zero);
            if (_animator.GetBool(AnimationID.HasInputID))
            {
                if(GameInputManager.MainInstance.Run)
                    _animator.SetBool(AnimationID.RunID, true);
                
                if (_animator.GetFloat(AnimationID.LockID) < 0.5f) 
                {
                    _animator.SetFloat(AnimationID.MovementID, _animator.GetBool(AnimationID.RunID) ?
                                           2f : GameInputManager.MainInstance.Movement.sqrMagnitude, 0.25f, Time.deltaTime);
                } 
                else 
                {
                    _animator.SetFloat(AnimationID.HorizontalID, (_animator.GetBool(AnimationID.RunID) ?
                                           1.414f : 1f )* GameInputManager.MainInstance.Movement.x, 0.25f, Time.deltaTime);
                    _animator.SetFloat(AnimationID.VerticalID, (_animator.GetBool(AnimationID.RunID) ?
                                           1.414f : 1f )* GameInputManager.MainInstance.Movement.y, 0.25f, Time.deltaTime);
                }
            }
            else
            {
                if (_animator.GetFloat(AnimationID.LockID) < 0.5f) 
                    _animator.SetFloat(AnimationID.MovementID, 0f, 0.25f, Time.deltaTime);
                else 
                {
                    _animator.SetFloat(AnimationID.HorizontalID,0f,0.25f,Time.deltaTime);
                    _animator.SetFloat(AnimationID.VerticalID,0f,0.25f,Time.deltaTime);
                }
                Vector2 temp = new Vector2(_animator.GetFloat(AnimationID.VerticalID), _animator.GetFloat(AnimationID.HorizontalID));
                //set false when almost stop running
                if (temp.SqrMagnitude()<0.2f || _animator.GetFloat(AnimationID.MovementID) < 0.2f)
                {
                    _animator.SetBool(AnimationID.RunID, false);
                }
            }
        }
        
        public string PlayerStateInfo()
        {
            if (_animator.AnimationAtTag("Hit"))
                return "Player is being hit \n";
            else if(_animator.AnimationAtTag("Attack"))
                return "Player is attacking \n";
            else if(_animator.AnimationAtTag("Motion"))
                return "Player in locomotion \n";
            else if (_animator.AnimationAtTag("Parry"))
                return "Player is parrying \n";
            else if (_animator.AnimationAtTag("Idle"))
                return "Player is Idle. \n";
            else
                return "";
        }
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
        }
        private void LateUpdate()
        {
            UpdateAnimation();
            CharacterRotationControl();
        }
    }
}

