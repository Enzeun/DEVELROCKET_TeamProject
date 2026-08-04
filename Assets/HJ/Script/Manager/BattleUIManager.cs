using DamageNumbersPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private TextMeshProUGUI playerHpBarText;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private Slider playerHpSlider;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private DamageNumber player_NumberPrefab;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private RectTransform PlayerHP_Number_Location;

    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private DamageNumber enemy_NumberPrefab;



    private PlayerBaseStat playerStat;

    private void OnEnable()
    {
        playerStat.OnHpChanged += PlayerHpChangedUI;
        playerStat.OnDamagedTaken += TakeDamage_UI_NumberAnimation;
    }

    private void OnDisable()
    {
        playerStat.OnHpChanged -= PlayerHpChangedUI;
        playerStat.OnDamagedTaken -= TakeDamage_UI_NumberAnimation;
    }

    [BoxGroup("UI Debug"), Button]
    private void PlayerHpChangedUI(int currentHp, int maxHp)
    {
        float endValue = (float)currentHp / maxHp;

        playerHpBarText.text = $"{currentHp}/{maxHp}";

        HP_Decrease_UIAnimation(endValue);

    }

    private void TakeDamage_UI_NumberAnimation(int damage)
    {
        DamageNumber damageNumber = player_NumberPrefab.SpawnGUI(PlayerHP_Number_Location, Vector2.zero, damage);

    }

    [BoxGroup("UI Debug"), Button]
    private void HP_Decrease_UIAnimation(float endValue)
    {

        playerHpSlider.transform.DOShakePosition(0.2f, 10f, 90);
        playerHpSlider.DOValue(endValue, 0.1f);
    }

}
