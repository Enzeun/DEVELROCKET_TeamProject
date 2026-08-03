using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseStat : MonoBehaviour
{
    public string Name {  get; private set; }
    public int NowHP {  get; set; }
    public int MaxHP { get; set; }
    public int MaxCost { get; set; }
    public int NowCost {  get; set; }
    public int AtkPoint { get; set; }
    public int DefPoint {  get; set; }
    public Dictionary<int, SkillBaseStat> SkillData { get; private set; }

    public event Action<int> OnDamagedTaken;
    public event Action<int, int> OnHpChanged;

    private PlayerBaseStat(string name, int maxHP, int nowHP, int maxCost, int nowCost,
        int atkPoint, int defPoint,
        Dictionary<int, SkillBaseStat> skillData)
    {
        Name = name;
        MaxHP = maxHP;
        NowHP = nowHP;
        MaxCost = maxCost;
        NowCost = nowCost;
        AtkPoint = atkPoint;
        DefPoint = defPoint;
        SkillData = skillData;
    }

    public void TakeDamage(int damage)
    {
        NowHP = Mathf.Clamp(NowHP - damage, 0, MaxHP);
        OnDamagedTaken.Invoke(damage);
        OnHpChanged?.Invoke(NowHP, MaxHP);
    }

    public bool UseCost(int cost)
    {
        if (cost > NowCost) return false;
        else
        {
            NowCost -= cost;
            return true;
        }
    }

    public void ResetCost()
    {
        NowCost = MaxCost;
    }

}
