using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Game.Tool;
using UnityEngine.UI;
using UnityEngine.Rendering.VirtualTexturing;
using BehaviorDesigner.Runtime;
using OpenCover.Framework.Model;
using TMPro;
using Unity.VisualScripting;

public class BehaviorManager : Game.Tool.Singleton.Singleton<BehaviorManager>
{
    /// <summary>
    /// Skill Match
    /// </summary>
    [SerializeField] private List<GameObject> behavior = new List<GameObject>();
    private Dictionary<GameObject, IconMatch> _matchBehavior = new Dictionary<GameObject, IconMatch>();
    private Dictionary<GameObject, RectTransform> _transformBehavior = new Dictionary<GameObject, RectTransform>();
    
    [SerializeField] private RectTransform _displayZero;
    public StringBuilder currentInput;
    
    /// <summary>
    /// Check Box
    /// </summary>
    [SerializeField] private List<Image> _menuOption = new List<Image>();
    private Dictionary<Image, bool> _optionCheck = new Dictionary<Image, bool>();
    
    /// <summary>
    /// Damage UI
    /// </summary>
    [SerializeField] private RectTransform _damageDebugLocation;
    [SerializeField] private GameObject _debugUI;
    private List<GameObject> _debugList = new List<GameObject>();
    private readonly int _limitNum = 5;
    private int _bitUsed = 0;
    private bool _ableToDebug = true;


    public enum GameProcess
    {
        IDLE,
        RUNNING,
        PLAYER_WIN,
        YOU_WIN,
    }
    public GameProcess currentGame;
    
    
    #region Menu

    public bool GetValue(Image img)
    {
        if (_optionCheck.TryGetValue(img, out bool value))
        {
            return value;
        }
        else
        {
            throw new KeyNotFoundException("The specified image key was not found.");
        }
    }

    public void SetValue(Image img, bool value)
    {
        if (_optionCheck.ContainsKey(img))
        {
            _optionCheck[img] = value;
        }
        else
        {
            throw new KeyNotFoundException("The specified image key was not found.");
        }
    }

    #endregion
    
    #region Skill
    public void ClearAll()
    {
        for(int i = 0; i < behavior.Count; i++)
        {
             _matchBehavior[behavior[i]].DisMatch();
        }
    }
    private void StartMatch()
    {
        int displayIndex = 0;
        for (int i = 0; i < behavior.Count; i++)
        {
            if (_matchBehavior[behavior[i]].Match(currentInput.ToString())&&GetValue(_menuOption[2]))
            {
                _transformBehavior[behavior[i]].anchoredPosition = _displayZero.anchoredPosition 
                    + new Vector2((5 - behavior[i].name.Length)*100, -displayIndex * 100);
                displayIndex++;
            }
            else
            {
                _transformBehavior[behavior[i]].anchoredPosition = new Vector2(10000, 0);
            }
        }
    }
    
    #endregion

    #region Damage
    public void DamageDebug(float damage)
    {
        if (!_ableToDebug) return;
        int idx = Unused();
        var newIcon=_debugList[idx];
        newIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-80.0f * idx);
        newIcon.SetActive(true);
        if (damage < 0) 
        {
            newIcon.GetComponent<Image>().color = Color.red;
            newIcon.GetComponentInChildren<TextMeshProUGUI>().text = " " + damage;
            TimerManager.MainInstance.TryGetOneTimer(1.8f,() => {
                newIcon.SetActive(false);
                _bitUsed ^= (1<<idx);
            });
        } 
        else 
        {
            newIcon.GetComponent<Image>().color = Color.green;
            newIcon.GetComponentInChildren<TextMeshProUGUI>().text = "+" + damage;
            TimerManager.MainInstance.TryGetOneTimer(1.8f,() => {
                newIcon.SetActive(false);
                _bitUsed ^= (1<<idx);
            });
        }
    }

    public void SetDebugVariable()
    {
        _ableToDebug = !_ableToDebug;
    }
    public int Unused()
    {
        for (int i = 0; i < _limitNum; i++) 
        {
            int temp = _bitUsed & (1<<i);
            if (temp == 0) 
            {
                _bitUsed |= (1<<i);
                return i;
            }
        }
        return 0;
    }
    #endregion


    protected override void Awake()
    {
        base.Awake();
        currentInput = new StringBuilder();
    }
    void Start()
    {
        //skill start
        foreach (var skill in behavior) {
            var tempIcon = skill.GetComponent<IconMatch>();
            var tempTransform = skill.GetComponent<RectTransform>();
            if(tempIcon)
                _matchBehavior.Add(skill,tempIcon);
            if(tempTransform)
                _transformBehavior.Add(skill,tempTransform);
        }
        
        
        //menu start
        foreach (var option in _menuOption) 
        {
            _optionCheck.Add(option,true);
        }
        
        SetValue(_menuOption[0],true);
        
        // damage start
        for (int i = 0; i < _limitNum; i++) 
        {
            var newIcon = Instantiate(_debugUI, _damageDebugLocation);
            newIcon.SetActive(false);
            _debugList.Add(newIcon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartMatch();
    }
    
}
