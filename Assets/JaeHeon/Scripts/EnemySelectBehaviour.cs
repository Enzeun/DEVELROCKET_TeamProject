using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemySelectBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;
    //Enemy의 행동에 따른 가중치를 Dictionary로 저장
    private Dictionary<Enemy_Behaviour, float> enemyBehaviourValue = new Dictionary<Enemy_Behaviour, float>();
    //가중치 범위 계산 후 결과값 저장
    [SerializeField] private Enemy_Behaviour finalValue;
    [SerializeField] private Enemy_Behaviour currentBehaviour;

    private List<BehaviorData> enemyBehaviourListData;

    float sumWeight = 0f;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
        enemyBehaviourListData = enemyBase.GetListData();
    }

    private void Start()
    {
        SetValue_EnemyBehaviour();
    }



    /// <summary>
    /// Scriptable Object로 만든 데이터를 가져와서 Dictionary에 저장합니다.
    /// </summary>
    private void SetValue_EnemyBehaviour()
    {
        sumWeight = 0f;

        for (int i = 0; i < enemyBehaviourListData.Count; ++i)
        {
            var data = enemyBehaviourListData[i];
            SetWeight(data.Type, data.Weight);
            sumWeight += data.Weight;
        }
    }

    /// <summary>
    /// Dictionary에 저장된 값을 불러와서 범위를 지정해 해당 범위 내 값을 토대로 enemy의 행동을 정해줍니다.
    /// 범위를 역산했기 때문에 20~ buffWeight 까지 버프 20-buffWeight 부터 Attak 만큼 Attack 또 그에서 뺀 값에서 skill_1만큼 뺀 값이 skill_1입니다.
    /// </summary>
    /// <returns></returns>
    [ContextMenu("범위 측정 및 계산")]
    public Enemy_Behaviour Calc_Enemy_Behaviour()
    {
        float behaviourRange = UnityEngine.Random.Range(1, sumWeight);
        float buffValue = GetWeight(Enemy_Behaviour.Buff);

        float remainValue = sumWeight - buffValue;

        List<float> getAw = new List<float>();
        foreach (var behaviorData in enemyBehaviourListData)
        {
            getAw.Add(behaviorData.Weight);
        }

        Debug.Log($"랜덤 값 범위 : 1 ~ {sumWeight}");
        Debug.Log($"랜덤값? : {behaviourRange} / 공격 값 :  0 ~ {remainValue} / 버프 값 : {remainValue} ~ {sumWeight}");





        if (behaviourRange > remainValue && behaviourRange <= sumWeight)
        {
            finalValue = Enemy_Behaviour.Buff;
        }
        else
        {
            for (int i = 0; i < getAw.Count; i++)
            {
                if (getAw[i] == 0)
                {
                    Debug.Log($"attackWeight{i}의 가중치 0");
                    //return Enemy_Behaviour.None;
                }
                else if (behaviourRange > remainValue - getAw[i] && behaviourRange <= remainValue)
                {
                    finalValue = enemyBehaviourListData[i].Type;
                    Debug.Log($"계산한 행동 값 : {finalValue}");
                    return finalValue;
                }
                remainValue -= getAw[i];
            }
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
        if (enemyBehaviourValue.TryGetValue(behaviour, out float value) == false)
            return -1.0f;
        
        return value;
    }
}
