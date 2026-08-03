using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;


enum Enemy_Behaviour
{
    None,
    Attack,
    Buff,
    Defence
}
struct Weight
{
    int attackWeight;
    int buffWeight;
    int defenceWeight;
}

public class EnemyBase : MonoBehaviour
{
    //가중치를 통한 행동 변경 
    //행동 목록 enum
    //적 타입 enum
    //가중치는 적 타입에 따라 정해지게끔


    //적들 필요한 필드 내용 : 몬스터타입, 체력, 방어력, 가중치, 현재 할 행동 내역, 
    private Enemy_Type enemyType;
    private int enemyHp;
    private int enemyDefence;
    private Enemy_Behaviour currentEnemyBehaviour;
    
    //몬스터가 살았는지 죽었는지도 여기서 판단할거같아보이긴한데
    //죽었을 때 event로 넘기기

    //

    
}
