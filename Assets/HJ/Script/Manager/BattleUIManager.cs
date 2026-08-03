using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;
using DG.Tweening;
using DamageNumbersPro;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField, Required, BoxGroup("**참조필요!**")]
    private TextMeshProUGUI playerHpBarText;
    [SerializeField, Required, BoxGroup("**참조필요!**")]
    private Slider playerHpSlider;
    [SerializeField, Required, BoxGroup("**참조필요!**")]
    private DamageNumber numberPrefab;
    [SerializeField, Required, BoxGroup("**참조필요!**")]
    private RectTransform rectParent;

    private PlayerBaseStat playerStat;




    [BoxGroup("UI Debug"), Button]
    private void PlayerTakeDamageUI(int currentHp, int damage)
    {
        int max = 300;

        float value = (float)currentHp / max;

        playerHpBarText.text = $"{currentHp}/{max}";

        PlayDamagedUIAnimation(damage, value);
    }

    [BoxGroup("UI Debug"), Button]
    private void PlayDamagedUIAnimation(int damage, float endValue)
    {
        DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, Vector2.zero, damage);

        playerHpSlider.transform.DOShakePosition(0.2f, 10f, 90);
        playerHpSlider.DOValue(endValue, 0.1f);
    }

}
