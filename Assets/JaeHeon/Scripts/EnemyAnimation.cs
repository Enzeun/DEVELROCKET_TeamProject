using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;

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
    private float waitSec = 1f;

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
        ws = new WaitForSeconds(waitSec);
    }

    private void Start()
    {
        //targetTransform을 enemyBase에서  가져옴
        targetTransform = enemyBase.playerTransform;

        //시작할 때 현재 상태 저장
        currentTransform = transform;
        currentRotation = transform.rotation;
    }

 

    [Button] //발사 공격
    public void EnemyShootProjectile(Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - transform.position;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLookAt(targetTransform.position, 0.4f));
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            animator.SetTrigger("ProjectileAttack");
        });
        seq.AppendInterval(1f);
        seq.Append(transform.DORotate(currentRotation.eulerAngles, 0.4f));
    }

    // Event >> target 위치로 발사체 발사 (발사체 방향도 target 방향으로)
    public void EventEnemyEmissionProjectile()
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = EmissionTransform.position;

        Vector3 direction = (targetTransform.position - EmissionTransform.position).normalized;
        projectile.transform.forward = direction;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }

    [Button] //일반 공격(근접 공격)
    public void EnemyNormalAttack(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLookAt(targetTransform.position,0.4f));

        seq.Append(transform.DOMove((targetTransform.position - direction * distanceOfPlayerAtNormalAttack), enemyMoveSpeed));

        seq.AppendCallback(()=>
        {
            animator.SetTrigger("NormalAttack");
        });
        seq.AppendCallback(() =>
        {
            StartCoroutine(ApplyDamageRoutine());
        });
        seq.AppendInterval(1f);
        seq.Append(transform.DOMove(currentTransform.position, enemyMoveSpeed));
        seq.Append(transform.DORotate(currentRotation.eulerAngles, 0.4f));
    }

    private IEnumerator ApplyDamageRoutine()
    {
        Debug.Log("start routine");
        yield return ws;
        enemyBase.ApplyDamage();
    }

    [Button] //마법 공격
    public void EnemyCastSpell(Transform targetTransform)
    {
        Sequence seq = DOTween.Sequence();

        animator.SetTrigger("CastSpell");
        seq.Append(transform.DOLookAt(targetTransform.position, 0.4f));
        seq.AppendInterval(enemyMagicSpeed);
        seq.AppendCallback(() =>
        {
            GameObject spell = GameObject.Instantiate(spellPrefab);
            spell.transform.position = targetTransform.position;
        });
        seq.Append(transform.DORotate(currentRotation.eulerAngles, 0.4f));
    }

    //Event >> 해당 위치에서 플레이어 damage 받는 함수 제작
    public void EventEnemyCastSpell()
    {
        //player.TakeDamage(enemyBase.attackPower);
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
        });
        //버프 할 사항 추가
    }

    public void EnemyTakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    public void EnemyDie()
    {
        animator.SetTrigger("Die");
    }
}
