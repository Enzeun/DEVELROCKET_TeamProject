using UnityEngine;
public enum Enemy_Type
{
    //몬스터 이름으로 타입을 나눌 예정 ( 현재 프로토타입으로 가명 넣어둠 )
    None,
    Priority_Attack_Type,
    Priority_Buff_Type,
    Priority_Defence_Type
}

[CreateAssetMenu(menuName = "Game Data/Enemy Data")]
public class EnemyInfo : ScriptableObject
{
    public int hp;
    public int defence;
    public Enemy_Type enemyType;
}
