using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int projectileDamage;
    [SerializeField] private bool isCalc = false;
    public PlayerCombat combat;

    private void OnDisable()
    {
        if (combat == null)
            return;
        combat.player.TakeDamage(projectileDamage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (projectileDamage == 0)
        {
            Debug.Log("Damage 0");
            Destroy(this.gameObject);
            return;
        }

        if (isCalc)
            return;

        if (other.transform.TryGetComponent<PlayerCombat>(out PlayerCombat combat))
        {
            isCalc = true;
            //combat.player.TakeDamage(projectileDamage);
            Destroy(this.gameObject);
        }
    }
}
