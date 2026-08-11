
using UnityEngine;
using System;

public class DrawCircleWhenMouseOver : MonoBehaviour
{
    private EnemyMarkerCircle enemyMarker;
    private Vector3 originalPosition;
    EnemyBase enemy;
    private void Start()
    {
        enemyMarker = FindAnyObjectByType<EnemyMarkerCircle>();
        if (enemyMarker == null)
        {
            enabled = false;
            return;
        }
        originalPosition = enemyMarker.transform.position;


        enemy = GetComponent<EnemyBase>();

        if (enemy == null)
        {
            Debug.Log("잘못된 object에 붙어있습니다. 확인하세요");
            return;
        }
    }
    private void OnMouseEnter()
    {
        if (enemy.isDead) return;

        if (!enabled || TurnManager.instance.currentState != TurnManager.TurnState.PlayerPlanning)
        {
            enemyMarker.gameObject.transform.position = originalPosition;
            return;
        }

        enemyMarker.gameObject.transform.position = transform.position;
    }
    private void OnMouseExit()
    {
        if (!enabled || TurnManager.instance.currentState != TurnManager.TurnState.PlayerPlanning)
        {
            enemyMarker.gameObject.transform.position = originalPosition;
            return;
        }

        enemyMarker.gameObject.transform.position = originalPosition;
    }
    private void OnMouseDown()
    {
        if (enemy.isDead) return;

        if (!enabled || TurnManager.instance.currentState != TurnManager.TurnState.PlayerPlanning)
        {
            enemyMarker.gameObject.transform.position = originalPosition;
            return;
        }


        OnEnemyClicked?.Invoke(enemy);
    }

    public void InitCircleLocation()
    {
        enemyMarker.gameObject.transform.position = originalPosition;
    }

    public Action<EnemyBase> OnEnemyClicked;
}
