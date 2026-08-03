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
    private float maxHp;
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
    public float currentEnemyHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector, ReadOnly]
    public bool isDead { get; private set; } = false;

    public event Action OnEnemyDie;


    private void Awake()
    {
        currentEnemyHp = maxHp;
        OnEnemyDie += EnemyDie;
        Debug.Log($"현재 HP : {currentEnemyHp} / maxHP : {maxHp}  / attack : {attackPower} weight : {attackWeight} / defence : {defencePower}");
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
    public void TakeDamage(float amount)
    {
        currentEnemyHp  =  Math.Clamp(currentEnemyHp, 0, currentEnemyHp - amount);

        if(currentEnemyHp == 0)
        {
            OnEnemyDie?.Invoke();
        }
    }
}
