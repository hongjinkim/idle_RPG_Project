using UnityEngine;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    [Title("Target Settings")]
    private Transform target; // 플레이어 연결
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // Z는 무조건 -10

    [Title("Motion Settings")]
    [SerializeField] private float smoothTime = 0.2f; // 지연 시간 (0에 가까울수록 딱딱하게 붙음)

    // 내부 연산 변수
    private Vector3 _currentVelocity;

    // --- 화면 흔들림(Shake) 변수 ---
    private float _shakeDuration = 0f;
    private float _shakeMagnitude = 0.1f;
    private float _initialShakeDuration;

    private void Start()
    {
        if (target == null)
        {
            target = MainSystem.Battle.MainHero.transform;
        }

    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. 목표 위치 계산 (플레이어 + 오프셋)
        Vector3 targetPosition = target.position + offset;

        // 2. Z축 강제 고정 (2D 배경이 안 사라지게)
        // 오프셋의 Z값을 그대로 유지하거나, -10으로 고정
        targetPosition.z = offset.z;

        // 3. 부드러운 이동 (SmoothDamp)
        // Lerp보다 물리적으로 훨씬 자연스러운 감속을 제공합니다.
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);

        // 4. 화면 흔들림 적용
        if (_shakeDuration > 0)
        {
            // 랜덤한 위치를 더해줌 (Sphere * 강도)
            Vector3 shakeOffset = Random.insideUnitSphere * _shakeMagnitude;
            // Z축은 흔들리면 안 됨 (화면 깜빡임 방지)
            shakeOffset.z = 0;

            smoothedPosition += shakeOffset;

            _shakeDuration -= Time.deltaTime;
        }

        transform.position = smoothedPosition;
    }

    // --- 외부에서 호출할 함수 (Screen Shake) ---
    [Button("Test Shake")]
    public void Shake(float duration, float magnitude)
    {
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
        _initialShakeDuration = duration;
    }
}