using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public enum Enemy_Type
{
    //몬스터 이름으로 타입을 나눌 예정 ( 현재 프로토타입으로 가명 넣어둠 )
    None,
    Priority_Attack_Type,
    Priority_Buff_Type,
    Priority_Defence_Type
}

enum Enemy_Behaviour
{
    None,
    Attack,
    Buff,
    Defence
}


public class EnemyBase : MonoBehaviour
{
    //가중치를 통한 행동 변경 
    //행동 목록 enum
    //적 타입 enum
    //가중치는 적 타입에 따라 정해지게끔
    //적들 필요한 필드 내용 : 몬스터타입, 체력, 방어력, 가중치, 현재 할 행동 내역, 

    [SerializeField] private EnemyInfo enemyInfo = new EnemyInfo();

    [BoxGroup("적 초기스탯"), SerializeField]
    private float maxHp;
    [BoxGroup("적 초기스탯"), SerializeField]
    private float attack;

    [BoxGroup("적 현재스탯"), ShowInInspector,ReadOnly]
    public float currentHp { get; private set; }
    [BoxGroup("적 현재스탯"), ShowInInspector,ReadOnly]
    public bool isDead { get; private set; } = false;


    private int currentEnemyHp;
    public int EnemyHp
    {
        get => currentEnemyHp;
        private set
        {
            currentEnemyHp = Mathf.Clamp(value, 0, enemyMaxHp);
            if(currentEnemyHp <= 0)
            {
                OnEnemyDie?.Invoke();
            }
        }
    }
    public int enemyMaxHp
    {
        get => enemyMaxHp;
        set
        {
            value = enemyInfo.maxHp;
        }
    }
    public int enemyDefence
    {
        get => enemyDefence;
        set
        {
            value = enemyInfo.defence;
        }
    }

    public event Action OnEnemyDie;


    private void Awake()
    {
        OnEnemyDie += EnemyDie;
        currentEnemyHp = enemyInfo.hp;

        Debug.Log($"현재 HP : {currentEnemyHp} / maxHP : {enemyMaxHp}  / defence : {enemyDefence}");
    }
    [ContextMenu("hp0")]    
    
    public void EnemyHP0()
    {
        currentEnemyHp = 0;
    }

    [Button]
    private void EnemyDie()
    {
        Debug.Log($"{gameObject.name} >> EnemyDie");
    }
}
