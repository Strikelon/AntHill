using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] ResourceSpawner _resourceSpawner;
    [SerializeField] float _secondsBetweenResourcesHandled;
    [SerializeField] List<Worker> _workers = new List<Worker>();

    private Queue<Resource> _detectedResources = new Queue<Resource>();
    private int _resourceCount = 0;

    public void CollectResource(Resource resource)
    {
        resource.gameObject.SetActive(false);
        _resourceCount++;
    }

    private void OnEnable()
    {
        _resourceSpawner.ResourceSpawned += OnResourceSpawned;
    }

    private void OnDisable()
    {
        _resourceSpawner.ResourceSpawned -= OnResourceSpawned;
    }

    private void Start()
    {
        InitializeWorkers();
        StartCoroutine(HandleDetectedResources());
    }

    private void InitializeWorkers()
    {
        foreach (var worker in _workers)
        {
            worker.Initialize(this);
        }
    }

    private void OnResourceSpawned(Resource resource)
    {
        _detectedResources.Enqueue(resource);
    }

    private IEnumerator HandleDetectedResources()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(_secondsBetweenResourcesHandled);

            if (_detectedResources.Count != 0)
            {
                Worker freeWorker = _workers.FirstOrDefault(p => p.IsFree() == true);

                if (freeWorker != null)
                {
                    var target = _detectedResources.Dequeue();
                    freeWorker.TakeResource(target);
                }
            }
        }
    }
}
