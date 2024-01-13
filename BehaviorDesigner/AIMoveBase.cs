using BehaviorDesigner.Runtime.Tasks;
using Experiment.Move;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIMoveBase : Action
{
    protected EnemyMove _enemyMove;
    protected Transform _targetTransform;
    public override void OnAwake()
    {
        _enemyMove = GetComponent<EnemyMove>();
        _targetTransform = _enemyMove.targetTransform;
    }
}
