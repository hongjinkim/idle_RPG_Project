using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class BattleManager : BaseManager
{
    [Title("Main Character")]
    [ShowInInspector] public Hero PlayerHero { get; private set; }
    private Transform _heroTransform => PlayerHero != null ? PlayerHero.transform : null;

    [Title("Radius Settings")]
    [InfoBox("최소 반경은 카메라 화면 대각선 길이보다 커야 화면 밖에서 생성됩니다.")]
    public float minSpawnRadius = 10.0f; // 화면 밖 (최소 거리)
    public float maxSpawnRadius = 14.0f; // 최대 거리

    [ShowInInspector, ReadOnly]
    private List<Enemy> _enemies = new();
    [ShowInInspector]
    public int EnemyCount => _enemies.Count;
    // 리포지셔닝 거리 제한
    [SerializeField] private float repositionDistance = 20.0f;
    private float _repositionDistSqr;

    public int CurrentStage;

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        // 씬에서 Player 객체 찾기
        PlayerHero = FindFirstObjectByType<Hero>();
        if (PlayerHero == null)
        {
            Debug.LogError("BattleManager: Player object not found in the scene!");
        }
        _repositionDistSqr = repositionDistance * repositionDistance;
        TestLoadHero(PlayerHero);
        CurrentStage = MainSystem.Player.playerData.Value.CurrentStage;
    }
    private void TestLoadHero(Hero hero)
    {
        hero.Setup(MainSystem.Data.Hero.GetClone("HERO_001"));
    }

    private void Update()
    {
        if (!IsInitialized) return;
        if (PlayerHero == null) return;

        float dt = Time.deltaTime;

        // 1. 플레이어 행동
        PlayerHero.Tick(dt);

        // 2. 몬스터들 행동 (역순 루프: 도중에 죽어서 리스트에서 빠질 수 있으므로)
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            var mob = _enemies[i];
            if (mob.gameObject.activeSelf)
            {
                mob.Tick(dt);
            }
        }
    }

    public CharacterBase GetNearestEnemy(Vector3 position)
    {
        CharacterBase nearest = null;
        float nearestDistSqr = float.MaxValue;

        var enemyList = _enemies;
        int count = enemyList.Count;

        for (int i = 0; i < count; i++)
        {
            var enemy = enemyList[i];

            // 1. 유효성 검사
            if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.State == ECharacterState.Dead)
                continue;

            // 2. 거리 제곱 계산
            float distSqr = (enemy.transform.position - position).sqrMagnitude;

            // 리포지셔닝 (너무 먼 적 땡겨오기)
            if (distSqr > _repositionDistSqr)
            {
                RepositionEnemy(enemy);
                continue;
            }

            // 가장 가까운 적 찾기
            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }

    public void RepositionEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        // 플레이어가 없거나 죽었으면 생성 중단
        if (PlayerHero == null || PlayerHero.State == ECharacterState.Dead) return;

        Vector2 randomDir = Random.insideUnitCircle.normalized;

        float randomDist = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector2 playerPos = PlayerHero.transform.position;
        Vector2 spawnPos = playerPos + (Vector2)(randomDir * randomDist);

        enemy.transform.position = spawnPos;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
    }

    public void OnCharacterDead(CharacterBase character)
    {
        if (character is Enemy enemy)
        {
            UnregisterEnemy(enemy);
            // 여기서 골드 획득, 점수 증가 로직 호출
            MainSystem.Loot.SpawnLoot(ELootType.Gold,enemy.dropGold, enemy.transform.position, _heroTransform);
        }
        else if (character is Hero)
        {
            // 플레이어 사망 처리 로직
            Debug.Log("Player Defeated! Game Over.");
        }
    }
    

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && PlayerHero != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(PlayerHero.transform.position, minSpawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PlayerHero.transform.position, maxSpawnRadius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(PlayerHero.transform.position, repositionDistance);
        }
    }
}
