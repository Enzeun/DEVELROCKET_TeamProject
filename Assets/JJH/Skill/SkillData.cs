using System.Collections.Generic;
using UnityEngine;
using static SkillEnums;

public static class SkillData
{
    public static readonly Dictionary<int, SkillBaseStat> BaseSkillData = new()
    {
        [1000] = new SkillBaseStat(
            // 아이디
            1000,
            // 이름
            "화염살",
            // 스킬 설명
            "[damage]%의 [targetType]피해를 줍니다.",
            // 코스트
            2,
            // 피해량
            150,
            SkillPoseType.Horizontal,
            // 가능한 업그레이드
            new List<SkillUpgradeType> { 
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill,
                SkillUpgradeType.Overcharge
            }
            ,
            // 타겟 지정 종류
            SkillTargetType.Single,
            SkillTargetSubType.Multi
        ),
        [1001] = new SkillBaseStat(
            1001,
            "화염구",
            "[damage]%의 [targetType]피해를 줍니다.\n<size=16>방어력 관통 30%</size>",
            4,
            260,
            SkillPoseType.Instance,
            new List<SkillUpgradeType> {
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill,
                SkillUpgradeType.WideRange
            }
            ,                     
            SkillTargetType.Single,
            SkillTargetSubType.Multi

        ),
        [1002] = new SkillBaseStat(
            1002,
            "낙뢰",
            "[damage]%의 [targetType]피해를 줍니다.", 
            5, 
            190,
            SkillPoseType.Stomp,
            new List<SkillUpgradeType> {
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill,
                SkillUpgradeType.Overpower
            }
            ,                   
            SkillTargetType.Area,
            SkillTargetSubType.Single
        ),
    };
}
