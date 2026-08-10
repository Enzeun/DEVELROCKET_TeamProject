using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static SkillEnums;
using SF = UnityEngine.SerializeField;
using HIn = UnityEngine.HideInInspector;
using Hovl;

public class SkillEffectSpawner : MonoBehaviour
{
    #region 베이스 데이터
    // 낙뢰 0.93초
    // 화염구 0.43초
    // 투사체는 히트판정으로
    [Serializable]
    public struct EffectBase
    {
        public int id;
        public float hitTime;
        public GameObject prefab;
    }
    #endregion

    #region 직렬화 데이터
    [Header("플레이어 관련")]
    [SF] private Transform PlayerTransform;
    [SF] private Transform PlayerStaffTransformByHorizontal;
    [SF] private Transform PlayerStaffTransformByVertical;

    [Header("이펙트 관련")]
    [SF] private EffectBase[] prefabs;

    [Header("스크립트")]
    [SF] private PlayerCombat playerCombat;

    [Header("기타")]
    [SF] private Transform areaEffectTransform;

    #endregion

    #region 필드 변수
    private Dictionary<int, IObjectPool<GameObject>> effectPool;
    private Coroutine hitTimer;

    [HIn] public bool ProjectileHit;
    private int skillIdCk;

    /// <summary>
    /// 스킬 사용 후, 적이 피격해 이펙트가 끝날 경우 작동
    /// </summary>
    public event Action OnEffectFinished;
    #endregion

    private void Awake()
    {
        effectPool = new();

        foreach (var prefab in prefabs)
        {
            int effectId = prefab.id;

            //오브젝트 풀링(각 스킬 별로 별도 생성)
            IObjectPool<GameObject> effect = new ObjectPool<GameObject>(
                createFunc: CreateProjectile,
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                defaultCapacity: 5,
                maxSize: 10
            );

            effectPool[effectId] = effect;
        }

    }

    /// <summary>
    /// 오브젝트 풀링 전용 스킬 이펙트 생성 함수
    /// </summary>
    /// <returns></returns>
    private GameObject CreateProjectile()
    {
        EffectBase baseCk = new();

        foreach (var item in prefabs)
        {
            if (item.id == skillIdCk)
            {
                baseCk = item; 
                break;
            }
        }

        GameObject dummy = Instantiate(baseCk.prefab, transform);

        // 생성된 복제체에 담긴 HS_ProjectileMover 스크립트에 SkillEffectSpawner 주입 
        if (dummy.TryGetComponent(out HS_ProjectileMover data))
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
    /// <param name="id">호출하는 스킬의 아이디</param>
    /// <param name="data">스킬 정보</param>
    /// <param name="targetTransform">대상의 위치</param>
    public void SpawnEffect(int id, SkillBaseStat data, 
        Transform[] targetTransform)
    {
        foreach (var item in targetTransform)
        {
            GameObject obj = GetObject(id);
            Collider collider = item.GetComponent<Collider>();

            // 투사체인 경우 
            if (data.Pose == SkillPoseType.Vertical ||
                data.Pose == SkillPoseType.Horizontal)
            {
                Vector3 startPostion = Vector3.zero;

                // 발사 위치 조정
                if (data.Pose == SkillPoseType.Horizontal)
                    obj.transform.position = PlayerStaffTransformByHorizontal.position;
                else
                    obj.transform.position = PlayerStaffTransformByVertical.position;

                // 몬스터 중앙 확인
                Vector3 colliderCenter = collider.bounds.center;

                Vector3 direction = new Vector3(item.position.x, colliderCenter.y, item.position.z) - obj.transform.position;
                Vector3 dir = direction.normalized;

                if (dir != Vector3.zero)
                    obj.transform.forward = dir;

                obj.SetActive(true);

                // 투사체 삭제 딜레이
                DOVirtual.DelayedCall(4, () => ReleaseObject(id, obj));
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
                    if (id == prefab.id) hitTime = prefab.hitTime;
                }

                if (hitTimer != null) StopCoroutine(hitTimer);

                hitTimer = StartCoroutine(HitTarget(hitTime));

                DOVirtual.DelayedCall(2, () => ReleaseObject(id, obj));

                if (data.TargetSubType == SkillTargetSubType.Single) break;
            }
        }
    }

    /// <summary>
    /// 스킬 이펙트 Pooling.Get()
    /// </summary>
    /// <param name="skillName">사용하는 스킬 이름</param>
    /// <returns></returns>
    private GameObject GetObject(int id) {

        skillIdCk = id;
        GameObject obj = effectPool[id].Get();
        obj.SetActive(false);
        return obj;
    }

    /// <summary>
    /// 스킬 이펙트 Pooling.Release()
    /// </summary>
    /// <param name="id">사용하는 스킬 아이디</param>
    /// <param name="obj">풀에 반환할 오브젝트</param>
    private void ReleaseObject(int id, GameObject obj)
    {
        // 복제체 위치를 스포너로 복귀
        obj.transform.position = transform.position;

        // 복제체 릴리즈
        effectPool[id].Release(obj);

        // 이펙트 종료 이벤트 호출
        if(!ProjectileHit) OnEffectFinished?.Invoke();

        ProjectileHit = false;
    }
}
