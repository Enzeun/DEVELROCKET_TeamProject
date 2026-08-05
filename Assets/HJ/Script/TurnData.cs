using UnityEngine;

public class PlayerTurnData
{

    private SkillBaseStat skill;
    private EnemyBase target;

    public PlayerTurnData(SkillBaseStat _skill, EnemyBase _target)
    {        
        skill = _skill;
        target = _target;
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
