using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class SkillBtnController : MonoBehaviour
{
    [SerializeField, Required]
    private TextMeshProUGUI skillNameText;
    [SerializeField, Required]
    private TextMeshProUGUI skillCostText;
    [SerializeField, Required]
    private TextMeshProUGUI skillDescText;
    [SerializeField, Required]
    private TextMeshProUGUI skillDmgText;

    private SkillBaseStat mySkill;

    public void SetSkillInfo(SkillBaseStat skill)
    {
        mySkill = skill;

        SetSkillName();

        SetSkillCost();
    }

    public void SetSkillName()
    {
        string name = mySkill.Name;
        skillNameText.text = name;
    }

    public void SetSkillCost()
    {
        int cost = mySkill.GetCost();
        skillCostText.text = $"cost: {cost}";
    }

    public void SetSkillDesc()
    {
        int cost;
        skillDescText.text = $"";
    }

    public void SetSkillDmg()
    {
        int cost = mySkill.GetCost();
        skillCostText.text = $"cost: {cost}";
    }


    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }

}
