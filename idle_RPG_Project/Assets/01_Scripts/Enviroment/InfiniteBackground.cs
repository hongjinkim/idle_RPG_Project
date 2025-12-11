using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float tileSize = 16f;

    private Material _material;
    private static readonly int OffsetID = Shader.PropertyToID("_Offset");

    void Start()
    {
        _material = GetComponent<Renderer>().material;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Quad 위치: 카메라를 따라다님
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            10f
        );

        // 2. 오프셋 계산 (비율 보정)
        Vector2 offset = new Vector2(
            cameraTransform.position.x / tileSize,
            cameraTransform.position.y / tileSize
        );

        // 쉐이더 그래프의 _Offset 변수에 적용
        _material.SetVector(OffsetID, offset);
    }
}