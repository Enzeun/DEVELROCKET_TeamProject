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
            1,
            // 피해량
            100,
            // 가능한 업그레이드
            new List<SkillUpgradeType> { 
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill  }
            ,
            // 타겟 지정 종류
            SkillTargetType.Single 
        ),
        [1001] = new SkillBaseStat(
            1001,
            "단일2",
            "설명2",
            2,      
            200,     
            new List<SkillUpgradeType> {
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill  }
            ,                     
            SkillTargetType.Single
        ),
        [1002] = new SkillBaseStat(
            1002,
            "광역1", 
            "설명3", 
            3, 
            150, 
            new List<SkillUpgradeType> {
                SkillUpgradeType.Damage,
                SkillUpgradeType.DownCost,
                SkillUpgradeType.LifeStill  }
            ,                   
            SkillTargetType.Area
        ),
    };
}
