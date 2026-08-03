using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using static SkillEnums;
using SF = UnityEngine.SerializeField;

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

    private Dictionary<string, IObjectPool<GameObject>> effectPool;
    private Coroutine hitTimer;

    private void Awake()
    {
        effectPool = new();

        foreach (var prefab in prefabs)
        {
            string effectName = prefab.name;

            IObjectPool<GameObject> effect = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab.prefab, transform),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                defaultCapacity: 5,
                maxSize: 10
            );

            effectPool[effectName] = effect;
        }

    }

    public IEnumerator HitTarget(float time)
    {
        yield return new WaitForSeconds(time);

        // 호출 UI 및 히트 판정 호출
    }

    public void SpawnEffect(string skillName, SkillPoseType pose, Transform targetTransform)
    {
        GameObject obj = GetObject(skillName);

        // 투사체인 경우 
        if (pose == SkillPoseType.Vertical || pose == SkillPoseType.Horizontal)
        {
            Vector3 startPostion = Vector3.zero;
            if (pose == SkillPoseType.Horizontal)
                obj.transform.position = PlayerStaffTransformByHorizontal.position;
            else
                obj.transform.position = PlayerStaffTransformByVertical.position;

            Vector3 direction = targetTransform.position - obj.transform.position;
            Vector3 dir = direction.normalized;

            if (dir != Vector3.zero)
                obj.transform.forward = dir;
            
            obj.SetActive(true);
        }
        // 즉발 객체인 경우
        else
        {
            obj.transform.position = targetTransform.position;
            obj.SetActive(true);
            float hitTime = 0;

            Debug.Log("엥?");
            foreach (var item in prefabs)
            {
                if (skillName == item.name) hitTime = item.hitTime;
            }

            if (hitTimer != null) StopCoroutine(hitTimer);

            hitTimer = StartCoroutine(HitTarget(hitTime));
        }

        DOVirtual.DelayedCall(4, () => ReleaseObject(skillName, obj));
    }

    public void SpawnEffect(Transform targetTransform)
    {
        GameObject obj = GetObject("단일1");

        obj.transform
                .SetPositionAndRotation(PlayerStaffTransformByHorizontal.position, 
                PlayerStaffTransformByHorizontal.rotation);
        obj.transform.LookAt(targetTransform);
        obj.SetActive(true);
        Vector3 direction = targetTransform.position - obj.transform.position;
        
        if (direction != Vector3.zero)
            obj.transform.rotation = Quaternion.LookRotation(direction);

        DOVirtual.DelayedCall(5, () => ReleaseObject("단일1", obj));
    }

    private GameObject GetObject(string skillName) {
        GameObject obj = effectPool[skillName].Get();
        obj.SetActive(false);
        return obj;
    }

    private void ReleaseObject(string skillName, GameObject obj)
    {
        effectPool[skillName].Release(obj);
    }
}
