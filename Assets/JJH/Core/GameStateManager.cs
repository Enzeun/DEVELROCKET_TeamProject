using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    public enum GameSceneName { 
        Title, 
        Loading,
        // 전투씬
        Battle_1, Battle_2, Battle_3, Battle_4, Battle_5, Battle_6, Battle_7, Battle_8
    }
    
    public static GameStateManager Instance { get; private set; } = new GameStateManager();

    public GameSceneName LoadSceneName;
    public Dictionary<int, SkillBaseStat> saveSkillData = new();
}
