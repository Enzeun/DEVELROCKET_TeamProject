using UnityEngine;

public class PlayerTurnData
{

    private SkillBaseStat _skill;
    public SkillBaseStat skill { get => _skill; }
    private EnemyBase[] _target;
    public EnemyBase[] target { get => _target; }

    public PlayerTurnData(SkillBaseStat skill, EnemyBase[] target)
    {
        _skill = skill;
        _target = target;
    }
}
public class EnemyTurnData
{
    public EnemyBase casterEnemy { get; private set; }
    public Enemy_Behaviour enemy_Behaviour { get; private set; }


    public EnemyTurnData(EnemyBase caster, Enemy_Behaviour _enemy_Behaviour)
    {
        casterEnemy = caster;
        enemy_Behaviour = _enemy_Behaviour;
    }
}
