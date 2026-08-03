using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySetValue : MonoBehaviour
{
    [SerializeField] private EnemyInfo enemyInfo;
    private Dictionary<Enemy_Behaviour, int> enemyBehaviourValue = new Dictionary<Enemy_Behaviour, int>();
    private Enemy_Behaviour finalValue;



    private void Awake()
    {
        SetValue_EnemyBehaviour();
    }
    /// <summary>
    /// Scriptable Object로 만든 데이터를 가져와서 Dictionary에 저장합니다.
    /// </summary>
    private void SetValue_EnemyBehaviour()
    {
        SetWeight(Enemy_Behaviour.Attack, enemyInfo.attackWeight);
        SetWeight(Enemy_Behaviour.Defence, enemyInfo.defenceWeight);
        SetWeight(Enemy_Behaviour.Buff, enemyInfo.buffweight);
    }
    /// <summary>
    /// Dictionary에 저장된 값을 불러와서 범위를 지정해 해당 범위 내 값을 토대로 enemy의 행동을 정해줍니다.
    /// </summary>
    /// <returns></returns>
    [ContextMenu("범위 측정 및 계산")]
    private Enemy_Behaviour Calc_Enemy_Behaviour()
    {
        int sumWeight = 0;
        foreach(var weight in enemyBehaviourValue)
        {
            sumWeight += weight.Value;
        }
        //랜덤 값 뒤에 +1 해준 이유는 int형으로 random을 돌렸을 때 max값은 제외 되고 min값은 포함 된 채로 계산되어 나와서 +1을 해줘 max값도 포함되게 한다.
        int behaviourRange = Random.Range(1, sumWeight + 1);
        int attackValue = GetWeight(Enemy_Behaviour.Attack);
        int defenceValue = GetWeight(Enemy_Behaviour.Defence);
        //int buffValue = GetWeight(Enemy_Behaviour.Buff);
        Debug.Log($"랜덤 값 범위 : 1~{sumWeight}");
        Debug.Log($"랜덤값? : {behaviourRange} / 공격값 :  0 ~ {attackValue} / 방어 값 : {attackValue} ~ {attackValue + defenceValue} / 버프 값 : {attackValue + defenceValue} ~");

        if(behaviourRange >= 1 && behaviourRange <= attackValue)
        {
            finalValue = Enemy_Behaviour.Attack;
        }
        else if( behaviourRange > attackValue && behaviourRange <= attackValue + defenceValue)
        {
            finalValue = Enemy_Behaviour.Defence;
        }
        else
        {
            finalValue = Enemy_Behaviour.Buff;
        }
        Debug.Log($"계산한 행동 값 : {finalValue}");

        return finalValue;
    }


    //Dictionary에 저장하기 위한 함수
    private void SetWeight(Enemy_Behaviour behaviour, int value)
    {
        enemyBehaviourValue.Add(behaviour, value);
    }
    //Dictionary에서 읽어오기 위한 함수
    private int GetWeight(Enemy_Behaviour behaviour)
    {
        enemyBehaviourValue.TryGetValue(behaviour, out int value);
        return value;
    }

    private void EnemyTurn()
    {
        Calc_Enemy_Behaviour();
    }
}
