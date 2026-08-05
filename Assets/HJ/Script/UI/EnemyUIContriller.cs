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
            case Enemy_Behaviour.Attack:
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
