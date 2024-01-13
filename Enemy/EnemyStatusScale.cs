using Experiment.Status;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStatusScale : CharactorStatusScaleBase
{
    [SerializeField] private GameObject _status;
    private GameObject _statusInstance;
    private void Awake()
    {
        
    }
    protected override void Start()
    {
        _statusInstance = Instantiate(_status, transform.position + Vector3.up * 2.2f, Quaternion.identity);
        _statusInstance.transform.SetParent(transform);
        var temp= _statusInstance.GetComponent<Status>();
        for (int i = 0; i < temp.icons.Count; i++) 
        {
            _statusList.Add(temp.icons[i]);
        }
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        _statusInstance.transform.rotation = Quaternion.LookRotation(_statusInstance.transform.position - Camera.main.transform.position);
    }
    
}
