using UnityEngine;


[CreateAssetMenu(menuName = "Game Data/Enemy Data")]
public class EnemyInfo : ScriptableObject
{
    public int hp;
    public int maxHp;
    public int attack;
    public int defence;
    public Enemy_Type enemyType;
    public int attackWeight;
    public int defenceWeight;
    public int buffweight;


}
