using DamageNumbersPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private PlayerCombat playerCombat;
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
    private GameObject[] enemy_HpBarLocations;
    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private EnemyBase[] enemys;


    private PlayerBaseStat playerStat;

    private void OnEnable()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged += PlayerHpChangedUI;
            playerStat.OnDamagedTaken += TakeDamage_UI_NumberAnimation_Player;
        }

        foreach (var enemy in enemys)
        {
            enemy.OnTakeDamage += Enemy_TakeDamage;

        }
    }

    private void OnDisable()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged -= PlayerHpChangedUI;
            playerStat.OnDamagedTaken -= TakeDamage_UI_NumberAnimation_Player;
        }
        foreach (var enemy in enemys)
        {
            enemy.OnTakeDamage -= Enemy_TakeDamage;
        }
    }

    private void Start()
    {
        playerStat = playerCombat.player;

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

        for (int i = 0; i < enemy_HpBarLocations.Length; i++)
        {
            Debug.Log(i + " 번째 실행 중");
            Vector3 newPosition = Camera.main.WorldToScreenPoint(enemy_HpBarLocations[i].transform.position);

            float movingPosition = enemy_HpBarControllers[i].GetComponent<RectTransform>().rect.width / 4;

            enemy_HpBarControllers[i].gameObject.transform.position = newPosition - Vector3.right * movingPosition;
        }
    }

    //==========================================================================================================================

    [BoxGroup("UI Debug_Player"), Button]
    private void InitializePlayerHpBar()
    {

    }


    [BoxGroup("UI Debug_Player"), Button]
    private void PlayerHpChangedUI(int currentHp, int maxHp)
    {
        float endValue = (float)currentHp / maxHp;

        playerHpBarText.text = $"{currentHp}/{maxHp}";

        HP_Decrease_UIAnimation_Player(endValue);

    }

    private void TakeDamage_UI_NumberAnimation_Player(int damage)
    {
        DamageNumber damageNumber = player_NumberPrefab.SpawnGUI(playerHP_Number_Location, Vector2.zero, damage);
    }

    [BoxGroup("UI Debug_Player"), Button]
    private void HP_Decrease_UIAnimation_Player(float endValue)
    {
        Math.Clamp(endValue, 0, 1);
        playerHpSlider.transform.DOShakePosition(0.2f, 10f, 90);
        playerHpSlider.DOValue(endValue, 0.1f);
    }

    //==========================================================================================================================

    [BoxGroup("UI Debug_Enemy"), Button]
    private void Enemy_TakeDamage(EnemyBase enemy, int currentHp, int damage)
    {
        for (int i = 0; i < enemys.Length; i++)
        {
            if (enemy == enemys[i])
            {
                TakeDamage_UI_NumberAnimation_Enemy(damage, i);

                enemy_HpBarControllers[i].hpText.text = $"{currentHp}/{enemy.maxHp}";

                float endValue = (float)currentHp / enemy.maxHp;

                //Debug.Log(currentHp + "1");
                //Debug.Log(endValue + "2");

                Math.Clamp(endValue, 0, 1);

                HP_Decrease_UIAnimation_Enemy(endValue, i);

                return;
            }
        }
    }


    [BoxGroup("UI Debug_Enemy"), Button]
    private void TakeDamage_UI_NumberAnimation_Enemy(int damage, int index)
    {
        if (index > enemy_HpBarControllers.Length)
        {
            Debug.Log("존재하지 않는 Index 입니다. 메서드 : TakeDamage_UI_NumberAnimation_Enemy");
            return;
        }
        DamageNumber damageNumber = enemy_NumberPrefab.SpawnGUI(enemy_HpBarControllers[index].numberLocation, Vector2.zero, damage);
    }
    [BoxGroup("UI Debug_Enemy"), Button]
    private void HP_Decrease_UIAnimation_Enemy(float endValue, int index)
    {
        if (index > enemy_HpBarControllers.Length)
        {
            Debug.Log("존재하지 않는 Index 입니다. 메서드 : HP_Decrease_UIAnimation_Enemy");
            return;
        }
        Math.Clamp(endValue, 0, 1);
        enemy_HpBarControllers[index].transform.DOShakePosition(0.2f, 10f, 90);
        enemy_HpBarControllers[index].hpSlider.DOValue(endValue, 0.1f);
    }

}
