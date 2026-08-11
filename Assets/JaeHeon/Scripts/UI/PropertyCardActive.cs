using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Shuffle;
using static SkillEnums;

public static class Shuffle
{
    public static List<T> FisherYatesShuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        return list;
    }
}

public class PropertyCardActive : MonoBehaviour
{
    [BoxGroup(""), SerializeField]
    private PlayerCombat player;
    [BoxGroup(""), SerializeField]
    private List<GameObject> cards = new();
    [BoxGroup(""), SerializeField]
    private List<TMP_Text> cardsDescription = new();
    [BoxGroup(""), SerializeField]
    private List<KeyValuePair<int,SkillUpgradeType>> upgradeList = new();
    [BoxGroup(""), SerializeField]
    private List<Image> cardIcons = new();
    [BoxGroup(""), SerializeField]
    private List<Sprite> skillIcons = new();
    [BoxGroup(""), SerializeField]
    private List<Sprite> cardFrames = new();
    [BoxGroup(""), SerializeField]
    private List<Animator> cardAnimator = new();
    [BoxGroup(""), SerializeField]
    private Sprite cardDefalutFrame;
    [BoxGroup(""), SerializeField]
    private Sprite cardDefalutIcon;


    [BoxGroup("설정"), SerializeField]
    private Vector3 punchValue;
    [BoxGroup("설정"), SerializeField]
    private float durationValue;
    [BoxGroup("설정"), SerializeField]
    private int vibratoValue;
    [BoxGroup("설정"), SerializeField]
    private float elasticValue;
    [BoxGroup("설정"), SerializeField]
    private int displayCardCount = 3;

    private static readonly int IsSpecialHash = Animator.StringToHash("IsSpecial");
    Dictionary<int, SkillBaseStat> data;

    private void OnEnable()
    {
        upgradeList = new List<KeyValuePair<int, SkillUpgradeType>>();
        displayCardCount = cardsDescription.Count;
        SetPlayer();
        InitCard();
        MakeCardList();
        StartCoroutine(CardRotate(cards));
    }

    public void SetPlayer()
    {
        player = TurnManager.instance.playerCombat;
    }

    //리스트 섞기, 리스트에서 3개 뽑기 , 화면에 뿌려주기, 

    private void InitCard()
    {
        for (int i = 0; i < displayCardCount; i++)
        {
            Image card = cards[i].GetComponent<Image>();

            cardIcons[i].sprite = cardDefalutIcon;
            cardIcons[i].color = Color.white;
            foreach(Animator ani in cardAnimator)
            {
                ani.enabled = false;
            }
            card.sprite = cardDefalutFrame;
            card.color = Color.white;
            cardsDescription[i].text = null;
        }
    }

    private void MakeCardList()
    {
        data = player.player.SkillData;

        bool ableOverpower = UnityEngine.Random.Range(0, 100) >= 85;
        bool ableOvercharge = UnityEngine.Random.Range(0, 100) >= 90;
        bool ableWideRange = UnityEngine.Random.Range(0, 100) >= 85;

        foreach (var item in data)
        {
            int key = item.Key;
            KeyValuePair<int, SkillUpgradeType> value = new();

            item.Value.UpgradeAbleList.ForEach(upgrade =>
            {
                // 코스트 감소는 1회만 가능
                if (upgrade == SkillUpgradeType.DownCost && !item.Value.IsDownCost())
                    value = new(key, upgrade);

                // 압도는 1회만 획득 가능
                if (upgrade == SkillUpgradeType.Overpower && ableOverpower && 
                    !item.Value.IsOverPower())
                    value = new(key, upgrade);

                // 과충전은 1회만 획득 가능
                if(upgrade == SkillUpgradeType.Overcharge &&  ableOvercharge && 
                    !item.Value.IsOverCharge())
                    value = new(key, upgrade);

                // 광역화는 1회만 획득 가능
                if(upgrade == SkillUpgradeType.WideRange && ableWideRange)
                    value = new(key, upgrade); 

                else if (upgrade != SkillUpgradeType.DownCost && 
                        upgrade != SkillUpgradeType.WideRange &&
                        upgrade != SkillUpgradeType.Overcharge &&
                        upgrade != SkillUpgradeType.Overpower
                        )
                    value = new(key, upgrade);

                upgradeList.Add(value);
            });
        }
    }

    private IEnumerator CardRotate(List<GameObject> cards)
    {
        Sequence sq = DOTween.Sequence();
        foreach (GameObject card in cards)
        {
            sq.Join(card.transform.DOPunchRotation(
                punch: punchValue,
                duration: durationValue,
                vibrato: vibratoValue,
                elasticity: elasticValue));
        }

        yield return sq.WaitForCompletion();

        ShowCards();
    }

    private void ShowCards()
    {
        if (upgradeList.Count > 0)
        {
            FisherYatesShuffle(upgradeList);
        }
        else
        {
            Debug.Log($"{upgradeList.Count} 갯수만 존재해서 shuffle 안됨");
            return;
        }
        
        // 단일 스킬 광역화
        int wideRangeIndex = upgradeList.FindIndex(x => x.Value == SkillUpgradeType.WideRange);
        
        if (wideRangeIndex >= 0)
        {
            KeyValuePair<int, SkillUpgradeType> temp = upgradeList[wideRangeIndex];

            upgradeList.RemoveAt(wideRangeIndex);
            upgradeList.Insert(0, temp);
        }

        // 단일 스킬 과충전
        int overchargeIndex = upgradeList.FindIndex(x => x.Value == SkillUpgradeType.Overcharge);

        if (overchargeIndex >= 0)
        {
            KeyValuePair<int, SkillUpgradeType> temp = upgradeList[overchargeIndex];

            upgradeList.RemoveAt(overchargeIndex);
            upgradeList.Insert(0, temp);
        }

        // 광역 스킬 압도
        int overpowerIndex = upgradeList.FindIndex(x => x.Value == SkillUpgradeType.Overpower);

        if (overpowerIndex >= 0)
        {
            KeyValuePair<int, SkillUpgradeType> temp = upgradeList[overpowerIndex];

            upgradeList.RemoveAt(overpowerIndex);
            upgradeList.Insert(0, temp);
        }

        DisplayCardFrame();
        DisplayCardIcon();
    }

    private void DisplayCardFrame()
    {
        for (int i = 0; i < displayCardCount; i++)
        {
            Image currentCard = cards[i].GetComponent<Image>();
            Animator nowAnimator = cardAnimator[i];
            currentCard.color = Color.white;
            nowAnimator.enabled = false;
            nowAnimator.SetBool(IsSpecialHash, false);

            switch (upgradeList[i].Value)
            {
                case SkillUpgradeType.Damage:
                    currentCard.sprite = cardFrames[0];
                    currentCard.color = Color.red;
                    cardsDescription[i].text = "데미지 50% 증가";
                    break;
                case SkillUpgradeType.DownCost:
                    currentCard.sprite = cardFrames[1];
                    currentCard.color = Color.skyBlue;
                    cardsDescription[i].text = "코스트 1 감소";
                    break;
                case SkillUpgradeType.LifeStill:
                    currentCard.sprite = cardFrames[2];
                    currentCard.color = Color.springGreen;
                    cardsDescription[i].text = "피해 15% 흡혈\n(광역 : 7%)";
                    break;
                case SkillUpgradeType.Overcharge:
                    currentCard.sprite = cardFrames[3];
                    currentCard.color = Color.gold;
                    nowAnimator.enabled = true;
                    nowAnimator.SetBool(IsSpecialHash, true);
                    cardsDescription[i].text = "<b>[과충전]</b> 부여</size>\n한 턴에 재사용 시 마다 데미지 40% 증가";
                    break;
                case SkillUpgradeType.WideRange:
                    currentCard.sprite = cardFrames[3];
                    currentCard.color = Color.gold;
                    nowAnimator.enabled = true;
                    nowAnimator.SetBool(IsSpecialHash, true);
                    cardsDescription[i].text = "<b>[광역화]</b> 부여\n스킬의 대상을 광역으로 변경";
                    break;
                case SkillUpgradeType.Overpower:
                    currentCard.sprite = cardFrames[3];
                    currentCard.color = Color.gold;
                    nowAnimator.enabled = true;
                    nowAnimator.SetBool(IsSpecialHash, true);
                    cardsDescription[i].text = "<b>[압도]</b> 부여\n대상이 하나일 때 1회 추가 공격";
                    break;
            }
        }
    }
    private void DisplayCardIcon()
    {
        for (int i = 0; i < displayCardCount; i++)
        {
            Image currentIcon = cardIcons[i].GetComponent<Image>();
            switch (upgradeList[i].Key.ToString())
            {
                case "1000":
                    currentIcon.sprite = skillIcons[0];
                    if(ColorUtility.TryParseHtmlString("#FFD225", out Color outColor))
                        currentIcon.color = outColor;
                    break;
                case "1001":
                    currentIcon.sprite = skillIcons[1];
                    break;
                case "1002":
                    currentIcon.sprite = skillIcons[2];
                    break;
            }
        }
    }

    public void SaveCardData(int index)
    {
        foreach(var card in cards)
        {
            if (!card.TryGetComponent(out Button btn))
                return;
            btn.interactable = false;
        }

        int id = upgradeList[index].Key;
        SkillUpgradeType value = upgradeList[index].Value;
        SkillBaseStat skill = player.player.SkillData[id];
        if (value == SkillUpgradeType.WideRange  || 
            value == SkillUpgradeType.Overcharge ||
            value == SkillUpgradeType.DownCost   || 
            value == SkillUpgradeType.Overpower)
        { 
            skill.UpgradeAbleList.Remove(value); 
            if(value  == SkillUpgradeType.WideRange)
                skill.TargetType = SkillTargetType.Area; 
        }
            
        skill.NowUpgradeList.Add(value);
        
        GameStateManager.Instance.saveSkillData = new Dictionary<int, SkillBaseStat>(player.player.SkillData);
    }
}
