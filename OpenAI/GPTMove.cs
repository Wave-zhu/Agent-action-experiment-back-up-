using Experiment.Combat;
using Experiment.Health;
using Experiment.Move;
using Game.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


namespace OpenAI 
{
    public class GPTMove : GPTBase
    {
        private Animator _animator;
        private EnemyMove _enemyMove;
        private EnemyCombat _enemyCombat;
        private EnemyHealth _enemyHealth;

        [SerializeField] private Transform _player;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private PlayerMove _playerMove;
        
        
        [SerializeField] private GameObject _actionDebug;
        private TextMeshProUGUI _debugText;

        public StringBuilder log = new StringBuilder();

        public Queue<string> _action = new Queue<string>();
        
        private bool _error;
        private float _time = 0;
        private int _damageReceived = 0;
        private int _damageDeal = 0;
        private int _damageDrain = 0;
        
        private string playerMessage = "";
        private bool alreadyRun = false;
        private bool ableToRun = true;
        private string instruction = "You play as the enemy of the player in the game, and your goal is to defeat the player.\n" +
                                     "You will receive information about the player's current behavior, and you will receive some information about yourself, as well as feedback.\n" +
                                     "and you will say the specified words to express the action you want to take based on these information.\n" +
                                     "For now, you can only choose the following words to express your actions: \"kick\", \"punch\", \"parry\",\"evade\".\n" +
                                     "Your final response should be separated by commas, and give it by THREE TIMES! like \"kick,parry,punch\" \"punch,punch,evade\" and so on. Otherwise the game won't be able to recognize and give you feedback.\n"+
                                     "So, actions do have their effects, you can explore this based on information you received from each round.\n" +
                                     "Don't forget that you are also a player of this game, so please enjoy the game, explore the information, achieve your goal, most of all have fun!\n";
        
        
        private async void UpdateBehavior()
        {
            while (ableToRun) {
                
                SetPlayerMessage();
                var newMessage = new ChatMessage()
                {
                    Role = "user",
                    Content = playerMessage
                };
                
                
                if (messages.Count == 0) newMessage.Content = instruction + "\n" + playerMessage;
                messages.Add(newMessage);
                playerMessage = "";

                var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                {
                    Model = "gpt-4-0613",
                    Messages = messages
                });

                if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
                {
                    var message = completionResponse.Choices[0].Message;
                    message.Content = message.Content.Trim();
                    
                    print(message.Content);
                    
                    string[] result = message.Content.Split(',');
                    
                    foreach (var str in result) 
                    {
                        _action.Enqueue(str.Trim());
                    }
                    messages.Add(message);
                    ResetDamageRecord();
                }
                else
                {
                    Debug.LogWarning("No text was generated from this prompt.");
                }

                if (BehaviorManager.MainInstance.currentGame != BehaviorManager.GameProcess.RUNNING) 
                {
                    foreach (var msg in messages) 
                    {
                        log.Append(msg.Role + ": " + msg.Content + "\n");
                    }
                    SaveLogToAssetFolder(log.ToString());
                    ableToRun = false;
                }
                
                await Task.Delay(3000);
            }
            alreadyRun = false;  
        }
        private void EnableEnemyMove()
        {
            _enemyMove.enabled = true;
        }
        private void PlayAnimation(string input)
        {
            switch (input)
            {
                case "kick":
                    _enemyCombat.SetRAttack();
                    break;
                case "punch":
                    _enemyCombat.SetLAttack();
                    break;
                case "parry": 
                    _enemyCombat.SetParry();
                    break;
                case "evade":
                    _enemyCombat.SetEvade();
                    break;
                default:
                    _error = true;
                    break;
            }
        }

        private void ApplyMove(string input)
        {
            switch (input) 
            {
                case "W":
                    _enemyMove.SetAnimatorMovementValue(0f,1f);
                    break;
                case "S":
                    _enemyMove.SetAnimatorMovementValue(0f,-1f); 
                    break;
                case "A":
                    _enemyMove.SetAnimatorMovementValue(-1f,0f); 
                    break;
                case "D":
                    _enemyMove.SetAnimatorMovementValue(1f,0f); 
                    break;
                default:
                    _error = true;
                    break;
            }
        }
        public string EnemyStateInfo()
        {
            if (_animator.AnimationAtTag("Hit"))
                return "You are being hit \n";
            else if(_animator.AnimationAtTag("Attack"))
                return "You are attacking \n";
            else if(_animator.AnimationAtTag("Motion"))
                return "You are in locomotion \n";
            else if (_animator.AnimationAtTag("Parry"))
                return "You are parrying \n";
            else if (_animator.AnimationAtTag("Idle"))
                return "You are Idle. \n";
            else
                return "";
        }
        public void SetDamageReceived(int value)
        {
            _damageReceived += value;
        }
        public void SetDamageDrain(int value)
        {
            _damageDrain += value;
        }
        public void SetDamageDeal(int value)
        {
            _damageDeal += value;
        }
        private void ResetDamageRecord()
        {
            _damageReceived = 0;
            _damageDeal = 0;
            _damageDrain = 0;
        }
        private string DamageFeedBack()
        {
            return "Damage received since last time: " + _damageReceived + ". \n"
                 + "Damage dealt since last time: " + _damageDeal+ ". \n"
                 + "Attack hp drain since last time: " + _damageDrain+ ". \n";
        }

        private void SetPlayerMessage()
        {
            if (_error) 
            {
                playerMessage = "Your previous reply was formatted incorrectly and the game didn't recognize it.";
                _error = false;
                return;
            }
            StringBuilder temp = new StringBuilder();
            switch (BehaviorManager.MainInstance.currentGame) 
            {
                case BehaviorManager.GameProcess.PLAYER_WIN:
                    temp.Append("Sorry, you lose for this time, please give your review");
                    break;
                case BehaviorManager.GameProcess.YOU_WIN:
                    temp.Append("You win! Please give your review");
                    break;
                case BehaviorManager.GameProcess.RUNNING:
                    temp.Append(DamageFeedBack());
                    temp.Append(_enemyHealth.EnemyHealthInfo());
                    temp.Append(_playerHealth.PlayerHealthInfo());
                    temp.Append(_enemyCombat.CurrentPositionInfo());
                    temp.Append(_enemyCombat.ReportAchievedInfo());
                    temp.Append(EnemyStateInfo());
                    temp.Append(_playerMove.PlayerStateInfo());
                    break;
            }
            
            playerMessage = temp.ToString();
            print(playerMessage);
        }
        
        private void SaveLogToAssetFolder(string text)
        {
            string assetPath = Application.dataPath;
        
            string directoryPath = Path.Combine(assetPath, "Log");
            
            string dateTimeFormat = "yyyy-MM-dd_HH-mm-ss"; 
            string fileName = "log_" + DateTime.Now.ToString(dateTimeFormat) + ".txt";
            string filePath = Path.Combine(directoryPath, fileName);
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(text);
            }
            
            Debug.Log("Log saved to: " + filePath);
        }
    
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _enemyMove= GetComponent<EnemyMove>();
            _enemyHealth = GetComponent<EnemyHealth>();
            _enemyCombat = GetComponent<EnemyCombat>();
            _actionDebug.SetActive(true);
            _debugText = _actionDebug.GetComponentInChildren<TextMeshProUGUI>();
            _actionDebug.SetActive(false);
        }
        private void Start()
        {
            print(instruction);
        }
        
        void OnEnable()
        {
            BehaviorManager.MainInstance.currentGame = BehaviorManager.GameProcess.RUNNING;
            ableToRun = true;
            if (!alreadyRun)
            {
                UpdateBehavior();
                alreadyRun = true; 
            }
        }

        void OnDisable()
        {
            ableToRun = false;
        }
        void Update()
        {
            _time += Time.deltaTime;
            if (_action.Count == 0) return;
            if (_time >= 1f) 
            {
                var tempAction = _action.Dequeue();
                
                _actionDebug.SetActive(true);
                _debugText.text=tempAction;
                
                PlayAnimation(tempAction);
                
                _time = 0f;
                TimerManager.MainInstance.TryGetOneTimer(0.8f, () => {
                    _actionDebug.SetActive(false);
                });
            }
        }
    }
}

