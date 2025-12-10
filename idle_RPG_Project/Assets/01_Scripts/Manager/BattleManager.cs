using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class BattleManager : BaseManager
{
    [Title("Main Character")]
    [ShowInInspector] public Player Player { get; private set; }

    [ShowInInspector, ReadOnly]
    private List<Enemy> _enemies = new();
    [ShowInInspector]
    public int EnemyCount => _enemies.Count;

    protected override async UniTask OnInitialize()
    {
        
        await UniTask.Delay(500);
    }
    private void Awake()
    {
        // 씬에서 Player 객체 찾기
        Player = FindFirstObjectByType<Player>();
        if (Player == null)
        {
            Debug.LogError("BattleManager: Player object not found in the scene!");
        }

    }

    private void Update()
    {
        if (!IsInitialized) return;
        if (Player == null) return;

        float dt = Time.deltaTime;

        // 1. 플레이어 행동
        Player.Tick(dt);

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
        foreach (var enemy in _enemies)
        {
            if (enemy.State == EntityState.Dead) continue;
            float distSqr = (enemy.transform.position - position).sqrMagnitude;
            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = enemy;
            }
        }
        return nearest;
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
            Debug.Log("Monster Defeated!");
        }
        else if (character is Player)
        {
            // 플레이어 사망 처리 로직
            Debug.Log("Player Defeated! Game Over.");
        }
    }


}
