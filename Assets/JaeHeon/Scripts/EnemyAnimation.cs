using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    //[SerializeField] private GameObject player;
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] Transform targetTransform;
    [SerializeField] float enemyMoveSpeed = 1f;
    [SerializeField] float enemyMagicSpeed = 0.4f;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform EmissionTransform;
    [SerializeField] private float projectileSpeed = 20f;

    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private GameObject buffPrefab;
    [SerializeField] private float distanceOfPlayerAtNormalAttack = 2f;

    Transform currentTransform;
    Quaternion currentRotation;

    /// <summary>
    /// doRotation, doLookAt할 때 걸리는 시간은 임의적으로 집어넣었음
    /// </summary>


    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyBase = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        targetTransform = enemyBase.playerTransform;

        currentTransform = transform;
        currentRotation = transform.rotation;
    }

    public void PlayerTakeDamage()
    {
        //player.TakeDamage(enemyBase.attackPower);
        Debug.Log("PlayerTakeDamage");
    }

    public void EnemyTakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    [Button]
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

    //
    //몬스터 방향을 먼저 플레어이 방향으로 돌리는 것도 필요한가
    //target 위치로 발사체 발사 (발사체 방향도 target 방향으로)
    public void EventEnemyEmissionProjectile()
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = EmissionTransform.position;

        Vector3 direction = (targetTransform.position - EmissionTransform.position).normalized;
        projectile.transform.forward = direction;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }

    [Button]
    public void EnemyNormalAttack(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLookAt(targetTransform.position,0.4f));

        seq.Append(transform.DOMove((targetTransform.position - direction * distanceOfPlayerAtNormalAttack), enemyMoveSpeed));

        seq.AppendCallback(()=>
        {
            animator.SetTrigger("StingAttack");
        });

        seq.AppendInterval(1f);
        seq.Append(transform.DOMove(currentTransform.position, enemyMoveSpeed));
        seq.Append(transform.DORotate(currentRotation.eulerAngles, 0.4f));
    }
    [Button]
    public void EnemyCastSpell(Transform targetTransform)
    {
        Sequence seq = DOTween.Sequence();

        animator.SetTrigger("CastSpell");
        seq.AppendInterval(enemyMagicSpeed);
        seq.AppendCallback(() =>
        {
            GameObject spell = GameObject.Instantiate(spellPrefab);
            spell.transform.position = targetTransform.position;
        });
    }

    public void EventEnemyCastSpell()
    {
        //해당 위치에서 플레이어 damage 받는 함수 제작
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
        
    }

    public void EnemyDie()
    {
        animator.SetTrigger("Die");
    }
}
