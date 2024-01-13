using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner;
using BehaviorDesigner.Runtime.Tasks;
using Experiment.Move;
using Game.Tool;

public class AIFreeMoveAction : AIMoveBase
{
    //move set
    private int _lastActionIndex;
    private int _actionIndex;
    private float _actionTimer;
    public override TaskStatus OnUpdate()
    {
        //leave node when need to attack player
        if (DistanceForTarget() >= 8f)
        {
            _enemyMove.SetAnimatorMovementValue(0f, 1f);
        }
        else if(DistanceForTarget() >= 1.5f)
        {
            FreeMovement();
            ApplyFreeAction();
        }
        else
        {
            _enemyMove.SetAnimatorMovementValue(0f, -0.7f);
        }
        return TaskStatus.Running;
    }
    private float DistanceForTarget() =>
        DevelopmentToos.DistanceForTarget(_targetTransform, _enemyMove.transform);

    private void FreeMovement()
    {
        switch(_actionIndex)
        {
            case 0:
                _enemyMove.SetAnimatorMovementValue(-1f, 0f);
                break;
            case 1:
                _enemyMove.SetAnimatorMovementValue(1f, 0f);
                break;
            case 2:
                _enemyMove.SetAnimatorMovementValue(0f, 0f);
                break;
            case 3:
                _enemyMove.SetAnimatorMovementValue(-1f, -1f);
                break;
            case 4:
                _enemyMove.SetAnimatorMovementValue(1f, -1f);
                break;
            case 5:
                _enemyMove.SetAnimatorMovementValue(0f, 1f);
                break;
        }
    }
    private void ApplyFreeAction()
    {
        if (_actionTimer > 0)
        {
            _actionTimer -= Time.deltaTime;
        }
        if (_actionTimer <= 0)
        {
            UpdateActionIndex();
        }

    }

    private void UpdateActionIndex()
    {
        //if same as the lastmove,try once again 
        _lastActionIndex = _actionIndex;
        _actionIndex = Random.Range(0, 6);
        if (_actionIndex ==_lastActionIndex)
        {
           _actionIndex = Random.Range(0, 6);
        }


        if (_actionIndex >= 3)
        {
            _actionTimer = 1f;
        }
        else
        {
            _actionTimer = Random.Range(2f, 5f);
        }
    }
}


