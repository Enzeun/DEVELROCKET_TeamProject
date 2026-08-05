using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        //DontDestroyOnLoad(gameObject);
    }

    //===============================================================================================

    // 턴 매니저의 상태 머신
    public enum TurnState
    {
        Initialize, // 아무 상태 아닐 때. 초기화

        StartBattle, // 배틀이 시작되는 시점

        StartNewRound, // 라운드 시작 (라운드 : 플레이어 턴, 적 턴 <- 하나의 라운드)

        EnemyPlanning, // 플레이어 턴 전에, 적들의 스킬이 선택됨

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



    //=============== 턴 매니저 필드 ================================================================================


    // 추적하며 디버깅 할 필드
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private TurnState currentState = TurnState.Initialize; // 초기상태로 시작
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private List<EnemyBase> enemyList = new();
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private Queue<PlayerTurnData> playerQueue = new Queue<PlayerTurnData>();
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private Queue<EnemyTurnData> enemyQueue = new Queue<EnemyTurnData>();


    // 필수 참조 필드
    [SerializeField, BoxGroup("**필수 참조 필드**"), Required]
    private BattleUIManager uIManager;




    // 턴 매니저 전투 관리 필드
    private PlayerCombat playerCombat;
    private PlayerBaseStat player;
    public bool isBattleStarted { get; private set; } = false; // 전투가 최초로 시작될 때 true, 
    public int currentRound { get; private set; } = 0; // 플레이어 턴, 적 턴 <- 하나의 라운드

 

    // 추가적으로 관리 할 필드
    [ReadOnly, ShowInInspector]
    private DrawCircleWhenMouseOver[] circleController; // Initial 에서 초기화



    //===============================================================================================

   


    //===============================================================================================

    [Button, BoxGroup("디버깅")]
    /// <summary>
    /// 전투매니저 초기화 (배틀 시작 시 호출)
    /// </summary>
    private void InitializeBattle()
    {
        if (isBattleStarted) return;

        // 여기서 부터 초기화 작업
        SetPlayerReference();

        uIManager.HideAllUI(true);


        isBattleStarted = true;

        currentRound = 0;

        GetAllEnemyCircleController();

        GetAllEnemies();

        InitUIManager();

        // 초기화 완료 후 1초 뒤 게임시작버튼 활성화

        StartCoroutine(GoToStepWithWait(TurnState.StartBattle, 1.0f));
    }

    /// <summary>
    /// 새로운 라운드를 위한 초기화
    /// </summary>
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
    [Button, BoxGroup("디버깅")]
    private void GoToStep(TurnState state)
    {
        if (state == currentState) return;

        Debug.Log($"** Turn State 변경!! : {currentState} -> {state} **");

        currentState = state;

        RunTurnBehavior();
    }

    // 지정된 state 로 넘어가기 + 일정 시간 뒤에 넘어가기
    private IEnumerator GoToStepWithWait(TurnState state, float sec)
    {
        if (state == currentState) yield break;

        yield return new WaitForSeconds(sec);

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
            case TurnState.EnemyPlanning:
                {
                    EnemyStartPlanning();
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

    //================== 게임 초기에 진행해야하는 필수 메서드 =============================================================================

    private void SetPlayerReference()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        player = playerCombat.player;
    }

    /// <summary>
    /// 필드의 모든 몬스터를 참조, 이벤트 등록
    /// </summary>
    private void GetAllEnemies()
    {
        enemyList.Clear();

        enemyList = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None).ToList();

        foreach (EnemyBase enemy in enemyList)
        {
            enemy.OnDie += (enemy) => { enemyList.Remove(enemy); };
        }

    }

    private void InitUIManager()
    {
        uIManager.SetPlayerInfo(playerCombat);
        uIManager.SetEnemyList(enemyList);
        uIManager.InitializeAllHpBar();
        uIManager.SetEnemyUILocation();
    }

    //================== Start Battle 구간 =============================================================================

    /// <summary>
    /// 최초 배틀이 시작될 때 호출
    /// </summary>
    private void StartBattle()
    {
        uIManager.ShowReadyBattleUI(true);

        uIManager.OnBattleStartClicked += OnBattleStart;
    }

    private void OnBattleStart()
    {
        uIManager.OnBattleStartClicked -= OnBattleStart;

        uIManager.ShowReadyBattleUI(false);

        GoToStep(TurnState.StartNewRound);


    }

    //================ Start New Round 구간 ===============================================================================

    private void StartNewRound()
    {
        ReadyForNewRound(); // 초기화 할 것 초기화

        uIManager.ShowBattleUI(true);

        GoToStep(TurnState.EnemyPlanning);
    }

    //=============== Enemy Planning 구간 ================================================================================
    private void EnemyStartPlanning()
    {

    }

    //===============================================================================================

    //===============================================================================================
    [Button, BoxGroup("디버깅")]
    private void PlayerSelectTarget()
    {
        foreach (var con in circleController)
        {
            con.enabled = true;

            if (con.enabled)
            {
                Debug.Log("활성화 되었습니다");
            }
            else
            {
                Debug.Log("활성화 실패했습니다");

            }
        }
    }
    //================== MonoBehavior 스크립트 =============================================================================

    private void Start()
    {
        InitializeBattle();
    }


    //================== UI Show / Hide 관련 =============================================================================



    //================== 최초 게임 시작 시 실행할 메서드들 =============================================================================
    private void GetAllEnemyCircleController()
    {
        circleController = FindObjectsByType<DrawCircleWhenMouseOver>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var con in circleController)
        {
            con.enabled = false;
        }
    }

    //================== 디버깅용 임시 메서드 =============================================================================

    [Button,BoxGroup("디버깅용 임시 메서드")]
    public void DealDamage(EnemyBase enemy, int damage)
    {
        enemy.TakeDamage(damage);
    }

}
