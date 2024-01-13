using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private Button _potionButton;
    [SerializeField] private float _maxCooldownTime = 5.0f;
    private float _currentCooldownTime = 0f;
    private bool _isCoolingDown = false;
    
    private void Update()
    {
        if (!_isCoolingDown) return;
        _currentCooldownTime -= Time.deltaTime;
        _cooldownImage.fillAmount = _currentCooldownTime / _maxCooldownTime;

        if ( _currentCooldownTime <= 0f)
        {
            _currentCooldownTime = 0f;
            _isCoolingDown = false;
            _cooldownImage.fillAmount = 0;
            _potionButton.interactable = true;
        }
    }

    public void StartCooldown()
    {
        _currentCooldownTime = _maxCooldownTime;
        _isCoolingDown = true;
        _potionButton.interactable = false;
    }

    public void StrengthPotion()
    {
        
    }
    // public void ModifyCooldown(float amount)
    // {
    //     _currentCooldownTime += amount;
    //     _currentCooldownTime = Mathf.Clamp(_currentCooldownTime, 0, _maxCooldownTime);
    // }

    // public void ResetCooldown()
    // {
    //     _currentCooldownTime = 0;
    //     _isCoolingDown = false;
    //     _cooldownImage.fillAmount = 0;
    // }
}
