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
        NowHP = Mathf.Clamp(NowHP - damage, 0, MaxHP);

        if (NowHP <= 0) OnDead.Invoke();
        else OnDamagedTaken.Invoke(damage);

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
            return true;
        }
    }

    /// <summary>
    /// 스킬 사용을 취소할 경우, 코스트 초기화
    /// </summary>
    public void ResetCost()
    {
        NowCost = MaxCost;
    }

}
