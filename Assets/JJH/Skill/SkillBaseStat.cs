using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SkillEnums;
using static SkillConstants;

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

    /// <summary>
    /// 스킬 발동 자세
    /// </summary>
    public SkillPoseType Pose;
    /// <summary>
    /// 가능한 업그레이드 목록
    /// </summary>
    public List<SkillUpgradeType> UpgradeAbleList;

    /// <summary>
    /// 현재 가진 업그레이드 목록
    /// </summary>
    public List<SkillUpgradeType> NowUpgradeList;

    /// <summary>
    /// 스킬이 지정할 수 있는 범위 타입(단일/광역)
    /// </summary>
    public SkillTargetType TargetType;
    /// <summary>
    /// 스킬이 발동되는 형태(투사체/즉발)
    /// </summary>
    public SkillTargetSubType TargetSubType;

    public SkillBaseStat(int id, string name, string description, int cost, int damage, 
        SkillPoseType pose, List<SkillUpgradeType> upgradeAbleList,
        SkillTargetType targetType, SkillTargetSubType targetSubType)
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
        TargetSubType = targetSubType;
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
        int count = NowUpgradeList.Count(x => x == SkillUpgradeType.LifeStill);

        if (count > 0)
        {
            int value = (TargetType == SkillTargetType.Area) ? 5 : 15;
            result = count * value;
            return true;
        }

        result = 0;
        return false;
    }

    /// <summary>
    /// 현재 스킬이 DownCost 속성을 이미 가지고 있는지 확인
    /// </summary>
    /// <returns>가지고 있을 경우 : true</returns>
    public bool IsDownCost()
    {
        if (NowUpgradeList.Contains(SkillUpgradeType.DownCost))
            return true;

        return false;
    }

    public bool IsOverCharge()
    {
        if (NowUpgradeList.Contains(SkillUpgradeType.Overcharge))
            return true;

        return false;
    }

    public bool IsOverPower()
    {
        if (NowUpgradeList.Contains(SkillUpgradeType.Overpower))
            return true;

        return false;
    }

    /// <summary>
    /// 현재 스킬 비용 반환
    /// </summary>
    /// <returns>int : 스킬 비용 값</returns>
    public int GetCost()
    {
        int count = NowUpgradeList.Count(x => x == SkillUpgradeType.DownCost);

        return Mathf.Clamp(Cost - count, 1, Cost);
    }

    public string GetDescription()
    {
        int damage = SkillDamageCalcByUpgrade();
        string targetType =
            TargetType == SkillTargetType.Single ?
            "<color=#E32D10>단일</color>" : "<color=#1D5DC4>광역</color>";
        string desc = Description
            .Replace($"[{DAMAGE}]", damage.ToString())
            .Replace($"[{TARGET_TYPE}]", targetType);

        return desc;
    }
}
