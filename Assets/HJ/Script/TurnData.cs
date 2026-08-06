using UnityEngine;

public class PlayerTurnData
{

    private SkillBaseStat _skill;
    private EnemyBase[] _target;

    public PlayerTurnData(SkillBaseStat skill, EnemyBase[] target)
    {        
        _skill = skill;
        _target = target;
    }
}
public class EnemyTurnData
{
    private EnemyBase casterEnemy;
    private Enemy_Behaviour enemy_Behaviour;


    public EnemyTurnData(EnemyBase caster, Enemy_Behaviour _enemy_Behaviour)
    {
        casterEnemy = caster;
        enemy_Behaviour = _enemy_Behaviour;
    }
}
