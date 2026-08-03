using System.Collections.Generic;
using UnityEngine;

public class EnemySetValue : MonoBehaviour
{
    private Enemy_Type enemyType;
    private EnemyInfo enmemyInfo;
    private Dictionary<Enemy_Type, Weight> enemyWeight = new Dictionary<Enemy_Type, Weight>();
    private Dictionary<Enemy_Behaviour, int> enemyBehaviourValue = new Dictionary<Enemy_Behaviour, int>();


    private void SetValue_EnemyBehaviour()
    {
        switch (enemyType)
        {
            case Enemy_Type.None:
                //?????
                enemyBehaviourValue.Clear();
                enemyBehaviourValue.Add(Enemy_Behaviour.Attack, 5);
                break;
            case Enemy_Type.Priority_Attack_Type:
                break;
            case Enemy_Type.Priority_Buff_Type:
                break;
            case Enemy_Type.Priority_Defence_Type:
                break;

        }
    }

    private void Calc_Enemy_Behaviour()
    {

    }



    private void EnemyTurn()
    {

    }

    private void SetWeight(Enemy_Behaviour behaviour, int value)
    {
        enemyBehaviourValue.Add(behaviour, value);
    }
    private int GetWeight(Enemy_Behaviour behaviour)
    {
        enemyBehaviourValue.TryGetValue(behaviour, out int value);
        return value;
    }
}
