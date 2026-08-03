using System.Runtime.InteropServices;
using UnityEngine;
using SF = UnityEngine.SerializeField;
public class PlayerCombat : MonoBehaviour
{

    [SF] private SkillEffectSpawner spawner;
    [SF] private Transform DummyTarget;
    [SF] private Animator playerAnimator;
    // 플레이어 기본 스탯은 임시로 상수
    [HideInInspector]public PlayerBaseStat player;

    private Transform nowTarget;
    private SkillBaseStat nowSkillData;

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

    public void SetNowSkillAndTarget(SkillBaseStat data, Transform target)
    {
        // 원랜 아래를 호출해야함
        nowSkillData = data;
        nowTarget = target;

    }

    public void EffectActive()
    {
        //임시
        nowSkillData = SkillData.BaseSkillData[1001];
        playerAnimator.Play(nowSkillData.Pose.ToString());
        nowTarget = DummyTarget;
        if(nowSkillData != null && nowTarget != null)
            spawner.SpawnEffect(nowSkillData.Name, nowSkillData.Pose, nowTarget);
    }
}
