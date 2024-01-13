using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    [SerializeField, TextArea] private string _gameStory;
    [SerializeField, TextArea] private string _gameWorld;

    public string GetPrompt()
    {
        return $"Game World: {_gameWorld}\n" +
               $"Game Story: {_gameStory}\n";
    }
}
