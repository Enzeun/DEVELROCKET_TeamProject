using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    // 턴 매니저 싱글턴 적용
    public static TurnManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    //===============================================================================================

    // 턴 매니저의 상태 머신
    public enum TurnState
    {
        Wait, // 아무 상태 아닐 때.
        StartBattle, // 배틀이 시작되는 시점
        PlayerTurnStart, // 플레이어 턴 시작
        PlayerPlanning, // 스킬 등록을 여기서 함.
        ExecuteSkills, // 등록된 스킬 사용
        PlayerTurnEnd, // 플레이어 턴 종료
        //CheckBattleState, // 전투가 지속 가능한 상황인지 체크
        EnemyTurn, // 적 턴
        EnemyTurnEnd, // 적 턴 종료
        EndBattle, // 배틀 종료 (전체 배틀이 종료된 상태)
    }

    public TurnState turnState { get; private set; } = TurnState.Wait; // 초기상태로 시작

    //===============================================================================================

    /// <summary>
    /// 전투매니저 초기화 (배틀 시작 시 호출)
    /// </summary>
    public void InitializeBattle()
    {
        turnState = TurnState.Wait;
        if (skillQueue.Count > 0)
        {
            skillQueue.Clear();
        }
    }

    //===============================================================================================
      
    // 플레이어의 스킬을 큐 형식으로 저장함
    public Queue<SkillBaseStat> skillQueue { get; private set; } = new ();

    //===============================================================================================

    // 지정된 state 로 넘어가기
    public void GoToStep(TurnState state)
    {
        turnState = state;

        RunTurnBehavior();

    }

    /// <summary>
    /// 현재 State 에 따른 TurnManager 의 행동 정의
    /// </summary>
    public void RunTurnBehavior()
    {
        switch (turnState)
        {
            default: break;

            case TurnState.Wait:
                {

                    break;
                }
            case TurnState.StartBattle:
                {

                    break;
                }
            case TurnState.PlayerTurnStart:
                {

                    break;
                }
            case TurnState.PlayerPlanning:
                {

                    break;
                }
            case TurnState.ExecuteSkills:
                {

                    break;
                }
            case TurnState.PlayerTurnEnd:
                {

                    break;
                }
            case TurnState.EnemyTurn:
                {

                    break;
                }
            case TurnState.EnemyTurnEnd:
                {

                    break;
                }
            case TurnState.EndBattle:
                {

                    break;
                }
        }

    }

    //===============================================================================================

    /// <summary>
    /// 전투가 지속 가능한 상황인지 체크
    /// </summary>
    public void CheckBattleState()
    {

    }

    //===============================================================================================



}
