using DG.Tweening;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static SkillEnums;

public class PropertyCardActive : MonoBehaviour
{
    [BoxGroup(""), SerializeField]
    private PlayerCombat player;
    [BoxGroup(""), SerializeField]
    List<GameObject> cards = new();
    [BoxGroup(""), SerializeField]
    Dictionary<int, List<SkillUpgradeType>> upgradeList = new();
    [BoxGroup(""), SerializeField]
    private bool isMake = false;


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

    private void MakeCardList()
    {
        if(isMake)
        {
            isMake = true;

            //upgradeList = new();
            Dictionary<int, SkillBaseStat> data = player.player.SkillData;
            foreach (var item in data)
            {
                int key = item.Key;
                upgradeList[key] = new List<SkillUpgradeType>();

                item.Value.UpgradeAbleList.ForEach(upgrade =>
                {
                    if (upgrade == SkillUpgradeType.DownCost && !item.Value.IsDownCost())
                        upgradeList[key].Add(upgrade);

                    else if (upgrade != SkillUpgradeType.DownCost)
                        upgradeList[key].Add(upgrade);
                });
            }
        }

        Debug.Log(upgradeList.Count);



    }

    private void CardRotate(List<GameObject> cards)
    {
        foreach (GameObject card in cards)
        {
            card.transform.DOPunchRotation(
                punch: punchValue,
                duration: durationValue,
                vibrato: vibratoValue,
                elasticity: elasticValue);
        }
    }
}
