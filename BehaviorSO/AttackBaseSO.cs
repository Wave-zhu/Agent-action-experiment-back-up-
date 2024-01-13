using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="AttackData", menuName = "Behavior/Attack Behavior/New AttackData", order = 0)]
public class AttackBaseSO :ScriptableObject
{
    [SerializeField]private string _attackName;
    [SerializeField]private List<DamageInfo> _attackDamageInfo;
    [SerializeField]private float _attackColdDuration;
    [SerializeField]private AttackBaseSO _nextAttackInfo;
    [SerializeField]private AttackBaseSO _childAttackInfo;
    [SerializeField]private float _attackOffset;
    [SerializeField]private float _attackRange;

    public string AttackName => _attackName;
    public List<DamageInfo> AttackDamageInfo => _attackDamageInfo;
    public float AttackColdDuration => _attackColdDuration;
    public AttackBaseSO NextAttackInfo => _nextAttackInfo;
    public AttackBaseSO ChildAttackInfo => _childAttackInfo;
    public float AttackOffset => _attackOffset;
    public float AttackRange => _attackRange;   
    public bool HasChildAttackInfo()
    {
        return _childAttackInfo != null; 
    }
    public bool HasNextAttackInfo()
    {
        return _nextAttackInfo != null;
    }
}
public enum DamageType
{
    PUNCH,
    KICK,
    SWORD
}
[System.Serializable]
public class DamageInfo
{
    public DamageType damageType;
    public int damage;
    public string hitName;
    public string parryName;
}
