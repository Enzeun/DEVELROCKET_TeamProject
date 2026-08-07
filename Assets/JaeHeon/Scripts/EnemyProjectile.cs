using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int projectileDamage;
    [SerializeField] private bool isCalc = false;

    private void OnTriggerEnter(Collider other)
    {
        if (projectileDamage == 0)
        {
            Debug.Log("Damage 0");
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
