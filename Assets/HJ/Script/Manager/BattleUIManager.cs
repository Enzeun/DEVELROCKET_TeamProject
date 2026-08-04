using DamageNumbersPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private TextMeshProUGUI playerHpBarText;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private Slider playerHpSlider;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private DamageNumber player_NumberPrefab;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private RectTransform playerHP_Number_Location;

    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private NumberLoacationContriller[] enemy_HpBarControllers;
    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private DamageNumber enemy_NumberPrefab;
    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private List<GameObject> enemy_HpBarLocations;




    private PlayerBaseStat playerStat;

    private void OnEnable()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged += PlayerHpChangedUI;
            playerStat.OnDamagedTaken += TakeDamage_UI_NumberAnimation;
        }
    }

    private void OnDisable()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged -= PlayerHpChangedUI;
            playerStat.OnDamagedTaken -= TakeDamage_UI_NumberAnimation;
        }
    }

    private void Start()
    {
        SetEnemyUILocation();
    }

    private void SetEnemyUILocation()
    {
        // 카메라 뒤쪽으로 넘어갔을때의 코드, 지금은 필요없음
        //if (newPosition.z < 0)
        //{           
        //    enemy_HpBarControllers[0].gameObject.SetActive(false);
        //    return;
        //}
        //enemy_HpBarControllers[0].gameObject.SetActive(true);

        Vector3 newPosition = Camera.main.WorldToScreenPoint(enemy_HpBarLocations[0].transform.position);

        float movingPosition = enemy_HpBarControllers[0].GetComponent<RectTransform>().rect.width /4;

        enemy_HpBarControllers[0].gameObject.transform.position = newPosition - Vector3.right* movingPosition;
    }

    //==========================================================================================================================

    [BoxGroup("UI Debug"), Button]
    private void PlayerHpChangedUI(int currentHp, int maxHp)
    {
        float endValue = (float)currentHp / maxHp;

        playerHpBarText.text = $"{currentHp}/{maxHp}";

        HP_Decrease_UIAnimation(endValue);

    }

    private void TakeDamage_UI_NumberAnimation(int damage)
    {
        DamageNumber damageNumber = player_NumberPrefab.SpawnGUI(playerHP_Number_Location, Vector2.zero, damage);

    }

    [BoxGroup("UI Debug"), Button]
    private void HP_Decrease_UIAnimation(float endValue)
    {

        playerHpSlider.transform.DOShakePosition(0.2f, 10f, 90);
        playerHpSlider.DOValue(endValue, 0.1f);
    }

}
