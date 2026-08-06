using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIContriller : MonoBehaviour
{
    public RectTransform numberLocation;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Image behaviorIcon;

    public Sprite attackicon;
    public Sprite bufficon;
    public Sprite warnningicon;

    private void Start()
    {
        HideBehaviorIcon();
    }

    public void ShowBehaviorIcon(Enemy_Behaviour behave)
    {
        behaviorIcon.enabled = true;

        switch (behave)
        {
            default:
                behaviorIcon.enabled = false;
                Debug.Log("설정이 안된 행동입니다. EnemyUIController 의 RefreshBehaviorIcon 을 확인하세요");
                return;
            case Enemy_Behaviour.NormalAttack:
                behaviorIcon.sprite = attackicon;
                break;

            case Enemy_Behaviour.Shoot:
                behaviorIcon.sprite = attackicon;
                break;
            case Enemy_Behaviour.Spell:
                behaviorIcon.sprite = attackicon;
                break;
            case Enemy_Behaviour.Skill3:
                behaviorIcon.sprite = attackicon;
                break;
            case Enemy_Behaviour.Skill4:
                behaviorIcon.sprite = attackicon;
                break;
         
            case Enemy_Behaviour.Buff:
                behaviorIcon.sprite = bufficon;
                break;
        }
    }

    public void HideBehaviorIcon()
    {
        behaviorIcon.enabled = false;
    }
}
