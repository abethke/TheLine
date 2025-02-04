using UnityEngine;
using UnityEngine.Pool;

public class WallObjectPool : MonoBehaviour
{
    public void Init(int in_size, float in_wallWidth, float in_wallHeight)
    {
        _wallWidth = in_wallWidth;
        _wallHeight = in_wallHeight;

        _pool?.Clear();
        _pool = new ObjectPool<WallSegment>(CreateWall, OnTakeFromPool, OnReturnToPool, OnDestroyPooledObject, true, in_size);
    }
    void OnDestroy()
    {
        _pool?.Clear();
    }
    public WallSegment Get()
    {
        return _pool.Get();
    }
    public void Release(WallSegment in_wall)
    {
        _pool.Release(in_wall);
    }
    protected WallSegment CreateWall()
    {
        GameObject instance = Instantiate(_wallPrefab, _wallContainer);
        instance.transform.localScale = new Vector3(_wallWidth, _wallHeight, 1);

        WallSegment wall = instance.GetComponent<WallSegment>();
        instance.SetActive(false);
        return wall;
    }
    protected void OnTakeFromPool(WallSegment in_wall)
    {
        in_wall.Reset();
        in_wall.gameObject.SetActive(true);
    }
    protected void OnReturnToPool(WallSegment in_wall)
    {
        in_wall.gameObject.SetActive(false);
    }
    protected void OnDestroyPooledObject(WallSegment in_wall)
    {
        Destroy(in_wall.gameObject);
    }

    protected IObjectPool<WallSegment> _pool;

    protected float _wallWidth;
    protected float _wallHeight;

    [Header("Configuration")]
    [SerializeField]
    protected GameObject _wallPrefab;
    [SerializeField]
    protected Transform _wallContainer;
}
