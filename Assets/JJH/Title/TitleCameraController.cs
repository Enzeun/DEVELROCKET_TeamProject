using UnityEngine;

using SF = UnityEngine.SerializeField;

public class TitleCameraController : MonoBehaviour
{
    [Header("공전 설정")]
    [SF] private Transform target;
    [SF] private Vector3 orbitAxis = Vector3.up;
    [SF] private float orbitSpeed = 1f;
    [SF] private float maxAngle = 10f;

    [Header("카메라 회전속도 Ease")]
    [SF] private AnimationCurve ease;

    private float _accumulatedAngle = 0f;
    private int _direction = 1;

    void Update()
    {
        if (target == null) return;

        float currentProgress = _accumulatedAngle / maxAngle;
        float speedMultiplier = ease.Evaluate(currentProgress);
        float angleToRotate = orbitSpeed * Time.deltaTime * _direction * speedMultiplier;

        transform.RotateAround(target.position, orbitAxis, angleToRotate);

        _accumulatedAngle += angleToRotate;

        if (_direction == 1 && _accumulatedAngle >= maxAngle)
            _direction = -1;
        
        else if (_direction == -1 && _accumulatedAngle <= 0f)
            _direction = 1;
    }
}
