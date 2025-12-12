using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Pool;

public class PoolBase<T> : MonoBehaviour where T : Enum
{
    [SerializeField] private GameObject _gameObj;
    [SerializeField] public T PoolType;
    [SerializeField] private IObjectPool<GameObject> _pool;

    public void Initialize()
    {
        if (_pool != null) return;

        _pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet : OnGetItem,
            actionOnRelease : OnReleaseItem,
            actionOnDestroy : OnDestroyItem,
            collectionCheck: true,
            maxSize: 1000 // 필요 시 조정
        );
    }

    private GameObject CreateItem()
    {
        GameObject obj = Instantiate(_gameObj, transform);
        obj.name = _gameObj.name; // 이름 깔끔하게

        // Poolable 컴포넌트 부착 및 설정
        var poolable = obj.GetComponent<Poolable>();
        if (poolable == null) poolable = obj.AddComponent<Poolable>();
        poolable.SetPool(_pool);

        return obj;
    }

    public GameObject Pop()
    {
        return _pool.Get();
    }
    public void Push(GameObject obj)
    {
        _pool.Release(obj);
    }

    private void OnGetItem(GameObject obj) => obj.SetActive(true);
    private void OnReleaseItem(GameObject obj) => obj.SetActive(false);
    private void OnDestroyItem(GameObject obj) => Destroy(obj);
}

