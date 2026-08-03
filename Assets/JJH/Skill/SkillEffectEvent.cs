using UnityEngine;
using SF = UnityEngine.SerializeField;
public class SkillEffectEvent: MonoBehaviour
{
    [SF] private PlayerCombat playerCombat;

    public void StartProjectileEvent()
    {
        playerCombat.EffectActive();
    }
}
