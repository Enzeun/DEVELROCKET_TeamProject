using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject player;
    //[SerializeField] private EnemyBase enemyBase;


    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform EmissionTransform;
    [SerializeField] private float projectileSpeed = 5f;

    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private GameObject buffPrefab;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        //enemyBase = GetComponent<EnemyBase>();
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

    [ContextMenu("AnimationShootProjectile")]
    public void EnemyShootProjectile(Transform targetTransform)
    {
        animator.SetTrigger("ProjectileAttack");
    }

    //
    //몬스터 방향을 먼저 플레어이 방향으로 돌리는 것도 필요한가
    //target 위치로 발사체 발사 (발사체 방향도 target 방향으로)
    [ContextMenu("EmissionProjectile")]
    public void EventEnemyEmissionProjectile(Transform targetTransform)
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = EmissionTransform.position;

        Vector3 direction = (targetTransform.position - projectile.transform.position).normalized;
        projectile.transform.forward = direction;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }

    public void EnemyNormalAttack()
    {
        animator.SetTrigger("StingAttack");
    }
    public void EnemyCastSpell(Transform targetTransform)
    {
        animator.SetTrigger("CastSpell");
    }

    public void EventEnemyCastSpell(Transform targetTransform)
    {
        //player.TakeDamage(enemyBase.attackPower);
    }

    

    public void EnemyBuff()
    {
        animator.SetTrigger("Buff");
    }

    public void EnemyDie()
    {
        animator.SetTrigger("Die");
    }
}
