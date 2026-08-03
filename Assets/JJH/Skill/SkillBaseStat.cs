using System.Collections.Generic;
using UnityEngine;
using static SkillEnums;

public class SkillBaseStat
{
    public int Id;
    public string Name;
    public string Description;
    private int Cost;
    private int Damage;
    
    public List<SkillUpgradeType> UpgradeAbleList;
    public List<SkillUpgradeType> NowUpgradeList;
    public SkillTargetType TargetType;

    public SkillBaseStat(int id, string name, string description, int cost, int damage, 
        List<SkillUpgradeType> upgradeAbleList, SkillTargetType targetType)
    {
        Id = id;
        Name = name;
        Description = description;
        Cost = cost;
        Damage = damage;
        UpgradeAbleList = upgradeAbleList;
        NowUpgradeList = new();
        TargetType = targetType;
    }

    public (int skillCost, int SkillId) UseSkillData()
    {
        return (Cost, Id);
    }

    public int SkillDamageCalcByUpgrade()
    {
        int nowDamage = Damage;

        if (NowUpgradeList != null && NowUpgradeList.Count > 0) 
        {
            NowUpgradeList.ForEach(damageUp => { 
                if(damageUp == SkillUpgradeType.Damage)
                    nowDamage = nowDamage * 150 / 100;
            });
        }

        return nowDamage;
    }
}
