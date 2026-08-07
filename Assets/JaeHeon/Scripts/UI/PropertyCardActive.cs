using DG.Tweening;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SkillEnums;

public class PropertyCardActive : MonoBehaviour
{
    [BoxGroup(""), SerializeField]
    private PlayerCombat player;
    [BoxGroup(""), SerializeField]
    List<GameObject> cards = new();
    [BoxGroup(""), SerializeField]
    List<KeyValuePair<int,SkillUpgradeType>> upgradeList = new();
    [BoxGroup(""), SerializeField]
    private bool isMake = false;
    // 이 창을 매 씬마다 배치할건지


    [BoxGroup("설정"), SerializeField]
    private Vector3 punchValue;
    [BoxGroup("설정"), SerializeField]
    private float durationValue;
    [BoxGroup("설정"), SerializeField]
    private int vibratoValue;
    [BoxGroup("설정"), SerializeField]
    private float elasticValue;

    [SerializeField] private List<RandomSkillCard> m_listCard;
    private void OnEnable()
    {
        MakeCardList();

        CardRotate(cards);
    }

    //리스트 섞기, 리스트에서 3개 뽑기 , 화면에 뿌려주기, 

    private void MakeCardList()
    {
        if(isMake)
        {
            isMake = true;
            List<KeyValuePair<int,SkillUpgradeType>> da = new();
            Dictionary<int, SkillBaseStat> data = player.player.SkillData;
            foreach (var item in data)
            {
                int key = item.Key;
                KeyValuePair<int, SkillUpgradeType> value = new();

                item.Value.UpgradeAbleList.ForEach(upgrade =>
                {
                    if (upgrade == SkillUpgradeType.DownCost && !item.Value.IsDownCost())
                        value =new(key,upgrade);

                    else if (upgrade != SkillUpgradeType.DownCost)
                        value = new(key, upgrade);

                    upgradeList.Add(value);
                });
            }
        }

        Debug.Log(upgradeList.Count);
    }

    private void CardRotate(List<GameObject> cards)
    {
        foreach (GameObject card in cards)
        {
            card.GetComponent<Image>().color = Color.black;

            card.transform.DOPunchRotation(
                punch: punchValue,
                duration: durationValue,
                vibrato: vibratoValue,
                elasticity: elasticValue);
        }
    }

    private void ShuffleCards()
    {

    }
}
