using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] private int _initialCapacity;
    [SerializeField] private T _prefab;
    [SerializeField] private float _lowerBoundX;
    [SerializeField] private float _upperBoundX;
    [SerializeField] private float _lowerBoundY;
    [SerializeField] private float _upperBoundY;
    [SerializeField] private float _lowerBoundZ;
    [SerializeField] private float _upperBoundZ;

    private Transform _container;

    protected List<T> Pool = new List<T>();

    private void Awake()
    {
        _container = transform;
    }

    private T CreateObject(Transform transform)
    {
        GameObject spawned = Instantiate(_prefab.gameObject, _container.position, Quaternion.identity, _container);
        spawned.transform.localScale = _prefab.transform.localScale;
        spawned.SetActive(false);
        T component = spawned.GetComponent<T>();
        Pool.Add(component);
        return component;
    }

    protected void Initialize()
    {
        for (int i = 0; i < _initialCapacity; i++)
        {
            CreateObject(_container);
        }
    }

    protected T GetObject()
    {
        var result = Pool.FirstOrDefault(p => p.gameObject.activeSelf == false);

        if (result == null)
        {
            result = CreateObject(_container);
        }

        Vector3 randomPosition = new Vector3(
                Random.Range(_lowerBoundX, _upperBoundX),
                Random.Range(_lowerBoundY, _upperBoundY),
                Random.Range(_lowerBoundZ, _upperBoundZ)
            );
        result.gameObject.transform.position = randomPosition;
        return result;
    }

    public void RestPool()
    {
        foreach (var objectGame in Pool)
        {
            objectGame.gameObject.SetActive(false);
        }
    }

}