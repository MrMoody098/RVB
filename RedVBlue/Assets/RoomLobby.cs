using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomLobby : MonoBehaviourPunCallbacks
{
    public List<RoomPlayer> players = new List<RoomPlayer>();
    public static GameObject lastSpawnedPlayer;
    public GameObject roomPlayerPrefab;
    public GameObject playerPrefab;
   
    public Transform contentArea;
    public Vector3 spawnPoint;
    private void Awake()
    { 
        foreach (KeyValuePair<int, Photon.Realtime.Player> p in PhotonNetwork.CurrentRoom.Players )
        { AddPlayerListItem(p.Value); }
        
        spawnPoint = GameObject.Find("Spawn").transform.position;
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) { AddPlayerListItem(newPlayer); }
    private void AddPlayerListItem(Photon.Realtime.Player player)
    {
        RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, contentArea).GetComponent<RoomPlayer>(); 
       
        if (player != null)
        {
            roomPlayer.SetPlayerInfo(player);
            roomPlayer.gameObject.name = roomPlayer.roomPlayer.NickName;
            players.Add(roomPlayer);

            foreach (Player p in FindObjectsOfType<Player>())
            { if (p.view.Owner.ActorNumber == player.ActorNumber)
                { roomPlayer.LinkPlayerInformation(p); } }
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = players.FindIndex(x => x.roomPlayer == otherPlayer);
        if (index != 0)
        { 
            Destroy(players[index].gameObject);
            players.RemoveAt(index);
        }
    }
}
