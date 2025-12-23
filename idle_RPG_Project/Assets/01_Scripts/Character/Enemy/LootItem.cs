using DG.Tweening;
using System.Collections;
using System.Numerics;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;


public class LootItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ELootType _lootType;
    private BigInteger _amount = 100000; // 재화량
    [SerializeField] private float spreadForce = 2; // 처음에 튀는 힘
    [SerializeField] private float suckDelay = 0.5f; // 빨려가기 전 대기 시간
    [SerializeField] private float suckAcceleration = 15f; // 빨려가는 가속도

    [Header("Visual Components")]
    [SerializeField] private SpriteRenderer mainRenderer;  // 아이콘
    //[SerializeField] private SpriteRenderer glowRenderer;  // 뒤에 후광

    private Transform _target; // 빨려갈 대상 (플레이어)
    private Coroutine _suckCoroutine;
    private Poolable _pool;

    public ELootType LootType => _lootType;
    public BigInteger Amount => _amount;

    private void Awake()
    {
        if(_pool == null)
            _pool = GetComponent<Poolable>();
    }

    public void Initialize(LootItemDataSO data, BigInteger amount, Vector3 startPos, Transform target)
    {
        // 1. 데이터 설정
        _lootType = data.type;
        _amount = amount;
        _target = target;

        // 2. 비주얼 교체 (스프라이트 갈아끼우기)
        mainRenderer.sprite = data.iconSprite;
        transform.localScale = Vector3.one * data.scale;

        // 3. 색상 교체
        //if (glowRenderer != null) glowRenderer.color = data.effectColor;

        // 4. 위치 잡고 활성화
        transform.position = startPos;
        gameObject.SetActive(true);

        // 5. 연출 시작
        SpawnAnimation();
    }

    private void SpawnAnimation()
    {
        // 1. Spread: 랜덤한 방향으로 살짝 튀어오름
        Vector3 randomDir = Random.insideUnitCircle.normalized;
        // 위쪽으로 조금 더 튀게 보정
        randomDir.y = Mathf.Abs(randomDir.y) + 0.5f;

        Vector3 spreadPos = transform.position + randomDir * spreadForce;

        // 점프하듯이 이동 (Jump, Arc 표현)
        transform.DOJump(spreadPos, 1.5f, 1, suckDelay)
            .SetEase(Ease.OutQuad)
            .OnComplete(StartSuckRoutine);

        // 등장 시 크기 펑! 튀기기
        transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
    }

    private void StartSuckRoutine()
    {
        if (_suckCoroutine != null) StopCoroutine(_suckCoroutine);
        _suckCoroutine = StartCoroutine(SuckRoutine());
    }

    private IEnumerator SuckRoutine()
    {
        yield return new WaitForSeconds(0.1f); // 아주 잠깐 대기

        float currentSpeed = 0f;

        // 타겟과의 거리가 가까워질 때까지 반복
        while (Vector3.Distance(transform.position, _target.position) > 0.5f)
        {
            // 타겟이 없거나 죽었으면 중단
            if (_target == null || !_target.gameObject.activeInHierarchy)
            {
                ReturnToPool();
                yield break;
            }

            // 점점 빨라지게 가속도 적용
            currentSpeed += suckAcceleration * Time.deltaTime;

            // 타겟을 향해 이동 (MoveTowards로 매 프레임 추적)
            transform.position = Vector3.MoveTowards(
                transform.position,
                _target.position,
                currentSpeed * Time.deltaTime
            );

            yield return null; // 다음 프레임까지 대기
        }

        Collect();
    }

    private void Collect()
    {
        // 획득 사운드 재생 (EffectManager 등 활용)
        // EffectManager.Instance.PlaySound("LootCollect");

        // 실제 재화 증가 처리
        MainSystem.Instance.Player.ChangeGold(_amount);

        // 획득 시 약간 작아지면서 사라지는 연출
        transform.DOScale(0, 0.2f).SetEase(Ease.InBack).OnComplete(ReturnToPool);
    }

    private void ReturnToPool()
    {
        // DOTween 안전하게 종료
        transform.DOKill();
        if (_suckCoroutine != null) StopCoroutine(_suckCoroutine);

        // 매니저를 통해 반납 (아직 안 만들었으니 비활성화만)
        if (_pool != null)
            _pool.Release();

    }
}