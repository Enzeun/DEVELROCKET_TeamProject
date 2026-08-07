using DG.Tweening;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SkillEnums;
using static Shuffle;
using TMPro;

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

    [SerializeField] private List<CardInfo> cardInfo;


    Dictionary<int, SkillBaseStat> data;

    private void OnEnable()
    {
        displayCardCount = cardsDescription.Count;
        SetPlayer();
        InitCard();
        CardRotate(cards);
        MakeCardList();

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
            card.sprite = cardDefalutFrame;
            cardsDescription[i].text = null;

        }
    }

    private void MakeCardList()
    {
        //List<KeyValuePair<int, SkillUpgradeType>> da = new();
        data = player.player.SkillData;

        foreach (var item in data)
        {
            int key = item.Key;
            KeyValuePair<int, SkillUpgradeType> value = new();

            item.Value.UpgradeAbleList.ForEach(upgrade =>
            {
                if (upgrade == SkillUpgradeType.DownCost && !item.Value.IsDownCost())
                    value = new(key, upgrade);

                else if (upgrade != SkillUpgradeType.DownCost)
                    value = new(key, upgrade);

                upgradeList.Add(value);
            });
        }

        //Debug.Log(upgradeList.Count);
        //ShowCards();
    }

    private void CardRotate(List<GameObject> cards)
    {
        foreach (GameObject card in cards)
        {
            card.transform.DOPunchRotation(
                punch: punchValue,
                duration: durationValue,
                vibrato: vibratoValue,
                elasticity: elasticValue).OnComplete(() =>
                {
                    ShowCards();
                });
        }
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

        for(int i = 0; i < displayCardCount; i++)
        {
            //cardsDescription[i].text =  upgradeList[i].Key.ToString() + upgradeList[i].Value.ToString();
            DisplayCardFrame();
            DisplayCardIcon();
        }
    }

    private void DisplayCardFrame()
    {
        for (int i = 0; i < displayCardCount; i++)
        {
            Image currentCard = cards[i].GetComponent<Image>();
            currentCard.color = Color.white;
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
                    cardsDescription[i].text = "피해 15% 흡혈\n(광역 : 5%)";
                    break;
            }
        }
    }
    private void DisplayCardIcon()
    {
        for (int i = 0; i < displayCardCount; i++)
        {
            switch (upgradeList[i].Key.ToString())
            {
                case "1000":
                    cardIcons[i].GetComponent<Image>().sprite = skillIcons[0];
                    break;
                case "1001":
                    cardIcons[i].GetComponent<Image>().sprite = skillIcons[1];
                    break;
                case "1002":
                    cardIcons[i].GetComponent<Image>().sprite = skillIcons[2];
                    break;
            }
        }
    }

    public void SaveCardData(int index)
    {
        foreach(var card in cards)
        {
            if (!card.TryGetComponent<Button>(out Button btn))
                return;
            btn.interactable = false;
        }

        int id = upgradeList[index].Key;

        SkillBaseStat skill = player.player.SkillData[id];
        skill.NowUpgradeList.Add(upgradeList[index].Value);

        GameStateManager.Instance.saveSkillData = new Dictionary<int, SkillBaseStat>(player.player.SkillData);
    }
}
