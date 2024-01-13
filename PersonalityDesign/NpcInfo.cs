using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Occupation 
{ 
    WISDOM,
    OP,
    MI_GUARDIAN,
    RARE
}
public enum Talent
{
    FIGHTING,
    HEALING,
    MAGIC,
    CRAFTING,
    FORGING
}
public enum Personality 
{   
    CYNICAL,
    PATRIOTIC,
    OPPORTUNISTIC,
    GRACIOUS
}


public class  NpcInfo : MonoBehaviour
{
    [SerializeField] private string _npcName = "";
    [SerializeField] private Occupation _npcOccupation;
    [SerializeField] private Talent _npcTalent;
    [SerializeField] private Personality _npcPersonality;

    public string GetPrompt()
    {
        return $"NPC Name: {_npcName}\n" +
               $"NPC Occupation: {_npcOccupation}\n"+
               $"NPC Talent: {_npcTalent}\n" +
               $"NPC Personality: {_npcPersonality}\n";
    }
}
