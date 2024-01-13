using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Experiment.Status
{
    public abstract class CharactorStatusScaleBase : MonoBehaviour
    {
        protected float _attackScale = 1f;
        protected float _defenseScale = 1f;
        protected float _speedScale = 1f;
        protected float _recoverScale = 1f;
        protected bool _drainable = false;
        
        
        [SerializeField] protected List<GameObject> _statusList = new List<GameObject>();
        private int _index = 0;

        
        public float GetAttackStatus() => _attackScale;
        public float GetDefenseStatus() => _defenseScale;
        public float GetSpeedStatus() => _speedScale;
        public float GetRecoverStatus() => _recoverScale;
        public bool GetDrainable() => _drainable;
        
        public void SetAttackScale(string info)
        {
            Vector2 temp = StringToTuple(info);  
            _attackScale *= temp.x;
            SetColor(_statusList[0], _attackScale);
            
            // forever
            if (temp.y == -1.0f)
                return;
            TimerManager.MainInstance.TryGetOneTimer(temp.y,(() => {
                _attackScale /= temp.x;
                SetColor(_statusList[0], _attackScale);
            }));
        }
        
        public void SetDefenseScale(string info)
        {
            Vector2 temp = StringToTuple(info);                              
            _defenseScale *= temp.x;
            SetColor(_statusList[1], _defenseScale);
            
            // forever
            if (temp.y == -1.0f)
                return;
            TimerManager.MainInstance.TryGetOneTimer(temp.y,(() => {
                _defenseScale /= temp.x;
                SetColor(_statusList[1],_defenseScale); 
            }));
        }
        public void SetSpeedScale(string info)
        {
            Vector2 temp = StringToTuple(info);       
            _speedScale *= temp.x;
            SetColor(_statusList[2],_speedScale);
            
            // forever
            if (temp.y == -1.0f)
                return;
            
            TimerManager.MainInstance.TryGetOneTimer(temp.y,(() => {
                _speedScale /= temp.x;
                SetColor(_statusList[2],_speedScale); 
            }));
        }
        public void SetRecoverScale(string info)
        {
            Vector2 temp = StringToTuple(info);  
            _recoverScale = temp.x;
            SetColor(_statusList[3],temp.x);
            TimerManager.MainInstance.TryGetOneTimer(temp.y,(() => {
                _recoverScale = 1f;
                _statusList[3].SetActive(false); 
            }));
        }
        public void SetDrainable(bool value)
        {
            _drainable = value;
            _statusList[4].SetActive(value);
        }
        
        public void ResetAttackScale()
        {
            _attackScale = 1f;
        }
        public void ResetDefenseScale()
        {
            _defenseScale= 1f;
        }
        public void ResetSpeedScale()
        {
            _speedScale = 1f;
        }
        // public void ResetRecoverScale()
        // {
        //     _resetScale = 1f;
        // }
        public void SetColor(GameObject icon,float value)
        {
            if (Math.Abs(value - 1f) < 0.01f) 
            {
                icon.SetActive(false);
            }
            else if (value >= 1.8f) 
            {
                icon.SetActive(true);
                icon.GetComponent<Image>().color=Color.green;
            } 
            else if (value > 1f)
            {
                icon.SetActive(true);
                icon.GetComponent<Image>().color=Color.white;
            }
            else if (value >= 0.7f)
            {
                icon.SetActive(true); 
                icon.GetComponent<Image>().color=Color.yellow;    
            } 
            else 
            {
                icon.SetActive(true); 
                icon.GetComponent<Image>().color=Color.red;   
            }
        }

        public Vector2 StringToTuple(String info)
        {
            string[] parts = info.Split(',');
            if(parts.Length != 2)
            {
                Debug.LogError("Invalid input format.");
            }
            float x, y;
            if(float.TryParse(parts[0].Trim(), out x) && float.TryParse(parts[1].Trim(), out y)) {
                return new Vector2(x, y);
            }
            return Vector2.zero; 
        }
        protected virtual void Start()
        {
            foreach (var ui in _statusList) 
            {
                ui.SetActive(false);
            }
        }
        protected virtual void Update()
        {
            for (int i = 0; i < _statusList.Count - 1; i++) 
            {
                if (_statusList[i].activeSelf) 
                {
                    _statusList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2( _index* 45f, 0);
                    _index++;
                }
            }
            _statusList[5].SetActive(_index > 0);
            _index = 0;
        }
    }
}

