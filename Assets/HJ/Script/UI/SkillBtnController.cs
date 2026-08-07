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


    public void SetSkillName(string name)
    {
        skillNameText.text = name;
    }

    public void SetSkillCost(int cost)
    {
        skillCostText.text = $"cost: {cost}";
    }
}
