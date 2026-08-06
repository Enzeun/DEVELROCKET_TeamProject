using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    public static GameStateManager Instance { get; private set; } = new GameStateManager();

    public string LoadSceneName;
    public Dictionary<int, SkillBaseStat> saveSkillData = new();
}
