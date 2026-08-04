using System.Collections.Generic;
using UnityEngine;
enum Enemy_Behaviour
{
    Attack,
    Skill1,
    Skill2,
    Skill3,
    Buff
}

public class EnemySelectBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;
    //Enemy의 행동에 따른 가중치를 Dictionary로 저장
    private Dictionary<Enemy_Behaviour, float> enemyBehaviourValue = new Dictionary<Enemy_Behaviour, float>();
    //가중치 범위 계산 후 결과값 저장
    [SerializeField] private Enemy_Behaviour finalValue;



    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
        SetValue_EnemyBehaviour();
    }



    /// <summary>
    /// Scriptable Object로 만든 데이터를 가져와서 Dictionary에 저장합니다.
    /// </summary>
    private void SetValue_EnemyBehaviour()
    {
        List<float> aw = enemyBase.GetAttackWeights();

        float sumAttackWeight = 0f;

        for (int i = 0; i < aw.Count; i++)
        {
            sumAttackWeight = aw[i];
        }
        SetWeight(Enemy_Behaviour.Attack, sumAttackWeight);
        SetWeight(Enemy_Behaviour.Buff, enemyBase.GetBuffWeight());

    }


    /// <summary>
    /// Dictionary에 저장된 값을 불러와서 범위를 지정해 해당 범위 내 값을 토대로 enemy의 행동을 정해줍니다.
    /// </summary>
    /// <returns></returns>
    [ContextMenu("범위 측정 및 계산")]
    private Enemy_Behaviour Calc_Enemy_Behaviour()
    {
        float sumWeight = 0;

        foreach(var weight in enemyBehaviourValue)
        {
            sumWeight += weight.Value;
        }

        float behaviourRange = Random.Range(1, sumWeight);
        float buffValue = sumWeight - GetWeight(Enemy_Behaviour.Buff);

        Debug.Log($"랜덤 값 범위 : 1 ~ {sumWeight}");
        Debug.Log($"랜덤값? : {behaviourRange} / 공격 값 :  0 ~ {sumWeight} / 버프 값 : {sumWeight} ~ {behaviourRange}");


        if(behaviourRange > buffValue && behaviourRange <= sumWeight)
        {
            finalValue = Enemy_Behaviour.Buff;
        }
        else
        {
            float remainValue = sumWeight - buffValue;
            List<float> aw = enemyBase.GetAttackWeights();
            for (int i = 0 ; i < aw.Count; i--)
            {
                if(behaviourRange > remainValue - aw[i] && behaviourRange <= remainValue)
                {
                    finalValue = Enemy_Behaviour.Attack;
                }
                remainValue -= aw[i];
            }
        }
                    //잘못됨
        //return 할 때 i 값도 같이 넘겨서 attackweight의 i번째를 사용하겠다는 느낌?


        if(behaviourRange >= 1 && behaviourRange < buffValue)
        {
            finalValue = Enemy_Behaviour.Attack;
        }
        else
        {
            finalValue = Enemy_Behaviour.Buff;
        }
        Debug.Log($"계산한 행동 값 : {finalValue}");

        return finalValue;
    }


    //Dictionary에 저장하기 위한 함수
    private void SetWeight(Enemy_Behaviour behaviour, float value)
    {
        enemyBehaviourValue.Add(behaviour, value);
    }
    //Dictionary에서 읽어오기 위한 함수
    private float GetWeight(Enemy_Behaviour behaviour)
    {
        enemyBehaviourValue.TryGetValue(behaviour, out float value);
        return value;
    }

    private void StartEnemyTurn()
    {
        Calc_Enemy_Behaviour();
    }

    private void EndTurn()
    {

    }
}
