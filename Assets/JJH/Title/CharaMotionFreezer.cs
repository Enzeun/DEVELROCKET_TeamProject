using UnityEngine;
using static UnityEngine.ParticleSystem;
using SF = UnityEngine.SerializeField;

public class CharaMotionFreezer : MonoBehaviour
{
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int MotionHash = Animator.StringToHash("Motion");

    [SF] private Animator monsterAnime;
    [SF] private Animator charaAnime;
    [SF] private Transform skillTransform;
    [SF] private ParticleSystem[] skillEffect;
    public float simulateTime = 1.5f;

    private void Awake()
    {
        monsterAnime.Play(MotionHash, 0, 0.44f);
        monsterAnime.speed = 0;
        charaAnime.Play(HorizontalHash, 0, 0.5f);
        charaAnime.speed = 0;

        foreach (ParticleSystem ps in skillEffect)
        {
            ps.Simulate(0.3f, true, true, false);
            ps.Pause();
        }

    }
}
