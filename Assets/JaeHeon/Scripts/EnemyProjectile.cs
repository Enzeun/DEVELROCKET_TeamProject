using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int projectileDamage;
    private bool isCalc = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (projectileDamage == 0)
            return;

        if (isCalc)
            return;

        if(collision.transform.TryGetComponent<PlayerCombat>(out PlayerCombat combat))
        {
            isCalc = true;
            combat.player.TakeDamage(projectileDamage);
            Destroy(this.gameObject);
        }
    }
}
