using System.Collections.Generic;
using Unity.VisualScripting;
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
        Initialize, // 아무 상태 아닐 때. 초기화

        StartBattle, // 배틀이 시작되는 시점

        StartNewRound, // 라운드 시작 (라운드 : 플레이어 턴, 적 턴 <- 하나의 라운드)

        PlayerTurnStart, // 플레이어 턴 시작
        PlayerPlanning, // 스킬 등록을 여기서 함.
        ExecuteSkills, // 등록된 스킬 사용
        PlayerTurnEnd, // 플레이어 턴 종료

        //CheckBattleState, // 전투가 지속 가능한 상황인지 체크

        EnemyTurn, // 적 턴
        EnemyTurnEnd, // 적 턴 종료

        EndRound, // 한 라운드 (루프 1개) 종료

        EndBattle, // 배틀 종료 (전체 배틀이 종료된 상태)
    }

    public TurnState currentState { get; private set; } = TurnState.Initialize; // 초기상태로 시작

    //===============================================================================================

    // 턴 매니저 전투 관리 필드

    public bool isBattleStarted { get; private set; } = false; // 전투가 최초로 시작될 때 true, 
    public int currentRound { get; private set; } = 0; // 플레이어 턴, 적 턴 <- 하나의 라운드



    //===============================================================================================

    /// <summary>
    /// 전투매니저 초기화 (배틀 시작 시 호출)
    /// </summary>
    private void InitializeBattle()
    {
        if (currentState != TurnState.Initialize) return;

        if (skillQueue.Count > 0)
        {
            skillQueue.Clear();
        }

        isBattleStarted = false;
        currentRound = 0;

        GoToStep(TurnState.StartBattle);
    }

    private void ReadyForNewRound()
    {
        ClearSkillQueue();

        currentRound++;
    }

    private void ClearSkillQueue()
    {
        if (skillQueue.Count > 0)
        {
            skillQueue.Clear();
        }
    }


    //===============================================================================================

    // 플레이어의 스킬을 큐 형식으로 저장함
    private Queue<SkillBaseStat> skillQueue = new();

    /// <summary>
    /// 플레이어 스킬 큐에 등록하기
    /// </summary>
    /// <param name="skill"></param>
    public void RegisterSkill(SkillBaseStat skill)
    {
        skillQueue.Enqueue(skill);
    }

    /// <summary>
    /// 큐에 등록된 스킬 사용하기
    /// </summary>
    /// <returns></returns>
    private SkillBaseStat UseRegisteredSkill()
    {
        if (skillQueue.Count == 0) return null;

        return skillQueue.Dequeue();
    }

    //===============================================================================================

    // 지정된 state 로 넘어가기
    private void GoToStep(TurnState state)
    {
        if (state == currentState) return;

        currentState = state;

        RunTurnBehavior();
    }

    /// <summary>
    /// 현재 State 에 따른 TurnManager 의 행동 정의
    /// </summary>
    private void RunTurnBehavior()
    {
        switch (currentState)
        {
            default: break;

            case TurnState.Initialize:
                {
                    InitializeBattle();
                    break;
                }
            case TurnState.StartBattle:
                {
                    StartBattle();
                    break;
                }
            case TurnState.StartNewRound:
                {
                    StartNewRound();
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
            case TurnState.EndRound:
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
    private void CheckBattleState()
    {

    }

    //===============================================================================================

    /// <summary>
    /// 필드의 모든 몬스터를 참조
    /// </summary>
    private void GetAllEnemies()
    {

    }


    //===============================================================================================

    /// <summary>
    /// 최초 배틀이 시작될 때 호출
    /// </summary>
    private void StartBattle()
    {
        if (isBattleStarted)
        {
            return;
        }

        isBattleStarted = true;

    }

    //===============================================================================================

    private void StartNewRound()
    {
        ReadyForNewRound();

    }
}
