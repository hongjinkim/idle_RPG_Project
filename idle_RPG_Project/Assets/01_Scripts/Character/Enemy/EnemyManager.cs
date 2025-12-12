using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;

public enum EPoolType
{
    Enemy
    
}
public class EnemyManager : PoolManager<EnemyManager, EPoolType>
{
    [Title("Settings")]
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

    protected override async UniTask OnInitialize()
    {
        await base.OnInitialize();
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

        Vector2 randomDir = Random.insideUnitCircle.normalized;

        float randomDist = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector2 playerPos = Main.Battle.PlayerCharacter.transform.position;
        Vector2 spawnPos = playerPos + (Vector2)(randomDir * randomDist);

        GameObject enemyObj = Pop(EPoolType.Enemy, spawnPos);

        if (enemyObj != null)
        {
            Enemy newEnemy = enemyObj.GetComponent<Enemy>();

            newEnemy.Setup(100, 10);

            // 배틀 매니저에 등록 (카운팅용)
            Main.Battle.RegisterEnemy(newEnemy);
        }
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
