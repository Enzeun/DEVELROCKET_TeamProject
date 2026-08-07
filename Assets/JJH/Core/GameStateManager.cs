using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    // 게임 씬 종류 Enum
    public enum GameSceneName { 
        Title, 
        Loading,
        // 전투씬
        Battle_1, Battle_2, Battle_3, Battle_4, Battle_5, Battle_6, Battle_7, Battle_8
    }
    
    public static GameStateManager Instance { get; private set; } = new GameStateManager();

    // 씬 로드용 스태틱 데이터
    public GameSceneName LoadSceneName;

    // 씬 로드시 스킬 정보 저장을 위한 스태틱 데이터
    public Dictionary<int, SkillBaseStat> saveSkillData = new();
}
