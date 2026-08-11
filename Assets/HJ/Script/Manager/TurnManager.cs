using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Scene currentScene;
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private int sceneIndex;
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
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private int remainEnemy = -1;
    [ShowInInspector, BoxGroup("필드 값 추적"), ReadOnly]
    public int currentRound { get; private set; } = 0; // 플레이어 턴, 적 턴 <- 하나의 라운드
    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private bool isEnemyTurnStarted = false; // 적 턴이 중복되지 않게 막는 필드

    // 플레이어와 적의 행동은 큐로 관리
    private Queue<PlayerTurnData> playerQueue = new Queue<PlayerTurnData>();
    private Queue<EnemyTurnData> enemyQueue = new Queue<EnemyTurnData>();



    // 필수 참조 필드
    [SerializeField, BoxGroup("**필수 참조 필드**"), Required]
    private BattleUIManager uIManager;


    // 턴 매니저 전투 관리 필드
    public PlayerCombat playerCombat { get; private set; }
    private PlayerBaseStat player;
    public bool isBattleStarted { get; private set; } = false; // 전투가 최초로 시작될 때 true, 



    // 추가적으로 관리 할 필드
    [ReadOnly, ShowInInspector]
    private DrawCircleWhenMouseOver[] circleController; // Initial 에서 초기화



    WaitForSeconds waitHalfSec = new WaitForSeconds(0.5f);
    WaitForSeconds waitOnefSec = new WaitForSeconds(1f);
    WaitForSeconds waitTwofSec = new WaitForSeconds(1f);



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
        StartCoroutine(GoToStepWithWait(TurnState.StartBattle));
    }

    /// <summary>
    /// 새로운 라운드를 위한 초기화
    /// </summary>
    private void ReadyForNewRound()
    {
        CheckRemainEnemy();

        player.ResetCost();

        currentSelectedSkillId = -1;

        ClearTurnQueue();

        currentRound++;
    }

    private void ClearTurnQueue()
    {
        if (playerQueueCount > 0)
        {
            playerQueue.Clear();
        }
        if (enemyQueueCount > 0)
        {
            enemyQueue.Clear();
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
        if (isGameOver) return;

        if (state == _currentState) return;

        Debug.Log($"** Turn State 변경!! : {_currentState} -> {state} **");

        _currentState = state;

        RunTurnBehavior();
    }

    // 지정된 state 로 넘어가기 + 일정 시간 뒤에 넘어가기
    private IEnumerator GoToStepWithWait(TurnState state)
    {
        if (isGameOver) yield break;

        if (state == _currentState) yield break;

        yield return waitTwofSec;

        _currentState = state;

        RunTurnBehavior();
    }

    /// <summary>
    /// 현재 State 에 따른 TurnManager 의 행동 정의
    /// </summary>
    private void RunTurnBehavior()
    {
        if (isGameOver) return;

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
        PlayerIsDead,
        NoMoreEnemyQueue,
        ContinueBattle,
    }

    /// <summary>
    /// 전투가 지속 가능한 상황인지 체크 (플레이어가 죽었는지)
    /// </summary>
    private EnemyBattleState CheckEnemyBattleState()
    {
        if (isGameOver)
        {
            return EnemyBattleState.PlayerIsDead;
        }

        if (player.NowHP <= 0)
        {
            return EnemyBattleState.PlayerIsDead;
        }

        if (enemyQueueCount <= 0)
        {
            return EnemyBattleState.NoMoreEnemyQueue;
        }

        return EnemyBattleState.ContinueBattle;

    }

    enum PlayerBattleState
    {
        NoMoreEnemy,
        NoMoreSkillQueue,
        ContinueBattle,
    }

    /// <summary>
    /// 전투가 지속 가능한 상황인지 체크 (아직 적이 남아있는지)
    /// </summary>
    private PlayerBattleState CheckPlayerBattleState()
    {
        CheckRemainEnemy();

        if (remainEnemy <= 0)
        {
            return PlayerBattleState.NoMoreEnemy;
        }

        if (playerQueueCount <= 0)
        {
            return PlayerBattleState.NoMoreSkillQueue;
        }
        return PlayerBattleState.ContinueBattle;

    }

    private void CheckRemainEnemy()
    {
        remainEnemy = 0;

        foreach (var enemy in enemyList)
        {
            if (!enemy.isDead)
            {
                remainEnemy++;
            }
        }
    }

    //================== 게임 초기에 진행해야하는 필수 메서드 =============================================================================

    private void SetPlayerReference()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        player = playerCombat.player;
        player.OnDead += () => { isGameOver = true; GameOver(); };
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
        }

    }

    private void InitUIManager()
    {
        uIManager.SetPlayerInfo(playerCombat);
        uIManager.SetEnemyList(enemyList);
        uIManager.InitUIDictinary();
        uIManager.InitializeAllHpBar();
        uIManager.SetEnemyUILocation();
        // 시작 전 UI 보여주기
        uIManager.ShowReadyBattleUI(true);
    }

    //================== Start Battle 구간 =============================================================================

    /// <summary>
    /// 최초 배틀이 시작될 때 호출
    /// </summary>
    private void StartBattle()
    {
        uIManager.OnBattleStartClicked += OnBattleStart;

    }

    private void OnBattleStart()
    {
        Debug.Log("버튼 클릭");
        uIManager.OnBattleStartClicked -= OnBattleStart;
        // 시작 전 UI 숨기기
        uIManager.ShowReadyBattleUI(false);
        // 몬스터 스폰 애니메이션
        SpawnAllEnemies();
        // 라운드 시작
        GoToStep(TurnState.StartNewRound);
    }

    private void SpawnAllEnemies()
    {
        foreach (var enemy in enemyList)
        {
            enemy.SpawnEnemy();
        }
    }

    //================ Start New Round 구간 ===============================================================================

    private void StartNewRound()
    {
        // 초기화 할 것 초기화
        ReadyForNewRound();

        // 맨 첫 라운드에서만 UI 띄우기
        if (currentRound == 1)
        {
            uIManager.ShowBattleUI(true);
        }

        // 적 계획 단계로 이동
        GoToStep(TurnState.EnemyPlanning);
    }

    //=============== Enemy Planning 구간 ================================================================================

    private void EnemyStartPlanning()
    {
        // 모든 적이 순차적으로 행동결정
        StartCoroutine(EnemyPlanningRoutine());
    }

    /// <summary>
    /// 0.5 초 간격으로 적들이 자신의 행동을 정함.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyPlanningRoutine()
    {
        foreach (EnemyBase enemy in enemyList)
        {
            // null 이거나 isDead 면 건너뛰기
            if (enemy == null || enemy.isDead) continue;

            // 행동 선택
            enemy.SelectBehaviour();

            // 선택한 행동으로 새로운 턴 Data 생성
            EnemyTurnData enemyTurnData = new EnemyTurnData(enemy, enemy.currentBehaviour);

            // 큐에 등록
            RegisterEnemyBehavior(enemyTurnData);

            yield return waitHalfSec;
        }

        EndEnemyPlanning();
    }

    private void EndEnemyPlanning()
    {
        // 행동이 끝나면 전체 유닛 행동아이콘 표시
        uIManager.ShowBehaveIcon();

        // 다음 단계로
        GoToStep(TurnState.PlayerTurnStart);
    }

    //=============== Player Turn Start 구간 ================================================================================

    private void PlayerTurnStart()
    {
        // 버튼에 이벤트 구독을 한다
        UnSubscribeSkillBtnClicked();
        SubscribeSkillBtnClicked();

        // 다음 단계로 이동
        GoToStep(TurnState.PlayerPlanning);
    }


    //================== Player Planning 구간 =============================================================

    private void PlayerPlanning()
    {
        // 스킬 큐에 데이터가 존재하면 EndTurn 버튼 활성화
        if (playerQueueCount > 0)
        {
            uIManager.EnableEndTurnBtn();
        }

        // 스킬 메뉴를 보여준다
        uIManager.ShowSkillMenu();

        // 플레이어가 행동할 때 까지 대기.
    }

    private void OnSkillBtnClicked(int id)
    {
        // 플레이어가 선택한 스킬 id 확인
        currentSelectedSkillId = id;

        // 스킬버튼을 누르면 스킬메뉴를 숨김.
        uIManager.ShowSkillMenu(false);

        // 스킬 정보 확인
        CheckSkillResult result = CheckSkillData();

        switch (result)
        {
            case CheckSkillResult.IsBug:
                break;

            // 단일 공격일 경우
            case CheckSkillResult.IsSingleAttack:
                // 먼저 턴종료 버튼을 비활성화
                uIManager.EnableEndTurnBtn(false);
                // 적 클릭에 이벤트 구독을 한다
                UnSubscribeEnemyClicked();
                SubscribeEnemyClicked();
                // 적을 선택 가능하게 한다.
                EnableSelectTarget(true);
                break;

            // 범위 공격일 경우
            case CheckSkillResult.IsAreaAttack:
                MakeAreaAttack();
                break;

            // 코스트가 부족할 경우
            case CheckSkillResult.OverCost:
                Debug.Log("코스트가 부족합니다");
                PlayerPlanning();
                break;
        }
    }

    // 지금 선택한 스킬이 사용가능한지 등등 확인
    private CheckSkillResult CheckSkillData()
    {
        if (!player.SkillData.TryGetValue(currentSelectedSkillId, out var skillData))
        {
            Debug.Log("스킬 ID 가 없습니다.");
            return CheckSkillResult.IsBug;
        }

        if (nowCost < skillData.GetCost())
        {
            Debug.Log("스킬 코스트가 부족합니다.");
            return CheckSkillResult.OverCost;
        }

        switch (skillData.TargetType)
        {
            case SkillEnums.SkillTargetType.Single:
                return CheckSkillResult.IsSingleAttack;

            case SkillEnums.SkillTargetType.Area:
                return CheckSkillResult.IsAreaAttack;
        }

        Debug.Log("스킬 데이터에 처리가 안된 것이 있습니다.");
        return CheckSkillResult.IsBug;
    }

    enum CheckSkillResult
    {
        OverCost,
        IsAreaAttack,
        IsSingleAttack,
        IsBug,
    }

    // 단일공격에서의 로직
    private void OnEnemyClicked(EnemyBase enemy)
    {
        // 적을 클릭하면 구독을 해제한다
        UnSubscribeEnemyClicked();

        // 해당 스킬을 큐에 저장한다
        RegisterSkill(MakeSingleTargetTurnData(enemy));

        // 스킬 id 를 초기화 한다
        currentSelectedSkillId = -1;

        // 선택이 불가능하게 한다
        EnableSelectTarget(false);

        // 남은 스킬 코스트를 확인하고
        if (CheckRemainCost())
        {
            // 코스트가 남아있으면 다시 Planning
            PlayerPlanning();

            return;
        }

        // 코스트가 없으면 턴종료 버튼을 활성화하고 대기
        uIManager.EnableEndTurnBtn();

    }

    // 범위 공격에서의 로직
    private void MakeAreaAttack()
    {
        // 모든 적을 타깃으로 설정 하고 큐에 저장
        RegisterSkill(MakeMultiTargetTurnData());

        // 스킬 id 를 초기화 한다
        currentSelectedSkillId = -1;

        // 남은 스킬 코스트를 확인하고
        if (CheckRemainCost())
        {
            // 코스트가 남아있으면 다시 Planning
            PlayerPlanning();

            return;
        }

        // 코스트가 없으면 대기

    }


    private bool CheckRemainCost()
    {
        if (nowCost <= 0)
        {
            return false;
        }
        return true;
    }

    // 단일 대상 턴데이터 생성
    private PlayerTurnData MakeSingleTargetTurnData(EnemyBase enemy)
    {
        PlayerTurnData turnData;

        EnemyBase[] targets = new[] { enemy };

        if (player.SkillData.TryGetValue(currentSelectedSkillId, out SkillBaseStat skill))
        {
            turnData = new PlayerTurnData(skill, targets);
            // 코스트를 깎는다.
            player.UseCost(skill.GetCost());

            return turnData;
        }
        else
        {
            return null;
        }
    }

    // 범위 대상 턴 데이터 생성
    private PlayerTurnData MakeMultiTargetTurnData()
    {
        PlayerTurnData turnData;

        // 죽은 enemy 는 제외시키고 타깃으로 설정
        List<EnemyBase> targetList = new List<EnemyBase>();
        foreach(var target in enemyList)
        {
            if (target.isDead) continue;

            targetList.Add(target);
        }

        EnemyBase[] targets = targetList.ToArray();

        if (player.SkillData.TryGetValue(currentSelectedSkillId, out SkillBaseStat skill))
        {
            turnData = new PlayerTurnData(skill, targets);
            // 코스트를 깎는다.
            player.UseCost(skill.GetCost());

            return turnData;
        }
        else
        {
            return null;
        }
    }


    private void OnEndTurnBtnClicked()
    {
        UnSubscribeSkillBtnClicked();
        UnSubscribeEnemyClicked();

        uIManager.ShowSkillMenu(false);

        uIManager.EnableEndTurnBtn(false);

        EndPlayerPlanning();
    }

    private void EndPlayerPlanning()
    {
        GoToStep(TurnState.ExecuteSkills);
    }

    //==================== Execute Skills 구간 ===========================================================

    private void ExecuteSkill()
    {
        // 플레이어 공격 끝나는 지점 구독
        UnSubscribePlayerCompleteAttack();
        SubscribePlayerCompleteAttack();

        // 스킬 시전 모션 시작
        playerCombat.PlayerActiveSkillSelect();

        PlayNextSkill();
    }

    private void PlayNextSkill()
    {
        // 큐에 있는걸 꺼낸다.
        PlayerTurnData turnData = playerQueue.Dequeue();

        // 큐에있는 적의 숫자 만큼 똑같은 Transform 배열을 생성한다.
        int length = turnData.target.Length;

        Transform[] targets = new Transform[length];

        // 싱글, 멀티 타깃에 따라 분기
        switch (turnData.skill.TargetType)
        {
            case SkillEnums.SkillTargetType.Single:
                // 싱글 타깃인데, 목표가 이미 죽었으면, 다른 목표 검색
                if (turnData.target[0].isDead)
                {
                    foreach (var enemy in enemyList)
                    {
                        if (!enemy.isDead)
                        {
                            targets[0] = enemy.transform;
                            break;
                        }
                    }
                }
                else
                {
                    targets[0] = turnData.target[0].transform;
                }
                break;


            case SkillEnums.SkillTargetType.Area:

                for (int i = 0; i < length; i++)
                {
                    targets[i] = turnData.target[i].transform;
                }
                break;
        }


        // 스킬 준비
        playerCombat.SetNowSkillAndTarget(turnData.skill, targets);

        // 스킬 시전 모션 시작
        StartCoroutine(PlayBeforeAttackRoutine());
    }

    private IEnumerator PlayBeforeAttackRoutine()
    {

        // 0.5초 후
        yield return waitHalfSec;
        // 실제 공격
        playerCombat.PlayerAnmationActive();
    }

    private void PlayerCompleteAttack()
    {
        StartCoroutine(PlayerDelay());
    }

    private IEnumerator PlayerDelay()
    {
        // 0.5 초 기다린다
        yield return waitHalfSec;
        // 남은 적이 있는지, 큐에 데이터가 남았는지, 배틀 상황을 확인한다.
        switch (CheckPlayerBattleState())
        {
            // 데이터가 있으면 다음 스킬 시전
            case PlayerBattleState.ContinueBattle:
                PlayNextSkill();
                yield break;

            // 큐에 남은 데이터가 없으면
            case PlayerBattleState.NoMoreSkillQueue:

                // 애니메이션을 idle로 복귀
                playerCombat.PlayerActiveIdle();

                // 턴 종료
                GoToStep(TurnState.PlayerTurnEnd);
                yield break;

            // 모든 적이 처치됐으면
            case PlayerBattleState.NoMoreEnemy:

                // 애니메이션을 idle로 복귀
                playerCombat.PlayerActiveIdle();

                // 배틀 종료
                GoToStep(TurnState.EndBattle);
                yield break;
        }
    }


    //========================= Player Turn End 구간 =====================================================


    private void PlayerTurnEnd()
    {
        StartCoroutine(PlayerTurnEndDelay());
    }

    private IEnumerator PlayerTurnEndDelay()
    {
        yield return waitHalfSec;

        GoToStep(TurnState.EnemyTurn);
    }

    //====================== Enemy Turn 구간 ==============================================================

    private void EnemyTurn()
    {
        PlayNextEnemyTurn();
    }

    private void PlayNextEnemyTurn()
    {
        if (isEnemyTurnStarted)
        {
            Debug.Log("이미 적이 행동 중인데 실행이 되고 있습니다.");
            return;
        }

        if (enemyQueueCount == 0)
        {
            Debug.Log("enemy Queue 에 아무것도 없는데 실행이 되고 있습니다. 확인이 필요합니다");
            WaitEnemyAttackDelay();
            return;
        }

        isEnemyTurnStarted = true;

        // 큐 데이터를 꺼낸다
        EnemyTurnData turnData = enemyQueue.Dequeue();

        // 캐스터가 죽어있으면 스킵한다.
        if (turnData.casterEnemy == null || turnData.casterEnemy.isDead)
        {
            StartCoroutine(WaitEnemyAttackDelay());
            return;
        }

        // 만약 버프면 바로 코루틴으로
        if (turnData.enemy_Behaviour == Enemy_Behaviour.Buff)
        {
            turnData.casterEnemy.StartBehaviour();
            StartCoroutine(WaitEnemyAttackDelay());
        }
        else
        {
            // 아니면 공격을 시작한다.
            RunEnemyAttack(turnData.casterEnemy);
        }
    }

    private void RunEnemyAttack(EnemyBase caster)
    {
        // 데미지 받을 때 다음 단계로 넘어가므로, 여기서 구독한다.
        player.OnDamagedTaken -= EnemyAttackCompleted;
        player.OnDamagedTaken += EnemyAttackCompleted;

        // 공격 시작
        caster.StartBehaviour();
    }

    private void EnemyAttackCompleted(int damage)
    {
        // 바로 구독을 해제해준다
        player.OnDamagedTaken -= EnemyAttackCompleted;

        // 대기시간을 가진다
        StartCoroutine(WaitEnemyAttackDelay());
    }

    private IEnumerator WaitEnemyAttackDelay()
    {
        // 기다린다
        yield return waitOnefSec;

        isEnemyTurnStarted = false;
        // 전투 결과를 확인하고 분기로 나눈다
        switch (CheckEnemyBattleState())
        {
            case EnemyBattleState.PlayerIsDead:
                GoToStep(TurnState.GameOver);
                break;

            case EnemyBattleState.NoMoreEnemyQueue:
                GoToStep(TurnState.EnemyTurnEnd);
                break;

            case EnemyBattleState.ContinueBattle:
                PlayNextEnemyTurn();
                break;
        }
    }


    //======================== Enemy Turn End 구간 ====================================================

    private void EnemyTurnEnd()
    {
        StartCoroutine(EnemyTurnEndDelay());
    }

    private IEnumerator EnemyTurnEndDelay()
    {
        yield return waitTwofSec;

        GoToStep(TurnState.EndRound);
    }


    //======================== End Round 구간 ==============================================================

    private void EndRound()
    {
        StartCoroutine(RoundResult());
    }

    private IEnumerator RoundResult()
    {
        yield return waitOnefSec;

        GoToStep(TurnState.StartNewRound);
    }

    //========================== End Battle 구간 =====================================================================

    private void EndBattle()
    {
        uIManager.OnSkillUpgradeCompleted += SkillUpgradeComplete;

        uIManager.ShowSkillupgrade();
    }

    public void SkillUpgradeComplete()
    {
        uIManager.OnVictoryBtnClicked += GotoNextScene;

        uIManager.ShowVictory();
    }

    private void GotoNextScene()
    {
        if (sceneIndex + 1 > SceneManager.sceneCountInBuildSettings - 1)
        {
            // 다음 씬이 없으면 타이틀 씬으로
            SceneManager.LoadScene(0);
        }

        // 다음씬으로 이동
        SceneManager.LoadScene(sceneIndex + 1);

    }

    //==================== Game Over 구간 ===========================================================================

    [SerializeField, BoxGroup("필드 값 추적"), ReadOnly]
    private bool GameOverDoOnce = false;
    private void GameOver()
    {
        if (GameOverDoOnce) return;

        GameOverDoOnce = true;
        uIManager.ShowGameOver();
        uIManager.OnGoToTitleBtnClicked += GoToTitle;
    }

    private void GoToTitle()
    {
        SceneManager.LoadScene(0);
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

        currentScene = SceneManager.GetActiveScene();
        sceneIndex = currentScene.buildIndex;
    }

    private IEnumerator WaitForStartGame()
    {
        yield return waitOnefSec;

        InitializeBattle();

        uIManager.FadeDarkImage(0.5f);

        yield return waitHalfSec;

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

    private void SubscribePlayerCompleteAttack()
    {
        playerCombat.SubEventByEffect(PlayerCompleteAttack);
    }

    private void UnSubscribePlayerCompleteAttack()
    {
        playerCombat.UnSubEventByEffect(PlayerCompleteAttack);
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
