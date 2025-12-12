using UnityEngine;
using UnityEngine.Pool;

public class Poolable : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;

    public void SetPool(IObjectPool<GameObject> pool)
    {
        _pool = pool;
    }

    public void Release()
    {
        if (_pool != null) _pool.Release(gameObject);
        else Destroy(gameObject);
    }
}