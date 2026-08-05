using UnityEngine;

public class GameStateManager
{
    public static GameStateManager Instance { get; private set; } = new GameStateManager();

    public string LoadSceneName;
}
