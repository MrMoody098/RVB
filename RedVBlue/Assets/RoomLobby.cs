using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
public class RoomLobby : MonoBehaviourPunCallbacks
{
    public List<RoomPlayer> players = new List<RoomPlayer>();
    public static GameObject lastSpawnedPlayer;
    public GameObject roomPlayerPrefab;
   
    public Transform contentArea;
    public Vector3 spawnPoint;
    private void Start()
    { 
        foreach (KeyValuePair<int, Photon.Realtime.Player> p in PhotonNetwork.CurrentRoom.Players )
        { IEnumerator c = AddPlayerListItem(p.Value, 2); StartCoroutine(c); }
        print("finding existing players");
        spawnPoint = GameObject.Find("Spawn").transform.position;
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) 
    { IEnumerator c =  AddPlayerListItem(newPlayer, 2); StartCoroutine(c); }
    private IEnumerator AddPlayerListItem(Photon.Realtime.Player player, float delay)
    {
        yield return new WaitForSeconds(delay);
        print("adding lobby player");
        RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, contentArea).GetComponent<RoomPlayer>();
        roomPlayer.SetPlayerInfo(player);
        players.Add(roomPlayer);
        print(player.ActorNumber);
        foreach (Player p in FindObjectsOfType<Player>())
        {
            print("ACNUM"+p.ACNUM);
            if(p.ACNUM > 0 && player.ActorNumber == p.ACNUM)
            {roomPlayer.LinkPlayerInformation(p);  }
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = players.FindIndex(i => i.info == otherPlayer);
        if (index != 0)
        { 
            Destroy(players[index].gameObject);
            players.RemoveAt(index);
        }
    }
}
