using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using SF = UnityEngine.SerializeField;

public class CharaMotionFreezer : MonoBehaviour
{
    [Serializable]
    private class EffectSet
    {
        public ParticleSystem particle;
        public float stopValue;
    }

    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int MotionHash = Animator.StringToHash("Motion");

    [SF] private Animator monsterAnime;
    [SF] private Animator charaAnime;
    [SF] private EffectSet[] skillEffect;

    private void Awake()
    {
        monsterAnime.Play(MotionHash, 0, 0.44f);
        monsterAnime.speed = 0;
        charaAnime.Play(HorizontalHash, 0, 0.5f);
        charaAnime.speed = 0;

        foreach (var ps in skillEffect)
        {
            ps.particle.Simulate(ps.stopValue, true, true, false);

            ps.particle.Pause();
        }
    }
}
