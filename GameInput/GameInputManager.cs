using System.Collections;
using System.Collections.Generic;
using Game.Tool.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInputManager : Singleton<GameInputManager>
{
    private GameInputAction _gameInputAction;
    public Vector2 Movement => _gameInputAction.GameInput.Movement.ReadValue<Vector2>();
    public Vector2 CameraLook => _gameInputAction.GameInput.CameraLook.ReadValue<Vector2>();
    public bool Run => _gameInputAction.GameInput.Run.triggered;
    public bool Jump => _gameInputAction.GameInput.Jump.triggered;
    public bool LAttack => !IsPointerOverUIObject()&&_gameInputAction.GameInput.LAttack.triggered;
    public bool RAttack => !IsPointerOverUIObject()&&_gameInputAction.GameInput.RAttack.triggered;  
    public bool AIStart => _gameInputAction.GameInput.AIStart.triggered;
    public bool Lock => _gameInputAction.GameInput.Lock.triggered;
    public bool Parry => _gameInputAction.GameInput.Parry.phase == InputActionPhase.Performed;
    public bool Evade => _gameInputAction.GameInput.Evade.triggered;
    
    public bool Format => _gameInputAction.GameInput.Format.triggered;
    private static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        
        foreach (var result in results)
        {
            //UI Layer = 5
            if (result.gameObject.layer == 5)
            {
                return true;
            }
        }
        return false;
    }

    protected override void Awake()
    {
        base.Awake();
        _gameInputAction ??=new GameInputAction();
    }
    private void OnEnable()
    {
        _gameInputAction.Enable();
    }
    private void OnDisable()
    {
        _gameInputAction.Disable();
    }
}
