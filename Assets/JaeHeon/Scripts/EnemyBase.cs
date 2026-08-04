using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //가중치를 통한 행동 변경 
    //행동 목록 enum
    //적 타입 enum
    //가중치는 적에게 개별 적용
    //적들 필요한 필드 내용 : 몬스터타입, 체력, 방어력, 가중치, 현재 할 행동 내역, 
    [SerializeField] EnemyAnimation ani;
    [SerializeField] EnemySelectBehaviour behaviour;

    [BoxGroup("스타트에서 Transform 참조됩니다.")]
    [SerializeField] Transform playerTransform;

    [BoxGroup("적 초기스탯")]
    public int maxHp { get; private set; }
    [BoxGroup("적 초기스탯"), SerializeField]
    private float attackPower;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float defencePower;
    [BoxGroup("적 초기스탯"), SerializeField]
    private List<float> attackWeights;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float buffWeight;

    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public bool isDead { get; private set; } = false;
    private Enemy_Behaviour currentBehaviour;


    /// <summary>
    /// 
    /// </summary>
    public event Action OnDie;
    public Action<int, int> OnTakeDamage;


    private void Awake()
    {
        currentHp = maxHp;
        OnDie += Die;
        Debug.Log($"현재 HP : {currentHp} / maxHP : {maxHp}  / attack : {attackPower} / defence : {defencePower}");
    }

    private void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerBaseStat>().transform;
    }

    public List<float> GetAttackWeights()
    {
        return attackWeights;
    }
    public float GetBuffWeight()
    {
        return buffWeight;
    }

    public void SelectBehaviour()
    {
        currentBehaviour = behaviour.Calc_Enemy_Behaviour();
    }

    //추후에 턴 넘어왔을 때 currentBehaviour = behaviour.Calc_Enemy_Behaviour(); 해주시면 어떤 행동 할 지 가져오게 됩니다.
    // 이후 아래 if문처럼 스킬, 공격, 버프 불러주시면 됩니다.
    private void StartBehaviour()
    {
        //currentBehaviour = behaviour.Calc_Enemy_Behaviour();

        if(currentBehaviour == Enemy_Behaviour.Attack)
        {
            NormalAttack();
        }
        else if(currentBehaviour == Enemy_Behaviour.Skill1)
        {
            Skill1();
        }
        else if(currentBehaviour == Enemy_Behaviour.Skill2)
        {
            Skill2();
        }
        else if(currentBehaviour == Enemy_Behaviour.Skill3)
        {
            Skill3();
        }
        else if(currentBehaviour == Enemy_Behaviour.Skill4)
        {
            Skill4();
        }
        else if(currentBehaviour == Enemy_Behaviour.Buff)
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
        ani.EnemyAttack();
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

    [Button]
    public void TakeDamage(int amount)
    {
        if (amount > 0)
        {
            currentHp = Math.Clamp(currentHp, 0, currentHp - amount);
            OnTakeDamage?.Invoke(currentHp, amount);
        }
        if(currentHp == 0)
        {
            OnDie?.Invoke();
        }
    }
}
