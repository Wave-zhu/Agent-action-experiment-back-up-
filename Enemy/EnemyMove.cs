using Game.Tool;
using Newtonsoft.Json.Bson;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Experiment.Move 
{
    public class EnemyMove : CharactorMoveBase
    {
        private bool _applyMovement;
        public Transform targetTransform;
        private GPTMove _wiseMove;


        private void LookTargetDirection()
        {
            transform.LookAt(targetTransform.position);
        }
        public void SetAnimatorMovementValue(float horizontal, float vertical)
        {
            if (_applyMovement)
            {
                _animator.SetBool(AnimationID.HasInputID, true);
                _animator.SetFloat(AnimationID.LockID, 1f);
                _animator.SetFloat(AnimationID.HorizontalID, horizontal, 0.2f, Time.deltaTime);
                _animator.SetFloat(AnimationID.VerticalID, vertical, 0.2f, Time.deltaTime);
            }
            else
            {
                _animator.SetFloat(AnimationID.LockID, 0f);
                _animator.SetFloat(AnimationID.HorizontalID, 0f, 0.2f, Time.deltaTime);
                _animator.SetFloat(AnimationID.VerticalID, 0f, 0.2f, Time.deltaTime);
            }

        }

        public void AnimationPlay(string clip)
        {
            _animator.Play(clip, 0, 0f);
        }

        private void ApplyGPT()
        {
            if (GameInputManager.MainInstance.AIStart)
            {
                _wiseMove.enabled = !_wiseMove.enabled; 
            }
        }
        //gizmo
        private void DrawDirection()
        {
            Debug.DrawRay(transform.position + transform.up * 0.7f,
                targetTransform.position - transform.position,
                Color.yellow);
        }

        #region movement
        public void SetApplyMovement(bool apply)
        {
            _applyMovement = apply;
        }
        public void EnableCharacterController(bool enable)
        {
            _characterController.enabled = enable;
        }
        #endregion
        

        protected override void Awake()
        {
            base.Awake();
            _wiseMove = GetComponent<GPTMove>();    
        }
        protected override void Start()
        {
            base.Start();
            SetApplyMovement(true);
        }
        protected override void Update()
        {
            base.Update();
            LookTargetDirection();
            DrawDirection();
            ApplyGPT();
        }

    }

}

