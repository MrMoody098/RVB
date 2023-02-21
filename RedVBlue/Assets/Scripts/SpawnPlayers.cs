using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;
    private void Start()
    {
        try
        {
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            Lobby.lastSpawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        }
        catch { print("GOING OFFLINE"); }
    }
}
