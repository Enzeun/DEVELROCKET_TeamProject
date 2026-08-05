using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Enemy_Behaviour
{
    None,
    Attack,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Buff
}

[Serializable]
public class BehaviorData
{
    [SerializeField] private Enemy_Behaviour BehaviorType;
    public Enemy_Behaviour Type => BehaviorType;

    [SerializeField] float Weight_;
    public float Weight => Weight_;
}

public class EnemyBase : MonoBehaviour
{
    //가중치를 통한 행동 변경 
    //행동 목록 enum
    //적 타입 enum
    //가중치는 적에게 개별 적용
    //적들 필요한 필드 내용 : 몬스터타입, 체력, 방어력, 가중치, 현재 할 행동 내역, 
    EnemyAnimation ani;
    EnemySelectBehaviour behaviour;


    [BoxGroup("적 초기스탯"), SerializeField]
    private int _maxHp;
    public int maxHp { get => _maxHp; }
    [BoxGroup("적 초기스탯"), SerializeField]
    private int attackPower;

    private int defencePower;
    [BoxGroup("적 초기스탯"), SerializeField]
    private List<BehaviorData> behaviourListData;

    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public bool isDead { get; private set; } = false;


    [BoxGroup("추적이 필요한 필드"), ShowInInspector, ReadOnly]
    public Transform playerTransform;
    [BoxGroup("추적이 필요한 필드"), ShowInInspector, ReadOnly]
    private PlayerCombat playerCombat;
    [BoxGroup("추적이 필요한 필드"), ShowInInspector, ReadOnly]
    private Enemy_Behaviour currentBehaviour;

    [SerializeField]
    Transform hpBarLocation;

    public event Action<EnemyBase> OnDie;
    public Action<EnemyBase, int, int> OnTakeDamage;
    private PlayerBaseStat playerStat;


    private void Awake()
    {
        currentHp = maxHp;
        Debug.Log($"현재 HP : {currentHp} / maxHP : {maxHp}  / attack : {attackPower} / defence : {defencePower}");
        ani = GetComponent<EnemyAnimation>();
        behaviour = GetComponent<EnemySelectBehaviour>();
    }

    private void Start()
    {
        SetTarget();
    }

    private void SetTarget()
    {
        if (playerCombat == null)
        {
            playerCombat = FindFirstObjectByType<PlayerCombat>();
            playerTransform = playerCombat.transform;
            playerStat = playerCombat.player;
        }
    }

    public List<BehaviorData> GetListData()
    {
        return behaviourListData;
    }

    //추후에 턴 넘어왔을 때 currentBehaviour = behaviour.Calc_Enemy_Behaviour(); 해주시면 어떤 행동 할 지 가져오게 됩니다.
    // 이후 아래 if문처럼 스킬, 공격, 버프 불러주시면 됩니다.
    public void SelectBehaviour()
    {
        if (!isDead)
        {
            currentBehaviour = behaviour.Calc_Enemy_Behaviour();
        }
    }
    private void StartBehaviour()
    {
        if (isDead)
            return;

        if (currentBehaviour == Enemy_Behaviour.Attack)
        {
            NormalAttack();
        }
        else if (currentBehaviour == Enemy_Behaviour.Skill1)
        {
            Skill1();
        }
        else if (currentBehaviour == Enemy_Behaviour.Skill2)
        {
            Skill2();
        }
        else if (currentBehaviour == Enemy_Behaviour.Skill3)
        {
            Skill3();
        }
        else if (currentBehaviour == Enemy_Behaviour.Skill4)
        {
            Skill4();
        }
        else if (currentBehaviour == Enemy_Behaviour.Buff)
        {
            Buff();
        }
        else
        {
            None();
        }
    }


    private void NormalAttack()
    {
        ani.EnemyNormalAttack(playerTransform);
    }
    //
    private void Skill1()
    {
        ani.EnemyShootProjectile(playerTransform);
    }
    private void Skill2()
    {
        ani.EnemyCastSpell(playerTransform);
    }
    private void Skill3()
    {
        Debug.Log("Null Skill");
    }
    private void Skill4()
    {
        Debug.Log("Null Skill");
    }
    private void Buff()
    {
        ani.EnemyBuff();
    }
    private void None()
    {
        Debug.Log("Behaviour >> None");
    }

    [Button]
    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} >> EnemyDie");
        ani.EnemyDie();
        OnDie?.Invoke(this);
    }

    //들어오는 방어력이 공격 피해량보다 높은지 확인 작업 필요
    [Button] // 방어력 만큼 현재 피해에서 감쇠 한 다음 hp 계산
    public void TakeDamage(int amount)
    {
        if (amount > 0)
        {
            ani.EnemyTakeDamage();
            amount = Math.Max(0, amount - defencePower);
            currentHp = Math.Clamp(currentHp, 0, currentHp - (amount));
            OnTakeDamage?.Invoke(this, currentHp, amount);
        }
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void ApplyDamage()
    {
        playerStat.TakeDamage(attackPower);
    }
}
