using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;
public class EnemySpawner : BaseManager
{
    [Title("Settings")]
    [SerializeField] private Enemy enemyPrefab; // 프리팹 연결
    [SerializeField] private float spawnInterval = 2.0f; // 젠 시간
    [SerializeField] private float spawnDistanceX = 12.0f; // 플레이어로부터 거리

    [Title("Debug")]
    [ShowInInspector, ReadOnly] private bool _isSpawning = false;

    protected override async UniTask OnInitialize()
    {
        // 몬스터 프리팹 로드 (또는 Addressable)

        StartSpawning();
        await UniTask.CompletedTask;
    }

    public void StartSpawning()
    {
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
            SpawnMonster();
            // UniTask로 딜레이 (밀리초 단위 변환)
            await UniTask.Delay((int)(spawnInterval * 1000));
        }
    }

    private void SpawnMonster()
    {
        if (Main.Battle.Player == null) return;

        // 플레이어 위치 기준 오른쪽 + spawnDistanceX
        Vector3 playerPos = Main.Battle.Player.transform.position;
        Vector3 spawnPos = new Vector3(playerPos.x + spawnDistanceX, playerPos.y, 0);

        // 생성 (나중에는 ObjectPool 써야 함)
        Enemy newMob = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 스탯 설정 (체력 100, 공격력 10) - 나중에 데이터테이블 연동
        newMob.Setup(100, 10);

        // 매니저에 등록
        Main.Battle.RegisterEnemy(newMob);
    }
}
