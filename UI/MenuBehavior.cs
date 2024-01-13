using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{
    static protected Sprite _uncheck;
    static protected Sprite _checked;

    public static void SwitchCheck(Image source)
    {
        if (!BehaviorManager.MainInstance.GetValue(source)) 
        {
            source.sprite = _checked;
            BehaviorManager.MainInstance.SetValue(source,true);
        } 
        else 
        {
            source.sprite = _uncheck;
            BehaviorManager.MainInstance.SetValue(source,false);
        }
    }
    public static void SwitchVisible(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }
    public void SwitchPause()
    {
        if (Time.timeScale != 0f) {
            Time.timeScale = 0f;
            return;
        }
        Time.timeScale = 1f;
    }
    private void Awake()
    {
        _uncheck =Resources.Load<Sprite>("Sprite/checkbox");
        _checked =Resources.Load<Sprite>("Sprite/checkbox_checked_square");
    }
}
