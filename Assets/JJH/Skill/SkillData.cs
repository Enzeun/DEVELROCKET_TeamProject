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
            "단일1",
            // 스킬 설명
            "설명1",
            // 코스트
            2,
            // 피해량
            100,
            SkillPoseType.Horizontal,
            // 가능한 업그레이드
            new List<SkillUpgradeType> { 
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill  }
            ,
            // 타겟 지정 종류
            SkillTargetType.Single,
            SkillTargetSubType.Multi
        ),
        [1001] = new SkillBaseStat(
            1001,
            "단일2",
            "설명2",
            3,      
            200,
            SkillPoseType.Instance,
            new List<SkillUpgradeType> {
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill  }
            ,                     
            SkillTargetType.Single,
            SkillTargetSubType.Multi

        ),
        [1002] = new SkillBaseStat(
            1002,
            "광역1", 
            "설명3", 
            4, 
            150, 
            SkillPoseType.Stomp,
            new List<SkillUpgradeType> {
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill  }
            ,                   
            SkillTargetType.Area,
            SkillTargetSubType.Single
        ),
    };
}
