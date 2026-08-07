using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_Behaviour
{
    None,
    NormalAttack,
    Shoot,//Skill1
    Spell,//Skill2
    Skill3,
    Skill4,
    Buff
}
//public enum EnemyBuffType
//{
//    AttackPointBuff,
//    DefencePointBuff,
//    HpBuff,
//    NoneBuff
//}
public enum EnemyType
{
    Scorpion,
    Plant,
    Golem,
    Worm,
    Boss
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
    EnemyAnimation ani;
    EnemyBuff eBuff;
    EnemySelectBehaviour behaviour;


    [BoxGroup("적 초기스탯"), SerializeField]
    private int _maxHp; 
    [BoxGroup("적 초기스탯"), SerializeField]
    private int maxBuffCount = 3;
    public int maxBuffCount_ { get => maxBuffCount; }
    public int maxHp { get => _maxHp; }
    [BoxGroup("적 초기스탯"), SerializeField]
    private int attackPoint;
    [BoxGroup("적 초기스탯"), SerializeField]
    private int defencePoint;

    [BoxGroup("적 스킬 가중치"), SerializeField]
    private List<BehaviorData> behaviourListData;

    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentAttackPoint { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentDefencePoint { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int buffStack { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public bool isDead { get; private set; } = false;


    [BoxGroup("추적이 필요한 필드"), ShowInInspector, ReadOnly]
    public Transform playerTransform;
    [BoxGroup("추적이 필요한 필드"), ShowInInspector, ReadOnly]
    private PlayerCombat playerCombat;
    [BoxGroup("추적이 필요한 필드"), ShowInInspector, ReadOnly]
    private Enemy_Behaviour _currentBehaviour;
    public Enemy_Behaviour currentBehaviour { get => _currentBehaviour; }

    public Transform hpBarLocation;
    
    public event Action<EnemyBase> OnDie;
    public Action<EnemyBase, int, int> OnTakeDamage;
    private PlayerBaseStat playerStat;

    private void Awake()
    {
        ani = GetComponent<EnemyAnimation>();
        behaviour = GetComponent<EnemySelectBehaviour>();
        eBuff = GetComponent<EnemyBuff>();
        InitStat();
    }

    private void InitStat()
    {
        currentHp = maxHp;
        currentAttackPoint = attackPoint;
        currentDefencePoint = defencePoint;
        buffStack = 0;
    }


    public void InitEnemyTarget(PlayerCombat _playerCombat)
    {
        if (_playerCombat == null)
        {
            Debug.Log("_playerCombat 이 null 임. 확인 필요!! **********");
        }

        playerCombat = _playerCombat;

        playerTransform = playerCombat.transform;

        playerStat = playerCombat.player;

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
            _currentBehaviour = behaviour.Calc_Enemy_Behaviour();
        }
    }
    public void StartBehaviour()
    {
        if (isDead)
            return;

        if (_currentBehaviour == Enemy_Behaviour.NormalAttack)
        {
            NormalAttack();
        }
        else if (_currentBehaviour == Enemy_Behaviour.Shoot)
        {
            Skill1();
        }
        else if (_currentBehaviour == Enemy_Behaviour.Spell)
        {
            Skill2();
        }
        else if (_currentBehaviour == Enemy_Behaviour.Skill3)
        {
            Skill3();
        }
        else if (_currentBehaviour == Enemy_Behaviour.Skill4)
        {
            Skill4();
        }
        else if (_currentBehaviour == Enemy_Behaviour.Buff)
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
        buffStack++;
        eBuff.CheckBuff();
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
        if (isDead)
            return;

        if (amount > 0)
        {
            ani.EnemyTakeDamage();

            amount = Math.Max(0, amount - currentDefencePoint);

            currentHp = Math.Clamp((currentHp - amount), 0, maxHp);

            OnTakeDamage?.Invoke(this, currentHp, amount);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void ApplyChangeStat(EnemyType type, int amount)
    {
        switch(type)
        {
            case EnemyType.Scorpion:
                currentAttackPoint += amount;
                break;
            case EnemyType.Golem:
                currentDefencePoint += amount;
                break;
            case EnemyType.Plant:
                currentDefencePoint += amount;
                    break;
            case EnemyType.Worm:
                break;
            case EnemyType.Boss:
                currentAttackPoint += amount;
                currentDefencePoint += amount;
                break;
        }
        ani.EnemyBuff();
    }
    //public void ApplyChangeStat(EnemyBuffType type, int amount)
    //{
    //    switch(type)
    //    {
    //        case EnemyBuffType.AttackPointBuff:
    //            currentAttackPoint += amount;
    //            break;
    //        case EnemyBuffType.DefencePointBuff:
    //            currentDefencePoint += amount;
    //            break;
    //        case EnemyBuffType.HpBuff:
    //            currentHp = Math.Min(currentHp += amount, maxHp);
    //                break;
    //        case EnemyBuffType.NoneBuff:
    //            break;
    //    }
    //    buffStack++;
    //    ani.EnemyBuff();
    //}

    public void ApplyDamage()
    {
        if (playerStat == null)
        {
            playerStat = playerCombat.player;
        }
        playerStat.TakeDamage(currentAttackPoint);
    }

    public int GetAttackPoint()
    {
        return attackPoint;
    }

    //적 소환
    public void SpawnEnemy()
    {
        ani.EnemySpawn();
    }
}
