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
    // 플레이어 관련 참조
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private TextMeshProUGUI playerHpBarText;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private Slider playerHpSlider;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private DamageNumber player_NumberPrefab;
    [SerializeField, Required, BoxGroup("**참조필요!**플레이어")]
    private RectTransform playerHP_Number_Location;

    // 적 관련 참조
    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private EnemyUIContriller[] enemy_UIControllers;
    [SerializeField, Required, BoxGroup("**참조필요!**적")]
    private DamageNumber enemy_NumberPrefab;
    //[SerializeField, Required, BoxGroup("**참조필요!**적")]
    //private GameObject[] enemy_HpBarLocations;
    //[SerializeField, Required, BoxGroup("**참조필요!**적")]
    //private EnemyBase[] enemys;

    // UI 컴포넌트 참조
    [SerializeField, Required, BoxGroup("**UI 컴포넌트 참조**")]
    private Canvas ReadyBattleCanvas;
    [SerializeField, Required, BoxGroup("**UI 컴포넌트 참조**")]
    private Canvas BattleUICanvas;

    // 참조해야하는 필드들
    private PlayerCombat playerCombat;
    private PlayerBaseStat playerStat;
    [ShowInInspector, ReadOnly]
    private List<EnemyBase> enemyList;
    private Dictionary<EnemyBase, EnemyUIContriller> UI_Dictionary = new();



    private void OnDisable()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged -= PlayerHpChangedUI;
            playerStat.OnDamagedTaken -= TakeDamage_UI_NumberAnimation_Player;
        }
        foreach (var enemy in enemyList)
        {
            enemy.OnTakeDamage -= Enemy_TakeDamage;
            enemy.OnDie += OnEnemyDie;
        }
    }




    //======================= 초기화, 초기상태 설정 메서드 ====================================================

    public void SetEnemyUILocation()
    {
        foreach (var enemy in enemyList)
        {
            EnemyUIContriller enemyUIController;

            UI_Dictionary.TryGetValue(enemy, out enemyUIController);

            if (enemyUIController == null)
            {
                Debug.Log("해당 enemy 에 대응하는 ui 가 없음.");
                return;
            }

            Vector3 newPosition = Camera.main.WorldToScreenPoint(enemy.transform.position);

            float movingPosition = enemyUIController.GetComponent<RectTransform>().rect.width / 4;

            enemyUIController.gameObject.transform.position = newPosition - Vector3.right * movingPosition;
        }
    }


    [BoxGroup("UI Debug_Player"), Button]
    public void InitializeAllHpBar()
    {
        int playerMaxHp = playerStat.MaxHP;

        playerHpBarText.text = $"{playerMaxHp}/{playerMaxHp}";
        playerHpSlider.value = 1;

        foreach (var enemy in enemyList)
        {
            EnemyUIContriller enemyHpbar;

            int maxHp = enemy.maxHp;

            UI_Dictionary.TryGetValue(enemy, out enemyHpbar);

            if (enemyHpbar == null)
            {
                Debug.Log("해당 Enemy 에 해당하는 hpBar 가 없습니다. 확인하세요");
                return;
            }

            enemyHpbar.hpSlider.value = 1;

            enemyHpbar.hpText.text = $"{maxHp}/{maxHp}";
        }
    }

    public void SetPlayerInfo(PlayerCombat _playerCombat)
    {
        playerCombat = _playerCombat;
        playerStat = playerCombat.player;
        RegisterPlayerEvent();
    }

    private void RegisterPlayerEvent()
    {
        playerStat.OnHpChanged += PlayerHpChangedUI;
        playerStat.OnDamagedTaken += TakeDamage_UI_NumberAnimation_Player;
    }

    public void SetEnemyList(List<EnemyBase> list)
    {
        enemyList = list;
    }

    public void InitUIDictinary()
    {
        UI_Dictionary = new Dictionary<EnemyBase, EnemyUIContriller>();

        int index = 0;

        foreach (var enemy in enemyList)
        {
            Debug.Log($"{index} 번 째 실행 중 (UI Dictionary)");
            EnemyUIContriller ui = enemy_UIControllers[index];
            UI_Dictionary.Add(enemy, ui);
            RegisterEnemyEvent(enemy, ui);
            index++;
        }
    }

    private void RegisterEnemyEvent(EnemyBase enemy, EnemyUIContriller uIContriller)
    {
        enemy.OnTakeDamage += Enemy_TakeDamage;
        enemy.OnDie += OnEnemyDie;
    }


    //======================= Enemy 목록 받아오기. TurnManager 에서 넣어줌 ====================================================


    //====================== Player 데미지 받을 때 메서드 ====================================================================================================

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
        playerHpSlider.DOValue(endValue, 0.5f);
    }

    //==================== Enemy 데미지 받을 때 메서드 ======================================================================================================

    private void Enemy_TakeDamage(EnemyBase enemy, int currentHp, int damage)
    {
        EnemyUIContriller uiController;

        UI_Dictionary.TryGetValue(enemy, out uiController);

        if (uiController == null)
        {
            Debug.Log("해당 Enemy 에 해당하는 uiController 가 없습니다. 확인하세요");
            return;
        }

        uiController.hpText.text = $"{currentHp}/{enemy.maxHp}";

        float endValue = (float)currentHp / enemy.maxHp;

        Math.Clamp(endValue, 0, 1);

        TakeDamage_UI_NumberAnimation_Enemy(damage, uiController);

        HP_Decrease_UIAnimation_Enemy(endValue, uiController);

        return;

    }


    private void TakeDamage_UI_NumberAnimation_Enemy(int damage, EnemyUIContriller uiController)
    {
        DamageNumber damageNumber = enemy_NumberPrefab.SpawnGUI(uiController.numberLocation, Vector2.zero, damage);
    }


    private void HP_Decrease_UIAnimation_Enemy(float endValue, EnemyUIContriller uiController)
    {
        uiController.transform.DOShakePosition(0.2f, 10f, 90);
        uiController.hpSlider.DOValue(endValue, 0.5f);
    }

    private void OnEnemyDie(EnemyBase enemy)
    {
        enemy.OnTakeDamage -= Enemy_TakeDamage;
        enemy.OnDie -= OnEnemyDie;

    }

    // ============= UI Show / Hide 관련 ===========================================================================

    [BoxGroup("UI 디버깅"), Button]
    public void HideAllUI(bool hide)
    {
        BattleUICanvas.gameObject.SetActive(!hide);
        ReadyBattleCanvas.gameObject.SetActive(!hide);
    }
    [BoxGroup("UI 디버깅"), Button]
    public void ShowBattleUI(bool show, bool anim = true)
    {
        if (anim)
        {
            if (show)
            {
                OpenPopup(BattleUICanvas);
            }
            else
            {
                ClosePopup(BattleUICanvas);
            }
        }
        else
        {
            BattleUICanvas.gameObject.SetActive(show);
        }
    }
    [BoxGroup("UI 디버깅"), Button]
    public void ShowReadyBattleUI(bool show, bool anim = true)
    {
        if (anim)
        {
            if (show)
            {
                OpenPopup(ReadyBattleCanvas);
            }
            else
            {
                ClosePopup(ReadyBattleCanvas);
            }
        }
        else
        {
            ReadyBattleCanvas.gameObject.SetActive(show);
        }
    }

    public void InvokeBattleStart()
    {
        OnBattleStartClicked?.Invoke();
    }

    public void OpenPopup(Canvas targetCanvas)
    {
        if (targetCanvas == null) return;

        targetCanvas.gameObject.SetActive(true);

        int childCount = targetCanvas.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform panelTransform = targetCanvas.transform.GetChild(i) as RectTransform;

            if (panelTransform == null) continue;

            CanvasGroup canvasGroup = panelTransform.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = panelTransform.gameObject.AddComponent<CanvasGroup>();
            }

            // 기존 진행 중인 트윈이 있다면 중단
            panelTransform.DOKill();
            canvasGroup.DOKill();

            // Scale 애니메이션
            panelTransform.DOScale(Vector3.zero, 0.4f)
                          .From()
                          .SetEase(Ease.OutBack)
                          .SetUpdate(true); // Pause 시에도 동작하도록 설정

            // Fade 애니메이션
            var fadeTween = canvasGroup.DOFade(0f, 0.25f)
                                       .From()
                                       .SetEase(Ease.OutQuad)
                                       .SetUpdate(true);
        }
    }

    public void ClosePopup(Canvas targetCanvas)
    {
        if (targetCanvas == null) return;

        int childCount = targetCanvas.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {

            RectTransform panelTransform = targetCanvas.transform.GetChild(i) as RectTransform;

            if (panelTransform == null) continue;

            CanvasGroup canvasGroup = panelTransform.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = panelTransform.gameObject.AddComponent<CanvasGroup>();
            }

            panelTransform.DOKill();
            canvasGroup.DOKill();

            // Scale 애니메이션
            panelTransform.DOScale(Vector3.zero, 0.25f)
                          .SetEase(Ease.InBack)
                          .SetUpdate(true);

            // Fade 애니메이션
            var fadeTween = canvasGroup.DOFade(0f, 0.25f)
                                       .SetEase(Ease.InQuad)
                                       .SetUpdate(true);

            // 마지막 자식 Panel의 트윈에만 OnComplete를 등록하여 Canvas 비활성화
            if (i == childCount - 1)
            {
                fadeTween.OnComplete(() =>
                {
                    targetCanvas.gameObject.SetActive(false);
                });
            }
        }
    }


    public void ShowBehaveIcon(EnemyBase enemy, bool show = true)
    {
        if (!show)
        {
            foreach (var ui in UI_Dictionary.Values)
            {
                ui.HideBehaviorIcon();
            }
        }

        else
        {
            if (enemy == null) return;

            EnemyUIContriller uiController;

            UI_Dictionary.TryGetValue(enemy, out uiController);

            uiController.ShowBehaviorIcon(enemy.currentBehaviour);
        }
    }


    // ============= UI 콜백 이벤트 =================================================================================

    public Action OnBattleStartClicked;
}
