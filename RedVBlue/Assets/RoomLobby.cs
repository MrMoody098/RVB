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
    //fires before player instantiation
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) 
    { IEnumerator c =  AddPlayerListItem(newPlayer, 2); StartCoroutine(c); }
    private IEnumerator AddPlayerListItem(Photon.Realtime.Player player, float delay)
    {
        yield return new WaitForSeconds(delay);
        print("adding lobby player");
        RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, contentArea).GetComponent<RoomPlayer>();
        roomPlayer.SetPlayerInfo(player);
        players.Add(roomPlayer);
        foreach (Player p in FindObjectsOfType<Player>())
        {
            print("ACNUM"+p.ACNUM);
            if(p.ACNUM > 0 && player.ActorNumber == p.ACNUM)
            {roomPlayer.LinkPlayerInformation(p);}
        }
        //print("adding player item to list");
        //if (player != null)
        //{
        //    roomPlayer.SetPlayerInfo(player);
        //    players.Add(roomPlayer);
        //    foreach (Player p in FindObjectsOfType<Player>())
        //    {
        //       if (p.view == null) { continue; }
        //        print(p.gameObject.name);
        //        if (p.view.Owner.ActorNumber == player.ActorNumber)
        //        {
        //            print("linking player AN" + p.view.Owner.ActorNumber + " with photon lobby player AN" + player.ActorNumber);
        //            roomPlayer.LinkPlayerInformation(p.GetComponent<Player>());
        //        }
        //        else { print("player AN" + p.view.Owner.ActorNumber + ": lobbyPlayer AN" + player.ActorNumber + " no match"); }
        //    }
        //}
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
