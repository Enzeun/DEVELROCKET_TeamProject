using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using static SkillEnums;
using SF = UnityEngine.SerializeField;
using HIn = UnityEngine.HideInInspector;
using Hovl;
public class SkillEffectSpawner : MonoBehaviour
{
    // 낙뢰 0.93초
    // 화염구 0.43초
    // 투사체는 히트판정으로
    [Serializable]
    public struct EffectBase
    {
        public string name;
        public float hitTime;
        public GameObject prefab;
    }

    [SF] private Transform PlayerTransform;
    [SF] private Transform PlayerStaffTransformByHorizontal;
    [SF] private Transform PlayerStaffTransformByVertical;
    [SF] private EffectBase[] prefabs;
    [SF] private PlayerCombat playerCombat;
    [SF] private Transform areaEffectTransform;
    private Dictionary<string, IObjectPool<GameObject>> effectPool;
    private Coroutine hitTimer;

    [HIn] public bool ProjectileHit;

    /// <summary>
    /// 스킬 사용 후, 적이 피격해 이펙트가 끝날 경우 작동
    /// </summary>
    public event Action OnEffectFinished;
    private string skillNameCk;

    private void Awake()
    {
        effectPool = new();

        foreach (var prefab in prefabs)
        {
            string effectName = prefab.name;

            IObjectPool<GameObject> effect = new ObjectPool<GameObject>(
                createFunc: CreateProjectile,
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                defaultCapacity: 5,
                maxSize: 10
            );

            effectPool[effectName] = effect;
        }

    }

    private GameObject CreateProjectile()
    {
        EffectBase baseCk = new();

        foreach (var item in prefabs)
        {
            if (item.name == skillNameCk)
            {
                baseCk = item; 
                break;
            }
        }

        GameObject dummy = Instantiate(baseCk.prefab, transform);

        if(dummy.TryGetComponent(out HS_ProjectileMover data))
            data.SetSpawner(this);
        
        return dummy;
    }

    /// <summary>
    /// 즉발 이펙트의 타격 이펙트가 발생할 때까지 피격 판정을 지연 타이머
    /// </summary>
    /// <param name="time">지연 시간</param>
    /// <returns></returns>
    public IEnumerator HitTarget(float time)
    {
        yield return new WaitForSeconds(time);
        ProjectileHit = true;
        OnEffectFinished?.Invoke();
    }

    /// <summary>
    /// 이펙트를 생성하는 함수
    /// </summary>
    /// <param name="skillName">호출하는 스킬 이름</param>
    /// <param name="data">스킬 정보</param>
    /// <param name="targetTransform">대상의 위치</param>
    public void SpawnEffect(string skillName, SkillBaseStat data, 
        Transform[] targetTransform)
    {
        foreach (var item in targetTransform)
        {
            GameObject obj = GetObject(skillName);
            Collider collider = item.GetComponent<Collider>();

            // 투사체인 경우 
            if (data.Pose == SkillPoseType.Vertical ||
                data.Pose == SkillPoseType.Horizontal)
            {
                Vector3 startPostion = Vector3.zero;
                if (data.Pose == SkillPoseType.Horizontal)
                    obj.transform.position = PlayerStaffTransformByHorizontal.position;
                else
                    obj.transform.position = PlayerStaffTransformByVertical.position;

                Vector3 colliderCenter = collider.bounds.center;

                Vector3 direction = new Vector3(item.position.x, colliderCenter.y, item.position.z) - obj.transform.position;
                Vector3 dir = direction.normalized;

                if (dir != Vector3.zero)
                    obj.transform.forward = dir;

                obj.SetActive(true);

                DOVirtual.DelayedCall(4, () => ReleaseObject(skillName, obj));
            }
            // 즉발 객체인 경우
            else
            {
                if (data.TargetSubType == SkillTargetSubType.Single)
                    obj.transform.position = areaEffectTransform.position;
                else
                {
                    Vector3 colliderCenter = collider.bounds.center;
                    float sizeY = collider.bounds.size.y / 2;
                    obj.transform.position =
                        new Vector3(item.position.x, colliderCenter.y - sizeY, item.position.z);
                }

                obj.SetActive(true);
                float hitTime = 0;

                foreach (var prefab in prefabs)
                {
                    if (skillName == prefab.name) hitTime = prefab.hitTime;
                }

                if (hitTimer != null) StopCoroutine(hitTimer);

                hitTimer = StartCoroutine(HitTarget(hitTime));

                DOVirtual.DelayedCall(2, () => ReleaseObject(skillName, obj));

                if (data.TargetSubType == SkillTargetSubType.Single) break;
            }
        }
    }

    /// <summary>
    /// 스킬 이펙트 Pooling.Get()
    /// </summary>
    /// <param name="skillName">사용하는 스킬 이름</param>
    /// <returns></returns>
    private GameObject GetObject(string skillName) {

        skillNameCk = skillName;
        GameObject obj = effectPool[skillName].Get();
        obj.SetActive(false);
        return obj;
    }

    /// <summary>
    /// 스킬 이펙트 Pooling.Release()
    /// </summary>
    /// <param name="skillName">사용하는 스킬 이름</param>
    /// <param name="obj">풀에 반환할 오브젝트</param>
    private void ReleaseObject(string skillName, GameObject obj)
    {
        effectPool[skillName].Release(obj);
        if(!ProjectileHit) OnEffectFinished?.Invoke();
        ProjectileHit = false;
    }
}
