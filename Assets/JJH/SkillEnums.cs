
public static class SkillEnums
{
    // 스킬이 가질 수 있는 전체 업그레이드 종류
    public enum SkillUpgradeType { Damage, DownCost, LifeStill, Overcharge, WideRange, Overpower }

    // 스킬이 지정할 수 있는 타겟 범위
    public enum SkillTargetType { Single, Area }

    // 스킬이 발사하는 투세체의 종류
    public enum SkillTargetSubType { Single, Multi }

    // 스킬이 사용하는 플레이어의 스킬 시전 자세
    public enum SkillPoseType { Instance, Horizontal, Vertical, Stomp, LongCast }
}
