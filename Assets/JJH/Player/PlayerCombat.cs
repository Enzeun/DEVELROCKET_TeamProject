using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // 플레이어 기본 스탯은 임시로 상수
    public PlayerBaseStat player;

    private void Start()
    {
        PlayerCombatStatInit();
    }

    private void PlayerCombatStatInit()
    {
        // 임시 데이터, 데이터 로드 방식 추가시 변경
        //string name, int maxHP, int nowHP, int maxCost, int nowCost,
        /*int atkPoint, int defPoint,
        Dictionary< int, SkillBaseStat > skillData*/
        player = new ("베이스", 200, 200, 6, 6, 10, 5, SkillData.BaseSkillData);
    }
}
