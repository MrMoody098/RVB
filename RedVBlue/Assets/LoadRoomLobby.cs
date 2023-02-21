using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoadRoomLobby : MonoBehaviourPunCallbacks
{
    Lobby lobby;
    public GameObject roomPlayerPrefab;
    public GameObject playerPrefab;
    GameObject spawnedPlayer;
    public Transform contentArea;
    public Vector3 spawnPoint;
    private void Awake()
    { 
        lobby = FindObjectOfType<RoomUI>().lobby; 
        foreach (KeyValuePair<int, Photon.Realtime.Player> p in PhotonNetwork.CurrentRoom.Players )
        {AddPlayerListItem(p.Value); }

        spawnPoint = GameObject.Find("Spawn").transform.position;
        try
        {
           // spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
        }
        catch { print("GOING OFFLINE"); }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) { AddPlayerListItem(newPlayer); }
    private void AddPlayerListItem(Photon.Realtime.Player player)
    {
        RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, contentArea).GetComponent<RoomPlayer>();

        if (player != null)
        {
            roomPlayer.SetPlayerInfo(player);
            roomPlayer.gameObject.name = roomPlayer.roomPlayer.NickName;
            lobby.players.Add(roomPlayer);
            if (Lobby.lastSpawnedPlayer != null)
            {
                roomPlayer.LinkPlayerInformation(Lobby.lastSpawnedPlayer.GetComponent<PhotonView>());
            }
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = lobby.players.FindIndex(x => x.roomPlayer == otherPlayer);
        if (index != 0)
        { 
            Destroy(lobby.players[index].gameObject);
            lobby.players.RemoveAt(index);
        }
    }
}
