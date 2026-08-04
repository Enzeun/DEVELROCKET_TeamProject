using Unity.Mathematics;
using UnityEngine;

public class DrawCircleWhenMouseOver : MonoBehaviour
{
    private EnemyMarkerCircle enemyMarker;
    private Vector3 originalPosition;
    private void Start()
    {
        enemyMarker = FindAnyObjectByType<EnemyMarkerCircle>();
        originalPosition = enemyMarker.transform.position;
    }
    private void OnMouseEnter()
    {
        if (!enabled) return;
        //if (TurnManager.instance.currentState != TurnManager.TurnState.PlayerPlanning) return;
        enemyMarker.gameObject.transform.position = transform.position;
    }
    private void OnMouseExit()
    {
        if (!enabled) return;
        //if (TurnManager.instance.currentState != TurnManager.TurnState.PlayerPlanning) return;
        enemyMarker.gameObject.transform.position = originalPosition;
    }

}
