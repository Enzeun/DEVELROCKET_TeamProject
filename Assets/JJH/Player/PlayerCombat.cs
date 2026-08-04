using System;
using System.Runtime.InteropServices;
using UnityEngine;
using SF = UnityEngine.SerializeField;
public class PlayerCombat : MonoBehaviour
{
    private static readonly int IsSkillSelectHash = Animator.StringToHash("IsSkillSelect");
    private static readonly int IsRunHash = Animator.StringToHash("IsRun");
    private static readonly int IsDeathHash = Animator.StringToHash("IsDeath");
    private static readonly int IsWeakHash = Animator.StringToHash("IsWeak");
    private static readonly int IsStrongHash = Animator.StringToHash("IsStrong");


    [SF] private SkillEffectSpawner spawner;
    [SF] private Transform DummyTarget;
    [SF] private Animator playerAnimator;
    // 플레이어 기본 스탯은 임시로 상수
    [HideInInspector]public PlayerBaseStat player;

    private Transform nowTarget;
    private SkillBaseStat nowSkillData;

    private bool damagedTakenSub = false;
    private bool deadSub = false;

    private void OnEnable()
    {
        SubEvent();
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnDamagedTaken -= PlayerHit;
            player.OnDead -= PlayerDeath;
        }

        if(spawner != null)
        {
            spawner.OnEffectFinished -= EffectEnd;
        }

        damagedTakenSub = false;
        deadSub = false;
    }

    private void Start()
    {
        PlayerCombatStatInit();
    }

    /// <summary>
    /// 전투를 위한 플레이어 데이터 생성
    /// </summary>
    private void PlayerCombatStatInit()
    {
        // 임시 데이터, 데이터 로드 방식 추가시 변경
        //string name, int maxHP, int nowHP, int maxCost, int nowCost,
        /*int atkPoint, int defPoint,
        Dictionary< int, SkillBaseStat > skillData*/
        player = new ("베이스", 200, 200, 6, 6, 10, 5, SkillData.BaseSkillData);

        if (playerAnimator.GetBool(IsDeathHash)) playerAnimator.SetBool(IsDeathHash, false);

        SubEvent();
    }

    /// <summary>
    /// 현재 사용할 스킬과 타겟 데이터 지정
    /// </summary>
    /// <param name="data"></param>
    /// <param name="target"></param>
    public void SetNowSkillAndTarget(SkillBaseStat data, Transform target)
    {
        // 원랜 아래를 호출해야함
        nowSkillData = data;
        nowTarget = target;
    }


    /// <summary>
    /// 플레이어 대기 자세 호출
    /// </summary>
    public void PlayerActiveIdle()
    {
        if (playerAnimator.GetBool(IsRunHash))
            playerAnimator.SetBool(IsRunHash, false);
        if(playerAnimator.GetBool(IsSkillSelectHash))
            playerAnimator.SetBool(IsSkillSelectHash, false);
    }

    /// <summary>
    /// 플레이어 달리기 자세 호출
    /// </summary>
    public void PlayerActiveRun()
    {
        if (!playerAnimator.GetBool(IsRunHash))
            playerAnimator.SetBool(IsRunHash, true);
    }

    /// <summary>
    /// 플레이어 스킬 대기 자세 호출
    /// </summary>
    public void PlayerActiveSkillSelect()
    {
        if (!playerAnimator.GetBool(IsSkillSelectHash))
            playerAnimator.SetBool(IsSkillSelectHash, true);
    }

    /// <summary>
    /// 플레이어가 피격당할 때의 자세 이벤트 구독 함수
    /// (피해량이 최대 체력의 20%보다 약하면 waek, 20%보다 강하면 strong 호출)
    /// </summary>
    /// <param name="damage"></param>
    public void PlayerHit(int damage)
    {
        if(damage < player.MaxHP / 5f)
            playerAnimator.SetTrigger(IsWeakHash);

        else
            playerAnimator.SetTrigger(IsStrongHash);
    }

    /// <summary>
    /// 플레이어 사망시 호출
    /// </summary>
    public void PlayerDeath()
    {
        playerAnimator.SetTrigger(IsDeathHash);
    }

    /// <summary>
    /// 플레이어 애니메이션 호출
    /// 
    /// <para>SetNowSkillAndTarget() 함수로 nowSkillData를 먼저 지정해야함</para>
    /// </summary>
    public void PlayerAnmationActive()
    {
        if(nowSkillData != null)
        playerAnimator.Play(nowSkillData.Pose.ToString());
    }

    /// <summary>
    /// 테스트용
    /// </summary>
    /// <param name="id"></param>
    public void EffectActiveTest(int id)
    {
        nowSkillData = SkillData.BaseSkillData[id];
        nowTarget = DummyTarget;
        playerAnimator.Play(nowSkillData.Pose.ToString());
    }

    public void EffectActive()
    {
        nowTarget = DummyTarget;
        
        if (nowSkillData != null && nowTarget != null)
            spawner.SpawnEffect(nowSkillData.Name, nowSkillData.Pose, nowTarget);
    }

    public void EffectEnd()
    {
        
    }

    /// <summary>
    /// 이벤트 구독 함수, 이벤트 추가시 계속 이곳에
    /// </summary>
    private void SubEvent()
    {
        if (player == null) return;
        if (spawner == null) return;

        if (!damagedTakenSub)
        {
            player.OnDamagedTaken += PlayerHit;
            damagedTakenSub = true;
        }

        if (!deadSub)
        {
            player.OnDead += PlayerDeath;
            deadSub = true;
        }

        spawner.OnEffectFinished += EffectEnd;
    }
}
