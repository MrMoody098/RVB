using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject lavaPrefab;
    public float spawnY = 50f;
    public float waitTime = 0.5f;

    private List<GameObject> generatedPrefabs;
   // private GameObject lava;
   // private Vector3 lavaStartPosition;

    void Start()
    {
        generatedPrefabs = new List<GameObject>();
        StartCoroutine(GenerateInitialLevel());
    }

    IEnumerator GenerateInitialLevel()
    {
        Vector3 startPosition = transform.position;
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, prefabs.Length);
            GameObject prefab = Instantiate(prefabs[randomIndex], startPosition, Quaternion.identity);
            generatedPrefabs.Add(prefab);
            startPosition.y += spawnY;
        }

      //  lavaStartPosition = startPosition - Vector3.up * spawnY;
     //   lava = Instantiate(lavaPrefab, lavaStartPosition, Quaternion.identity);

        yield return new WaitForSeconds(waitTime);
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        Vector3 startPosition = transform.position;
        while (true)
        {
            int randomIndex = Random.Range(0, prefabs.Length);
            GameObject prefab = Instantiate(prefabs[randomIndex], startPosition, Quaternion.identity);
            generatedPrefabs.Add(prefab);
            startPosition.y += spawnY;
            yield return new WaitForSeconds(waitTime);
        }
    }

    //IEnumerator DestroyPrefabWhenReachedByLava(GameObject prefab)
    //{
    //    while (lava.transform.position.y < prefab.transform.position.y)
    //    {
    //        yield return null;
    //    }

    //    if (lava.transform.position.y >= prefab.transform.position.y + 50f)
    //    {
    //        int prefabIndex = generatedPrefabs.IndexOf(prefab);
    //        GameObject prefabToDestroy = generatedPrefabs[prefabIndex - 1];
    //        generatedPrefabs.Remove(prefabToDestroy);
    //        Destroy(prefabToDestroy);
    //    }
    //}
}
