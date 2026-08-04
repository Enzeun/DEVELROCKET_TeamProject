using System;
using System.Runtime.InteropServices;
using UnityEngine;
using SF = UnityEngine.SerializeField;
public class PlayerCombat : MonoBehaviour
{
    private static readonly int IsDeathHash = Animator.StringToHash("IsDeath");
    private static readonly int IsWeakHash = Animator.StringToHash("IsWeak");
    private static readonly int IsStrongHash = Animator.StringToHash("IsStrong");
    private static readonly int IsSkillSelectHash = Animator.StringToHash("IsSkillSelect");
    private static readonly int IsRunHash = Animator.StringToHash("IsRun");
    [SF] private SkillEffectSpawner spawner;
    [SF] private Transform DummyTarget;
    [SF] private Animator playerAnimator;
    // 플레이어 기본 스탯은 임시로 상수
    [HideInInspector] public PlayerBaseStat player;

    private Transform nowTarget;
    private SkillBaseStat nowSkillData;

    private void OnEnable()
    {
        player.OnDamagedTaken += PlayerHit;
        player.OnDead += PlayerDeath;
    }

    private void OnDisable()
    {
        player.OnDamagedTaken -= PlayerHit;
        player.OnDead -= PlayerDeath;
    }

    private void Start()
    {
        PlayerCombatStatInit();
    }

    private void PlayerCombatStatInit()
    {
        // 임시 데이터, 데이터 로드 방식 추가시 변경
        //string name, int maxHP, int nowHP, int maxCost, int nowCost,
        /*int atkPoint, int defPoint,
        Dictionary< int, SkillBaseStat > skillData*/
        player = new ("베이스", 200, 200, 6, 6, 10, 5, SkillData.BaseSkillData);

        if (playerAnimator.GetBool(IsDeathHash)) playerAnimator.SetBool(IsDeathHash, false);
    }

    public void SetNowSkillAndTarget(SkillBaseStat data, Transform target)
    {
        // 원랜 아래를 호출해야함
        nowSkillData = data;
        nowTarget = target;
    }

    public void PlayerActiveIdle()
    {
        if (playerAnimator.GetBool(IsRunHash))
            playerAnimator.SetBool(IsRunHash, false);
        if(playerAnimator.GetBool(IsSkillSelectHash))
            playerAnimator.SetBool(IsSkillSelectHash, false);
    }

    public void PlayerActiveRun()
    {
        if (!playerAnimator.GetBool(IsRunHash))
            playerAnimator.SetBool(IsRunHash, true);
    }

    public void PlayerActiveSkillSelect()
    {
        if (!playerAnimator.GetBool(IsSkillSelectHash))
            playerAnimator.SetBool(IsSkillSelectHash, true);
    }

    public void PlayerHit(int damage)
    {
        if(damage < player.MaxHP / 5f)
            playerAnimator.SetTrigger(IsWeakHash);

        else
            playerAnimator.SetTrigger(IsStrongHash);
        
    }

    public void PlayerDeath()
    {
        playerAnimator.SetBool(IsDeathHash, true);
    }

    public void EffectActiveTest(int id)
    {
        nowSkillData = SkillData.BaseSkillData[id];
        nowTarget = DummyTarget;
        playerAnimator.Play(nowSkillData.Pose.ToString());
    }

    public void PlayerAnmationActive()
    {
        playerAnimator.Play(nowSkillData.Pose.ToString());
    }

    public void EffectActive()
    {
        nowSkillData = SkillData.BaseSkillData[1001];
        playerAnimator.Play(nowSkillData.Pose.ToString());
        nowTarget = DummyTarget;
        if(nowSkillData != null && nowTarget != null)
            spawner.SpawnEffect(nowSkillData.Name, nowSkillData.Pose, nowTarget);
    }
}
