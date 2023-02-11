using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour
{
    public List<GameObject> prefabs;
    public GameObject lavaPrefab;
    public float spawnY = 50f;
    public float spawnInterval = 1f;
    public int initialPrefabCount = 4;

    private GameObject lastPrefab;
    private GameObject lava;

    void Start()
    {
        StartCoroutine(GenerateInitialLevel());
    }

    IEnumerator GenerateInitialLevel()
    {
        for (int i = 0; i < initialPrefabCount; i++)
        {
            Vector3 position = lastPrefab ? lastPrefab.transform.position + Vector3.up * spawnY : Vector3.zero;
            int prefabIndex = Random.Range(0, prefabs.Count);
            lastPrefab = Instantiate(prefabs[prefabIndex], position, Quaternion.identity);
        }

        StartCoroutine(GenerateLevel());
        yield return null;
    }

    IEnumerator GenerateLevel()
    {
        while (true)
        {
            Vector3 position = lastPrefab.transform.position + Vector3.up * spawnY;
            int prefabIndex = Random.Range(0, prefabs.Count);
            lastPrefab = Instantiate(prefabs[prefabIndex], position, Quaternion.identity);
            StartCoroutine(DestroyPrefabWhenReachedByLava(lastPrefab));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator DestroyPrefabWhenReachedByLava(GameObject prefab)
    {
        GameObject previousPrefab = lastPrefab;
        if (!lava)
        {
            Vector3 lavaPosition = prefab.transform.position - Vector3.up * spawnY;
            lava = Instantiate(lavaPrefab, lavaPosition, Quaternion.identity);
        }

        while (lava.transform.position.y < prefab.transform.position.y)
        {
            yield return null;
        }

        if (lava.transform.position.y >= prefab.transform.position.y)
        {
            Destroy(previousPrefab);
        }

        lastPrefab = prefab;
    }

}