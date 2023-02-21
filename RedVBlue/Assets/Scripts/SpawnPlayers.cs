using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public Vector3 max,min;

    //public float minX;
    //public float maxX;
    //public float minY;
    //public float maxY;
    //public float minZ;
    //public float maxZ;
    private void Start()
    {
        try
        {
            Vector3 randomPosition = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            Lobby.lastSpawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        }
        catch { print("GOING OFFLINE"); }
    }
}
