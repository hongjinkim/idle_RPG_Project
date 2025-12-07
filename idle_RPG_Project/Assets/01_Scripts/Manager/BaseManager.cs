using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public abstract class BaseManager : MonoBehaviour
{
    protected MainSystem Main => MainSystem.Instance;

    [ShowInInspector, ReadOnly]
    public bool IsInitialized { get; private set; } = false;

    public async UniTask InitializeAsync()
    {
        if (IsInitialized) return;

        Debug.Log($"🔄 Initializing {GetType().Name}...");

        await OnInitialize(); // 실질적인 초기화 로직 실행

        IsInitialized = true;
        Debug.Log($"✅ {GetType().Name} Ready.");
    }

    // 자식 클래스에서 구체적인 내용을 작성하는 부분
    protected abstract UniTask OnInitialize();
}