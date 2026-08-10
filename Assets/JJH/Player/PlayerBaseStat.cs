using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseStat
{
    public string Name {  get; private set; }
    public int NowHP {  get; set; }
    public int MaxHP { get; set; }
    public int MaxCost { get; set; }
    public int NowCost {  get; set; }
    public int AtkPoint { get; set; }
    public int DefPoint {  get; set; }

    /// <summary>
    /// <para>Key(int) : 스킬 아이디</para>
    /// <para>Value(SkillBaseStat) : 스킬 데이터</para>
    /// </summary>
    public Dictionary<int, SkillBaseStat> SkillData { get; private set; }

    /// <summary>
    /// 캐릭터 피격 시 작동(피해량 전용)
    /// <para>int : 피해량</para>
    /// </summary>
    public event Action<int> OnDamagedTaken;
    /// <summary>
    /// 캐릭터 피격 시 작동(체력 변경량 전용)
    /// <para>int : 현재 체력, int : 최대 체력</para>
    /// </summary>
    public event Action<int, int> OnHpChanged;

    /// <summary>
    /// 코스트 변동 시 작동
    /// <para>int : 현재 코스트, int : 최대 코스트 </para>
    /// </summary>
    public event Action<int, int> OnCostChanged;

    /// <summary>
    /// 캐릭터가 사망할 경우 작동
    /// </summary>
    public event Action OnDead;

    public PlayerBaseStat(string name, int maxHP, int nowHP, int maxCost, int nowCost,
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

    /// <summary>
    /// 캐릭터에게 가하는 피해 계산
    /// </summary>
    /// <param name="damage">피해량</param>
    public void TakeDamage(int damage)
    {
        int finalDamage = FinalDmage(damage);

        NowHP = Mathf.Clamp(NowHP - finalDamage, 0, MaxHP);

        if (NowHP <= 0) OnDead?.Invoke();
        else OnDamagedTaken?.Invoke(finalDamage);

        OnHpChanged?.Invoke(NowHP, MaxHP);
    }

    /// <summary>
    /// 캐릭터가 받는 체력 회복
    /// </summary>
    /// <param name="heal">회복량</param>
    public void TakeHeal(int heal)
    {
        NowHP = Mathf.Min(NowHP + heal, MaxHP);

        OnHpChanged?.Invoke(NowHP, MaxHP);
    }

    /// <summary>
    /// 캐릭터가 사용하는 코스트 계산
    /// </summary>
    /// <param name="cost">사용한 코스트</param>
    /// <returns></returns>
    public bool UseCost(int cost)
    {
        if (cost > NowCost) return false;
        else
        {
            NowCost -= cost;
            OnCostChanged?.Invoke(NowCost, MaxCost);
            return true;
        }
    }

    /// <summary>
    /// 스킬 사용을 취소할 경우, 코스트 초기화
    /// </summary>
    public void ResetCost()
    {
        NowCost = MaxCost;
        OnCostChanged?.Invoke(NowCost, MaxCost);
    }

    /// <summary>
    /// 받은 대미지 대비 방어력 계산
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public int FinalDmage(int damage)
    {
        return damage - DefPoint;
    }

}
