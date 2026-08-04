using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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
    [SerializeField] EnemyAnimation ani;
    [SerializeField] EnemySelectBehaviour behaviour;

    [BoxGroup("스타트에서 Transform 참조됩니다")]
    public Transform playerTransform;
    private PlayerBaseStat player;

    [BoxGroup("적 초기스탯"), SerializeField]
    private int _maxHp;
    public int maxHp { get => _maxHp; }
    [BoxGroup("적 초기스탯"), SerializeField]
    private int attackPower;
    
    private int defencePower;
    [BoxGroup("적 초기스탯"), SerializeField]
    private List<BehaviorData> behaviourListData;
    //[BoxGroup("적 초기스탯"), SerializeField]
    //private List<float> attackWeights;
    //[BoxGroup("적 초기스탯"), SerializeField]
    //private float buffWeight;

    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public bool isDead { get; private set; } = false;
    private Enemy_Behaviour currentBehaviour;

    public event Action OnDie;
    public Action<EnemyBase, int, int> OnTakeDamage;


    private void Awake()
    {
        currentHp = maxHp;
        OnDie += Die;
        Debug.Log($"현재 HP : {currentHp} / maxHP : {maxHp}  / attack : {attackPower} / defence : {defencePower}");
        ani = GetComponent<EnemyAnimation>();
        behaviour = GetComponent<EnemySelectBehaviour>();
    }
    private void OnDisable()
    {
        OnDie -= Die;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerBaseStat>();
        playerTransform = player.transform;
    }

    public List<BehaviorData> GetListData()
    {
        return behaviourListData;
    }

    //public List<float>GetAttackWeights()
    //{
    //    return attackWeights;
    //}
    //public float GetBuffWeight()
    //{
    //    return buffWeight;
    //}


    //추후에 턴 넘어왔을 때 currentBehaviour = behaviour.Calc_Enemy_Behaviour(); 해주시면 어떤 행동 할 지 가져오게 됩니다.
    // 이후 아래 if문처럼 스킬, 공격, 버프 불러주시면 됩니다.
    public void SelectBehaviour()
    {
        currentBehaviour = behaviour.Calc_Enemy_Behaviour();
    }
    private void StartBehaviour()
    {
        //currentBehaviour = behaviour.Calc_Enemy_Behaviour();

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
        Debug.Log($"{gameObject.name} >> EnemyDie");
        ani.EnemyDie();
    }

    [Button] // 방어력 만큼 현재 피해에서 감쇠 한 다음 hp 계산
    public void TakeDamage(int amount)
    {
        if (amount > 0)
        {
            ani.EnemyTakeDamage();
            currentHp = Math.Clamp(currentHp, 0, currentHp - (amount -= defencePower));
            OnTakeDamage?.Invoke(this, currentHp, (amount));
        }
        else
        {
            Debug.Log("amount <= 값이 들어왔습니다.");
        }
        if (currentHp == 0)
        {
            OnDie?.Invoke();
        }
    }

    public void ApplyDamage()
    {
        player.TakeDamage(attackPower);
    }
}
