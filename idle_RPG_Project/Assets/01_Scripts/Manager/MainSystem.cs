using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector; // 리스트 조작용

public class MainSystem : MonoBehaviour
{
    public static MainSystem Instance { get; private set; }

    // 구체적인 접근이 필요할 땐 프로퍼티 유지
    public BattleManager Battle { get; private set; }
    public UIManager UI { get; private set; }

    // 모든 매니저를 담아둘 리스트 (일괄 관리용)
    [ShowInInspector, ReadOnly]
    private List<BaseManager> _allManagers = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame().Forget();
    }

    private async UniTaskVoid InitializeGame()
    {
        // 1. 내 자식으로 붙어있는 모든 BaseManager 상속받은 놈들을 다 찾음
        _allManagers = GetComponentsInChildren<BaseManager>().ToList();

        // 2. 자주 쓰는 매니저는 프로퍼티에 캐싱 (편의성)
        // (GetComponent는 무거우니 처음에 한 번만)
        Battle = GetManager<BattleManager>();
        UI = GetManager<UIManager>();

        // 3. 찾은 모든 매니저 일괄 초기화 (순서는 Unity Inspector 순서 따름)
        foreach (var manager in _allManagers)
        {
            await manager.InitializeAsync();
        }

        Debug.Log("🚀 Game Start!");
    }

    // 유틸리티: 특정 매니저 가져오기
    public T GetManager<T>() where T : BaseManager
    {
        // 이미 리스트에 있으면 반환, 없으면 찾기
        var mgr = _allManagers.FirstOrDefault(m => m is T);
        return mgr as T;
    }
}