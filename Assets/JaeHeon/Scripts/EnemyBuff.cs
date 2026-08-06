using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBuff : MonoBehaviour
{
    private EnemyBase eBase;


    [BoxGroup("버프 타입 설정"), SerializeField]
    //private EnemyBuffType enemyBuffType;
    private EnemyType enemyType;
    [BoxGroup("버프 값 설정"), SerializeField]
    private int buffAmount;
    
    private void Awake()
    {
        eBase = GetComponent<EnemyBase>();
    }

    [Button("버프 수치 체크 및 적용")]
    public void CheckBuff()
    {
        if (eBase.buffStack >= eBase.maxBuffCount_)
        {
            RemoveBuffStack();
            Debug.Log("버프 스택이 최대치에 도달했습니다.");
        }
        else
        {
            DoBuff(buffAmount);
        }
    }

    public void RemoveBuffStack()
    {
        if (enemyType == EnemyType.Worm)
            return;

        var dataList = eBase.GetListData();
        var data = dataList.Find(a => a.Type == Enemy_Behaviour.Buff);

        dataList.Remove(data);
    }

    public void DoBuff(int amount)
    {
        if(enemyType == EnemyType.Worm)
            return;

        if (amount <= 0)
        {
            Debug.Log("0 <= 값이 들어왔음, 수치 조정 필요");
            return;
        }

        eBase.ApplyChangeStat(enemyType, amount);
    }
    //public void RemoveBuffStack()
    //{
    //    if (enemyBuffType == EnemyBuffType.NoneBuff)
    //        return;

    //    var dataList = eBase.GetListData();
    //    var data = dataList.Find(a => a.Type == Enemy_Behaviour.Buff);

    //    dataList.Remove(data);
    //}

    //public void DoBuff(int amount)
    //{
    //    if(enemyBuffType == EnemyBuffType.NoneBuff)
    //        return;

    //    if (amount <= 0)
    //    {
    //        Debug.Log("수치 조정 필요");
    //        return;
    //    }

    //    eBase.ApplyChangeStat(enemyBuffType, amount);
    //}
}
