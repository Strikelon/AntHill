using System;
using System.Collections;
using UnityEngine;

public class ResourceSpawner : ObjectPool<Resource>
{
    [SerializeField] float _secondsBetweenSpawn;

    public event Action<Resource> ResourceSpawned;

    private void Start()
    {
        Initialize();
        StartCoroutine(SpawnResources());
    }

    private IEnumerator SpawnResources()
    {
        while (gameObject.activeSelf)
        {
            Resource resource = GetObject();
            resource.gameObject.SetActive(true);
            ResourceSpawned.Invoke(resource);

            yield return new WaitForSeconds(_secondsBetweenSpawn);
        }
    }
}
