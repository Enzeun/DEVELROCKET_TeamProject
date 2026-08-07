using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField, Required]
    private CanvasGroup skillInfoCanvas;


    private SkillBaseStat mySkill;
    private int playerBaseAttack;

    public void SetSkillInfo(SkillBaseStat skill, int attack)
    {
        mySkill = skill;

        playerBaseAttack = attack;

        SetSkillName();

        SetSkillCost();

        SetSkillDesc();

        SetSkillDmg();
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
        string desc = mySkill.GetDescription();
        skillDescText.text = desc;
    }

    public void SetSkillDmg()
    {
        int dmg = (playerBaseAttack * mySkill.SkillDamageCalcByUpgrade() + 50) / 100;
        skillDmgText.text = $"데미지 : {dmg}";
    }


    public void MouseEnter()
    {
        skillInfoCanvas.alpha = 1;
    }
    public void MouseExit()
    {
        skillInfoCanvas.alpha = 0;
    }
}
