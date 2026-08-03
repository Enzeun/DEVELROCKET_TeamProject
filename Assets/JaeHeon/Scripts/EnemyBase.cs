using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //가중치를 통한 행동 변경 
    //행동 목록 enum
    //적 타입 enum
    //가중치는 적에게 개별 적용
    //적들 필요한 필드 내용 : 몬스터타입, 체력, 방어력, 가중치, 현재 할 행동 내역, 

    [BoxGroup("적 초기스탯"), SerializeField]
    private int maxHp;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float attackPower;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float defencePower;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float attackWeight;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float defenceWeight;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float buffWeight;

    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public int currentHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public bool isDead { get; private set; } = false;

    public event Action OnDie;


    private void Awake()
    {
        currentHp = maxHp;
        OnDie += EnemyDie;
        Debug.Log($"현재 HP : {currentHp} / maxHP : {maxHp}  / attack : {attackPower} weight : {attackWeight} / defence : {defencePower}");
    }

    public float GetAttackWeight()
    {
        return attackWeight;
    }
    public float GetDefenceWeight()
    {
        return defenceWeight;
    }
    public float GetBuffWeight()
    {
        return buffWeight;
    }

    [Button]
    private void EnemyDie() 
    {
        Debug.Log($"{gameObject.name} >> EnemyDie");
    }

    [Button]
    public void TakeDamage(int amount)
    {
        currentHp  =  Math.Clamp(currentHp, 0, currentHp - amount);

        if(currentHp == 0)
        {
            OnDie?.Invoke();
        }
    }
}
