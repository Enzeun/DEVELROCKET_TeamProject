using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using SF = UnityEngine.SerializeField;

public class CharaMotionFreezer : MonoBehaviour
{
    #region 애니메이션 클립 해시
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int MotionHash = Animator.StringToHash("Motion");
    #endregion

    #region 베이스 데이터
    [Serializable]
    private class EffectSet
    {
        public ParticleSystem particle;
        public float stopValue;
    }
    #endregion

    #region 직렬화 데이터
    [SF] private Animator monsterAnime;
    [SF] private Animator charaAnime;
    [SF] private EffectSet[] skillEffect;
    #endregion

    private void Awake()
    {
        // 몬스터 고정
        monsterAnime.Play(MotionHash, 0, 0.44f);
        monsterAnime.speed = 0;

        //플레이어 캐릭터 고정
        charaAnime.Play(HorizontalHash, 0, 0.5f);
        charaAnime.speed = 0;

        // 스킬 이펙트 파티클 고정
        foreach (var ps in skillEffect)
        {
            ps.particle.Simulate(ps.stopValue, true, true, false);

            ps.particle.Pause();
        }
    }
}
