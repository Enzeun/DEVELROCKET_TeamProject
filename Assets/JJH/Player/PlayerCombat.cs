using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static SkillEnums;
using SF = UnityEngine.SerializeField;
public class PlayerCombat : MonoBehaviour
{
    #region 애니메이션 클립 해시
    private static readonly int IsSkillSelectHash = Animator.StringToHash("IsSkillSelect");
    private static readonly int IsRunHash = Animator.StringToHash("IsRun");
    private static readonly int IsDeathHash = Animator.StringToHash("IsDeath");
    private static readonly int IsWeakHash = Animator.StringToHash("IsWeak");
    private static readonly int IsStrongHash = Animator.StringToHash("IsStrong");
    #endregion

    #region 직렬화 변수
    [Header("스크립트")]
    [SF] private SkillEffectSpawner spawner;

    [Header("기타")]
    [SF] private Transform DummyTarget;
    [SF] private Animator playerAnimator;

    #endregion

    #region 지역 변수
    // 플레이어 기본 스탯은 임시로 상수
    public PlayerBaseStat player;

    private Tween moveTween;

    private Transform[] nowTarget;
    private SkillBaseStat nowSkillData;

    private bool damagedTakenSub = false;
    private bool deadSub = false;
    private bool effectFinSub = false;

    private bool isEndSet = false;
    #endregion

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
        effectFinSub = false;
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

    

    public void PlayerMove(Transform to)
    {
        moveTween?.Kill();

        PlayerActiveRun();
        float distance = Vector3.Distance(transform.position, to.position);
        
        Vector3 direction = to.position - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;

        moveTween = transform
            .DOMove(to.position, distance / 4f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { PlayerActiveIdle(); moveTween = null; });
    }
    /// <summary>
    /// 현재 사용할 스킬과 타겟 데이터 지정
    /// </summary>
    /// <param name="data"></param>
    /// <param name="target"></param>
    public void SetNowSkillAndTarget(SkillBaseStat data, Transform[] target)
    {
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
        isEndSet = false;
    }

    /// <summary>
    /// 테스트용
    /// </summary>
    /// <param name="id"></param>
    public void EffectActiveTest(int id)
    {
        nowSkillData = SkillData.BaseSkillData[id];
        nowTarget = new Transform[1] { DummyTarget };
        playerAnimator.Play(nowSkillData.Pose.ToString());
    }

    public void EffectActive()
    {
        if (nowSkillData != null && nowTarget != null)
            spawner.SpawnEffect(nowSkillData.Name, nowSkillData.Pose, nowTarget);
    }

    /// <summary>
    /// 스킬이 적에게 적중했거나 적중시간이 되었을 경우 이벤트를 통해 호출되는 함수
    /// </summary>
    public void EffectEnd()
    {
        if(nowSkillData.TargetType == SkillTargetType.Single && !isEndSet)
        {
            isEndSet = true;

            SkillBaseStat skill = nowSkillData;

            if (nowTarget != null && nowTarget[0].TryGetComponent(out EnemyBase stat))
            {
                int damage = (player.AtkPoint * skill.SkillDamageCalcByUpgrade() + 50) / 100;

                stat.TakeDamage(damage);
            }
        }
        else if (nowSkillData.TargetType == SkillTargetType.Area && !isEndSet)
        {
            isEndSet = true;

            SkillBaseStat skill = nowSkillData;

            foreach (Transform t in nowTarget)
            {
                if (t != null && t.TryGetComponent(out EnemyBase stat))
                {
                    int damage = (player.AtkPoint * skill.SkillDamageCalcByUpgrade() + 50) / 100;
                    stat.TakeDamage(damage);
                }
            }
        }
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
        if (!effectFinSub)
        {
            spawner.OnEffectFinished += EffectEnd;
            effectFinSub = true;
        }
        
    }
}
