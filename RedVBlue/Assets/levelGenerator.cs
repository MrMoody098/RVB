using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour
{
    public List<GameObject> prefabs;
    //spawn distance of next from previouse prefab y position
    public float spawnY = 50f;
    //spawning interval speed
    public float spawnInterval = 1f;
    //how long the prefab lives before prefab is destroyed
    public float prefabLifetime = 10f;

    private GameObject lastPrefab;

    void Start()
    {
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        while (true)
        {
            Vector3 position = lastPrefab ? lastPrefab.transform.position + Vector3.up * spawnY : Vector3.zero;
            int prefabIndex = Random.Range(0, prefabs.Count);
            lastPrefab = Instantiate(prefabs[prefabIndex], position, Quaternion.identity);
            StartCoroutine(DestroyPrefab(lastPrefab, prefabLifetime));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator DestroyPrefab(GameObject prefab, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(prefab);
    }
}