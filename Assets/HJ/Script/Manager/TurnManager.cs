using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        GameOver, // 게임오버 (플레이어 사망)
    }



    //=============== 턴 매니저 필드 ================================================================================


    // 추적하며 디버깅 할 필드
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private TurnState _currentState = TurnState.Initialize; // 초기상태로 시작
    public TurnState currentState { get => _currentState; }
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private List<EnemyBase> enemyList = new();
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private int playerQueueCount { get => playerQueue.Count; }
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private int enemyQueueCount { get => enemyQueue.Count; }
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private bool isGameOver = false;
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private int currentSelectedSkillId = -1;
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private EnemyBase currentTargetEnemy = null;
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    private int nowCost
    {
        get
        {
            if (player == null)
            {
                return -1;
            }
            return player.NowCost;
        }
    }


    // 플레이어와 적의 행동은 큐로 관리
    private Queue<PlayerTurnData> playerQueue = new Queue<PlayerTurnData>();
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
        if (playerQueue.Count > 0)
        {
            playerQueue.Clear();
        }
    }


    //===============================================================================================

    /// <summary>
    /// 플레이어 스킬 큐에 등록하기
    /// </summary>
    /// <param name="skill"></param>
    public void RegisterSkill(PlayerTurnData skill)
    {
        playerQueue.Enqueue(skill);
    }


    //===================== Enemy 의 행동을 큐로 관리 =============================================================

    private void RegisterEnemyBehavior(EnemyTurnData enemyTurnData)
    {
        enemyQueue.Enqueue(enemyTurnData);
    }



    //===============================================================================================

    // 지정된 state 로 넘어가기
    [Button, BoxGroup("디버깅")]
    private void GoToStep(TurnState state)
    {
        if (state == _currentState) return;

        Debug.Log($"** Turn State 변경!! : {_currentState} -> {state} **");

        _currentState = state;

        RunTurnBehavior();
    }

    // 지정된 state 로 넘어가기 + 일정 시간 뒤에 넘어가기
    private IEnumerator GoToStepWithWait(TurnState state, float sec)
    {
        if (state == _currentState) yield break;

        yield return new WaitForSeconds(sec);

        _currentState = state;

        RunTurnBehavior();
    }

    /// <summary>
    /// 현재 State 에 따른 TurnManager 의 행동 정의
    /// </summary>
    private void RunTurnBehavior()
    {
        switch (_currentState)
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
                    PlayerTurnStart();
                    break;
                }
            case TurnState.PlayerPlanning:
                {
                    PlayerPlanning();
                    break;
                }
            case TurnState.ExecuteSkills:
                {
                    ExecuteSkill();
                    break;
                }
            case TurnState.PlayerTurnEnd:
                {
                    PlayerTurnEnd();
                    break;
                }
            case TurnState.EnemyTurn:
                {
                    EnemyTurn();
                    break;
                }
            case TurnState.EnemyTurnEnd:
                {
                    EnemyTurnEnd();
                    break;
                }
            case TurnState.EndRound:
                {
                    EndRound();
                    break;
                }
            case TurnState.EndBattle:
                {
                    EndBattle();
                    break;
                }
            case TurnState.GameOver:
                {
                    GameOver();
                    break;
                }
        }

    }

    //============== 게임 중단점 체크 ============================================================

    enum EnemyBattleState
    {
        GameOver,
        NoMoreEnemyQueue,
        Continue,
    }

    /// <summary>
    /// 전투가 지속 가능한 상황인지 체크 (아직 적이 남아있는지)
    /// </summary>
    private EnemyBattleState CheckEnemyBattleState()
    {
        if (isGameOver)
        {
            return EnemyBattleState.GameOver;
        }

        if (player.NowHP <= 0)
        {
            return EnemyBattleState.GameOver;
        }

        if (enemyQueueCount <= 0)
        {
            return EnemyBattleState.NoMoreEnemyQueue;
        }
        return EnemyBattleState.Continue;

    }




    //================== 게임 초기에 진행해야하는 필수 메서드 =============================================================================

    private void SetPlayerReference()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        player = playerCombat.player;
        player.OnDead += () => { isGameOver = true; };
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
            enemy.InitEnemyTarget(playerCombat);
            enemy.OnDie += (enemy) => { enemyList.Remove(enemy); };
        }

    }

    private void InitUIManager()
    {
        uIManager.SetPlayerInfo(playerCombat);
        uIManager.SetEnemyList(enemyList);
        uIManager.InitUIDictinary();
        uIManager.InitializeAllHpBar();
        uIManager.SetEnemyUILocation();
        uIManager.InitSkillButtons();
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
        foreach (EnemyBase enemy in enemyList)
        {
            enemy.SelectBehaviour();

            EnemyTurnData enemyTurnData = new EnemyTurnData(enemy, enemy.currentBehaviour);

            RegisterEnemyBehavior(enemyTurnData);

            uIManager.ShowBehaveIcon(enemy);
        }

        GoToStep(TurnState.PlayerTurnStart);
    }


    //=============== Player Turn Start 구간 ================================================================================

    private void PlayerTurnStart()
    {
        // 버튼에 이벤트 구독을 한다
        SubscribeSkillBtnClicked();
        // 적 클릭에 이벤트 구독을 한다
        SubscribeEnemyClicked();
        // 
        // 다음 단계로 이동
        GoToStep(TurnState.PlayerPlanning);
    }


    //================== Player Planning 구간 =============================================================

    private void PlayerPlanning()
    {
        if (!CheckRemainCost())
        {
            EndPlayerPlanning();
            return;
        }

        uIManager.ShowSkillMenu(true);

    }
    private void OnSkillBtnClicked(int id)
    {
        currentSelectedSkillId = id;
        // 스킬버튼을 누르면 스킬메뉴를 숨김.
        uIManager.ShowSkillMenu(false);

        // 광역 공격 판정 여기서 해야함 ******************************************미완성

        // 적을 선택 가능하게 한다.
        EnableSelectTarget(true);
    }


    private void OnEnemyClicked(EnemyBase enemy)
    {
        // 적을 클릭하면
        currentTargetEnemy = enemy;
        // 해당 스킬을 큐에 저장한다
        RegisterSkill(MakePlayerTurnData());
        // 스킬 id 를 초기화 한다
        currentSelectedSkillId = -1;
        // 선택이 불가능하게 한다
        EnableSelectTarget(false);

        // 남은 스킬 코스트를 확인하고
        if (CheckRemainCost())
        {
            // 코스트가 남아있으면 스킬메뉴를 다시 보여준다
            uIManager.ShowSkillMenu(true);
            return;
        }

        // 코스트가 없으면 멈춘다.
        // 임시로 턴을 종료하게 만듦
        EndPlayerPlanning();
    }

    private bool CheckRemainCost()
    {
        if (nowCost <= 0)
        {
            return false;
        }
        return true;
    }

    private PlayerTurnData MakePlayerTurnData()
    {
        PlayerTurnData turnData;

        if (player.SkillData.TryGetValue(currentSelectedSkillId, out SkillBaseStat skill))
        {
            turnData = new PlayerTurnData(skill, new EnemyBase[] { currentTargetEnemy });
            return turnData;
        }
        else
        {
            
            return null;
        }


    }


    private void OnEndTurnBtnClicked()
    {
        uIManager.ShowSkillMenu(false);

        EndPlayerPlanning();
    }

    private void EndPlayerPlanning()
    {
        GoToStep(TurnState.ExecuteSkills);
    }

    //==================== Execute Skills 구간 ===========================================================

    private void ExecuteSkill()
    {
        PlayNextSkill();
    }

    private void PlayNextSkill()
    {
        if (playerQueueCount <= 0)
        {
            GoToStep(TurnState.PlayerTurnEnd);
            return;
        }

        PlayerTurnData turnData = playerQueue.Dequeue();

        int count = turnData.target.Length;

        Transform[] targets = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            targets[i] = turnData.target[i].transform;
        }

        playerCombat.SetNowSkillAndTarget(turnData.skill, targets);

        StartCoroutine(PlayBeforeAttackRoutine());
    }

    private IEnumerator PlayBeforeAttackRoutine()
    {
        playerCombat.PlayerActiveSkillSelect();

        yield return new WaitForSeconds(1);

        StartCoroutine(PlayAttackRoutine());
    }

    private IEnumerator PlayAttackRoutine()
    {
        playerCombat.PlayerAnmationActive();

        yield return new WaitForSeconds(2);

        if (playerQueueCount <= 0)
        {
            GoToStep(TurnState.PlayerTurnEnd);
            yield break;
        }

        PlayNextSkill();
    }



    //========================= Player Turn End 구간 =====================================================

    private void PlayerTurnEnd()
    {
        GoToStep(TurnState.EnemyTurn);

    }



    //====================== Enemy Turn 구간 ==============================================================

    private void EnemyTurn()
    {
        PlayNextEnemyTurn();
    }
    private void PlayNextEnemyTurn()
    {
        switch (CheckEnemyBattleState())
        {
            case EnemyBattleState.GameOver:
                GoToStep(TurnState.GameOver);
                break;

            case EnemyBattleState.NoMoreEnemyQueue:
                GoToStep(TurnState.EnemyTurnEnd);
                break;
            case EnemyBattleState.Continue:
                break;
        }

        EnemyTurnData turnData = enemyQueue.Dequeue();

        if (turnData.casterEnemy == null || turnData.casterEnemy.isDead)
        {
            PlayNextEnemyTurn();
            return;
        }
        else
        {
            StartCoroutine(PlayEnemyAttackRoutine(turnData.casterEnemy));
        }
    }

    private IEnumerator PlayEnemyAttackRoutine(EnemyBase caster)
    {
        caster.StartBehaviour();

        yield return new WaitForSeconds(1.5f);

        switch (CheckEnemyBattleState())
        {
            case EnemyBattleState.GameOver:
                GoToStep(TurnState.GameOver);
                break;

            case EnemyBattleState.NoMoreEnemyQueue:
                GoToStep(TurnState.EnemyTurnEnd);
                break;
            case EnemyBattleState.Continue:
                break;
        }

        PlayNextEnemyTurn();
    }

    //======================== Enemy Turn End 구간 ====================================================

    private void EnemyTurnEnd()
    {
        GoToStep(TurnState.EndRound);
    }

    


    //======================== End Round 구간 ==============================================================

    private void EndRound()
    {
        StartCoroutine(RoundResult());
    }

    private IEnumerator RoundResult()
    {
        yield return new WaitForSeconds(3f);

        GoToStep(TurnState.StartNewRound);
    }

    //========================== End Battle 구간 =====================================================================

    private void EndBattle()
    {

    }





    //==================== Game Over 구간 ===========================================================================

    private void GameOver()
    {

    }



    //===============================================================================================
    //===============================================================================================
    //===============================================================================================
    //===============================================================================================
    //===============================================================================================
    //===============================================================================================
    //===============================================================================================
    [Button, BoxGroup("디버깅")]
    private void EnableSelectTarget(bool enable)
    {
        foreach (var con in circleController)
        {
            con.enabled = enable;

            if (con.enabled)
            {
                Debug.Log("활성화 되었습니다");
            }
            else
            {
                con.InitCircleLocation();
                Debug.Log("비활성화 되었습니다");
            }
        }
    }
    //================== MonoBehavior 스크립트 =============================================================================

    private void Start()
    {
        StartCoroutine(WaitForStartGame());
    }

    private IEnumerator WaitForStartGame()
    {
        yield return new WaitForSeconds(1);

        InitializeBattle();

        uIManager.FadeDarkImage(0.5f);

        yield return new WaitForSeconds(0.5f);

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

    //================== 이벤트 구독 메서드 =============================================================================

    private void SubscribeSkillBtnClicked()
    {
        uIManager.OnSkillBtnClicked += OnSkillBtnClicked;
        uIManager.OnEndTurnBtnClicked += OnEndTurnBtnClicked;
    }
    private void UnSubscribeSkillBtnClicked()
    {
        uIManager.OnSkillBtnClicked -= OnSkillBtnClicked;
        uIManager.OnEndTurnBtnClicked -= OnEndTurnBtnClicked;
    }

    private void SubscribeEnemyClicked()
    {
        foreach (var con in circleController)
        {
            con.OnEnemyClicked += OnEnemyClicked;
        }
    }
    private void UnSubscribeEnemyClicked()
    {
        foreach (var con in circleController)
        {
            con.OnEnemyClicked -= OnEnemyClicked;
        }
    }




    //================== 디버깅용 임시 메서드 =============================================================================

    [Button, BoxGroup("디버깅용 임시 메서드")]
    public void EnemyDealDamage(EnemyBase enemy, int damage)
    {
        enemy.TakeDamage(damage);
    }

    [Button, BoxGroup("디버깅용 임시 메서드")]
    public void PlayerDealDamage(int damage)
    {
        player.TakeDamage(damage);
    }

    [Button, BoxGroup("디버깅용 임시 메서드")]
    public void PlayerUseSkill(int id, Transform target)
    {
        bool get = player.SkillData.TryGetValue(id, out var skill);

        if (get)
        {
            playerCombat.SetNowSkillAndTarget(skill, new Transform[] { target });
            playerCombat.PlayerAnmationActive();
        }
        else
        {
            Debug.Log("없는 스킬데이터 입니다");
            return;
        }
    }
}
