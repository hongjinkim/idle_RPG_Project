using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;
public class EnemySpawner : BaseManager
{
    [Title("Settings")]
    [SerializeField] private Enemy enemyPrefab; // 프리팹 연결
    [SerializeField] private float spawnInterval = 1.0f; // 젠 시간
    [SerializeField, MinValue(1)] private int maxEnemyCount = 100;
    [SerializeField, MinValue(0)] private int initialSpawnCount = 10;

    [Title("Radius Settings")]
    [InfoBox("최소 반경은 카메라 화면 대각선 길이보다 커야 화면 밖에서 생성됩니다.")]
    [SerializeField] private float minSpawnRadius = 10.0f; // 화면 밖 (최소 거리)
    [SerializeField] private float maxSpawnRadius = 14.0f; // 최대 거리

    [Title("Debug")]
    [ShowInInspector, ReadOnly] private bool _isSpawning = false;

    [ShowInInspector, ProgressBar(0, "maxEnemyCount")]
    private int CurrentCount => Main.Battle != null ? Main.Battle.EnemyCount : 0;

    protected override async UniTask OnInitialize()
    {
        if (initialSpawnCount > 0)
        {
            for (int i = 0; i < initialSpawnCount; i++)
            {
                SpawnEnemy();
            }
            Debug.Log($"Initial Spawn: {initialSpawnCount} enemies created.");
        }

        // 2. 정기 스폰 시작
        StartSpawning();
        await UniTask.CompletedTask;
    }

    public void StartSpawning()
    {
        if (_isSpawning) return;
        _isSpawning = true;
        SpawnLoop().Forget();
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    private async UniTaskVoid SpawnLoop()
    {
        while (_isSpawning)
        {
            if (Main.Battle.EnemyCount < maxEnemyCount)
            {
                SpawnEnemy();
            }
            // UniTask로 딜레이 (밀리초 단위 변환)
            await UniTask.Delay((int)(spawnInterval * 1000));
        }
    }

    private void SpawnEnemy()
    {
        // 플레이어가 없거나 죽었으면 생성 중단
        if (Main.Battle.PlayerCharacter == null || Main.Battle.PlayerCharacter.State == EntityState.Dead) return;

        // 1. 랜덤 방향 벡터 구하기 (길이 1인 원의 테두리 좌표)
        Vector2 randomDir = Random.insideUnitCircle.normalized;

        // 2. 랜덤 거리 구하기 (최소 ~ 최대 사이)
        float randomDist = Random.Range(minSpawnRadius, maxSpawnRadius);

        // 3. 최종 생성 위치 계산 (플레이어 위치 기준)
        Vector2 playerPos = Main.Battle.PlayerCharacter.transform.position;
        Vector2 spawnPos = playerPos + (Vector2)(randomDir * randomDist);

        // 4. 생성 및 초기화
        Enemy newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 스탯 설정 (나중에는 스테이지 데이터에 따라 다르게 설정)
        newEnemy.Setup(200, 10);

        // 매니저에 등록
        Main.Battle.RegisterEnemy(newEnemy);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && Main.Battle.PlayerCharacter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Main.Battle.PlayerCharacter.transform.position, minSpawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Main.Battle.PlayerCharacter.transform.position, maxSpawnRadius);
        }
    }
}
