using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class EnemyPoolManager : PoolManager<EnemyPoolManager, EPoolType>
{
    [Title("Settings")]
    [SerializeField] private int currnetStage;
    [SerializeField] private float spawnInterval = 1.0f; // 젠 시간
    [SerializeField] private int maxEnemyCount = 100;
    [SerializeField] private int initialSpawnCount = 10;
    private List<EnemyValue> EnemyList = new List<EnemyValue>(); // 스폰 가능한 적 ID 리스트
    private EnemyValue EnemyBoss;

    private float minSpawnRadius; // 화면 밖 (최소 거리)
    private float maxSpawnRadius; // 최대 거리

    [Title("Debug")]
    [ShowInInspector, ReadOnly] private bool _isSpawning = false;


    protected override async UniTask OnInitialize()
    {
        await base.OnInitialize();

        currnetStage = MainSystem.Battle.CurrentStage;

        // 배틀 매니저에서 반경 정보 가져오기
        minSpawnRadius = MainSystem.Battle.minSpawnRadius;
        maxSpawnRadius = MainSystem.Battle.maxSpawnRadius;

        GetEnemyValue();

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

    private void GetEnemyValue()
    {
        EnemyList = MainSystem.Data.Stage.GetEnemyListForStage(currnetStage);
        EnemyBoss = MainSystem.Data.Stage.GetEnemyBossForStage(currnetStage);
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
            if (MainSystem.Battle.EnemyCount < maxEnemyCount)
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
        if (MainSystem.Battle.PlayerHero == null || MainSystem.Battle.PlayerHero.State == ECharacterState.Dead) return;

        Vector2 randomDir = Random.insideUnitCircle.normalized;

        float randomDist = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector2 playerPos = MainSystem.Battle.PlayerHero.transform.position;
        Vector2 spawnPos = playerPos + (Vector2)(randomDir * randomDist);

        GameObject enemyObj = Pop(EPoolType.Enemy, spawnPos);

        if (enemyObj != null)
        {
            Enemy newEnemy = enemyObj.GetComponent<Enemy>();

            var value = EnemyList[Random.Range(0, EnemyList.Count)];

            newEnemy.Setup(value);

            // 배틀 매니저에 등록 (카운팅용)
            MainSystem.Battle.RegisterEnemy(newEnemy);
        }
    }

    
}
