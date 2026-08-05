using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;

/// <summary>
/// 
/// ==Animation Parameter==
/// 
/// NormalAttack
/// CastSpell
/// Die
/// Buff
/// TakeDamage
/// ProjectileAttack
/// 
/// </summary>

public class EnemyAnimation : MonoBehaviour
{
    [BoxGroup("받아올 컴포넌트"), SerializeField]
    private Animator animator;
    [BoxGroup("받아올 컴포넌트"), SerializeField]
    private EnemyBase enemyBase;
    [BoxGroup("받아올 컴포넌트"), SerializeField]
    private Transform targetTransform;

    [BoxGroup("넣어줄 컴포넌트"), SerializeField]
    private GameObject projectilePrefab;
    [BoxGroup("넣어줄 컴포넌트"), SerializeField]
    private GameObject projectileEffectPrefab;
    [BoxGroup("넣어줄 컴포넌트"), SerializeField]
    private Transform EmissionTransform;
    [BoxGroup("넣어줄 컴포넌트"), SerializeField]
    private GameObject spellPrefab;
    [BoxGroup("넣어줄 컴포넌트"), SerializeField]
    private GameObject buffPrefab;

    [BoxGroup("조작가능한 필드"), SerializeField]
    float enemyMagicSpeed = 0.4f;
    [BoxGroup("조작가능한 필드"), SerializeField]
    float enemyMoveSpeed = 1f;
    [BoxGroup("조작가능한 필드"), SerializeField] 
    private float projectileSpeed = 20f;
    [BoxGroup("조작가능한 필드"), SerializeField] 
    private float distanceOfPlayerAtNormalAttack = 2f;
    [BoxGroup("조작가능한 필드"), SerializeField]
    private float rotateInterval = 0.4f;
    [BoxGroup("조작가능한 필드"), SerializeField]
    private float waitInterval = 0.4f;
    [BoxGroup("조작가능한 필드"), SerializeField]
    private float animatingInterval = 1f;
    [BoxGroup("조작가능한 필드"), SerializeField]
    private float destroyEffectInterval = 2f;
    [BoxGroup("조작가능한 필드"), SerializeField]
    private float applyDamageInterval = 1f;

    //시작할 때 현재 상태 저장용 필드
    Transform currentTransform;
    Quaternion currentRotation;
    WaitForSeconds ws;

    /// <summary>
    /// doRotation, doLookAt할 때 걸리는 시간은 임의적으로 집어넣었음
    /// </summary>


    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyBase = GetComponent<EnemyBase>();
        ws = new WaitForSeconds(applyDamageInterval);
    }

    private void Start()
    {
        //targetTransform을 enemyBase에서  가져옴
        InitPlayerTransform();

        //시작할 때 현재 상태 저장
        currentTransform = transform;
        currentRotation = transform.rotation;
    }

    private void InitPlayerTransform()
    {
        if (targetTransform == null)
        {
            targetTransform = FindFirstObjectByType<PlayerCombat>().transform;
        }
    }

 

    [Button] //발사 공격
    public void EnemyShootProjectile(Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - transform.position;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLookAt(targetTransform.position, rotateInterval));
        seq.AppendInterval(waitInterval);
        seq.AppendCallback(() =>
        {
            animator.SetTrigger("ProjectileAttack");
        });
        seq.AppendInterval(animatingInterval);
        seq.Append(transform.DORotate(currentRotation.eulerAngles, rotateInterval));
    }

    // Event >> target 위치로 발사체 발사 (발사체 방향도 target 방향으로)
    // projectile 말고도 EffectProjectile 넣어주면 좋음
    public void EventEnemyEmissionProjectile()
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = EmissionTransform.position;
        Vector3 direction = (targetTransform.position - EmissionTransform.position).normalized;
        projectile.transform.forward = direction;
        EffectDestroy(projectile, destroyEffectInterval);

        if (projectileEffectPrefab != null)
        {
            GameObject effectProjectile = GameObject.Instantiate(projectileEffectPrefab);
            effectProjectile.transform.position = transform.position;
            Vector3 effectDirection = (targetTransform.position - transform.position).normalized;
            effectProjectile.transform.forward = effectDirection;
            EffectDestroy(effectProjectile, destroyEffectInterval);
        }
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }

    [Button] //일반 공격(근접 공격)
    public void EnemyNormalAttack(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLookAt(targetTransform.position, rotateInterval));
        seq.AppendInterval(waitInterval);
        seq.Append(transform.DOMove((targetTransform.position - direction * distanceOfPlayerAtNormalAttack), enemyMoveSpeed));

        seq.AppendCallback(()=>
        {
            animator.SetTrigger("NormalAttack");
        });
        seq.AppendCallback(() =>
        {
            StartCoroutine(ApplyDamageRoutine());
        });
        seq.AppendInterval(animatingInterval);
        seq.Append(transform.DOMove(currentTransform.position, enemyMoveSpeed));
        seq.AppendInterval(waitInterval);
        seq.Append(transform.DORotate(currentRotation.eulerAngles, rotateInterval));
    }

    private IEnumerator ApplyDamageRoutine()
    {
        yield return ws;
        Debug.Log("start routine");
        enemyBase.ApplyDamage();
    }

    [Button] //마법 공격
    public void EnemyCastSpell(Transform targetTransform)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLookAt(targetTransform.position, rotateInterval));
        seq.AppendCallback(() =>
        {
            animator.SetTrigger("CastSpell");
        });
        seq.AppendInterval(enemyMagicSpeed);
        seq.AppendCallback(() =>
        {
            GameObject spell = GameObject.Instantiate(spellPrefab);
            spell.transform.position = targetTransform.position;
            EffectDestroy(spell, destroyEffectInterval);
        });
        seq.AppendInterval(animatingInterval);
        seq.Append(transform.DORotate(currentRotation.eulerAngles, rotateInterval));
    }

    //Event >> 해당 위치에서 플레이어 damage 받는 함수 제작
    public void EventEnemyCastSpell()
    {
        StartCoroutine(ApplyDamageRoutine());
    }


    [Button]
    public void EnemyBuff()
    {
        Sequence seq = DOTween.Sequence();
        animator.SetTrigger("Buff");
        seq.AppendInterval(enemyMagicSpeed);
        seq.AppendCallback(() =>
        {
            GameObject buffEffect = GameObject.Instantiate(buffPrefab);
            buffEffect.transform.position = transform.position;

            EffectDestroy(buffEffect, destroyEffectInterval);
        });
        //버프 할 사항 추가
    }

    private void EffectDestroy(GameObject effect, float destroyTime)
    {
        Destroy(effect, destroyTime);
    }

    public void EnemyTakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    [Button]
    public void EnemyDie()
    {
        animator.SetTrigger("Die");
    }
}
