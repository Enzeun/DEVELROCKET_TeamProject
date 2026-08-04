using System;
using System.Collections.Generic;
using UnityEngine;
using static SkillEnums;

/// <summary>
/// 스킬 기본 구조
/// </summary>
public class SkillBaseStat
{
    public int Id;
    public string Name;
    public string Description;
    private int Cost;
    /// <summary>
    /// Damage는 피해량 배율로 100%가 기준값, 공격력에 곱해져야함
    /// </summary>
    private int Damage;

    public SkillPoseType Pose;
    public List<SkillUpgradeType> UpgradeAbleList;
    public List<SkillUpgradeType> NowUpgradeList;
    public SkillTargetType TargetType;

    public event Action OnSkillEnded;

    public SkillBaseStat(int id, string name, string description, int cost, int damage, 
        SkillPoseType pose, List<SkillUpgradeType> upgradeAbleList, SkillTargetType targetType)
    {
        Id = id;
        Name = name;
        Description = description;
        Cost = cost;
        Damage = damage;
        UpgradeAbleList = upgradeAbleList;
        NowUpgradeList = new();
        TargetType = targetType;
        Pose = pose;
    }

    /// <summary>
    /// 현재 보유한 업그레이드를 반영한 스킬 피해량 계산
    /// </summary>
    /// <returns></returns>
    public int SkillDamageCalcByUpgrade()
    {
        int nowDamage = Damage;

        if (NowUpgradeList != null && NowUpgradeList.Count > 0) 
        {
            NowUpgradeList.ForEach(damageUp => { 
                if(damageUp == SkillUpgradeType.Damage)
                    nowDamage = (nowDamage * 150 + 50) / 100;
            });
        }

        return nowDamage;
    }

    /// <summary>
    /// 흡혈 가능 여부 확인 함수
    /// </summary>
    /// <param name="result">스킬의 흡혈량</param>
    /// <returns>bool : 흡혈 가능 여부</returns>
    public bool IsLifeStill(out int result)
    {
        result = 0;

        int value = 0;

        NowUpgradeList.ForEach(lifeStill => {
            if (lifeStill == SkillUpgradeType.LifeStill)
                value += 15;
        });

        if (value == 0) return false;
        else
        {
            result = value;
            return true;
        }
    }

    /// <summary>
    /// 현재 스킬 비용 반환
    /// </summary>
    /// <returns>int : 스킬 비용 값</returns>
    public int GetCost()
    {
        int value = 0;

        NowUpgradeList.ForEach(lifeStill => {
            if (lifeStill == SkillUpgradeType.DownCost)
                value += 1;
        });

        return Mathf.Clamp(Cost - value, 1, Cost);

    }
}
