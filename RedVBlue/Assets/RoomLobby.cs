using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using System;
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
    public static int shooterIndex;
    private void Start()
    { 
        foreach (KeyValuePair<int, Photon.Realtime.Player> p in PhotonNetwork.CurrentRoom.Players )
        { IEnumerator c = AddPlayerListItem(p.Value, 2); StartCoroutine(c); }
        print("finding existing players");
        spawnPoint = GameObject.Find("Spawn").transform.position;
    }
    //delay event for two seconds to wait for player view instanciation
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) 
    {IEnumerator c =  AddPlayerListItem(newPlayer, 2); StartCoroutine(c); }
    private IEnumerator AddPlayerListItem(Photon.Realtime.Player player, float delay)
    {
        yield return new WaitForSeconds(delay);
        print("adding lobby player");
        RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, contentArea).GetComponent<RoomPlayer>();
        roomPlayer.SetPlayerInfo(player);
        players.Add(roomPlayer);
        foreach (Player p in FindObjectsOfType<Player>())
        {
            if(p.ACNUM > 0 && player.ActorNumber == p.ACNUM)
            {roomPlayer.LinkPlayerInformation(p);  }
        }

        if (PhotonNetwork.IsMasterClient)//if im the host
        { CalibrateTwoPlayerShooting(); }
        

    }
    private void CalibrateTwoPlayerShooting() 
    {
        //can change this whatever way but im just making it work for two player atm
        if (players.Count != 2)
        {
            print("removing players ability to shoot");
            players.ForEach(e =>
                { e.player.grapplingGun.GetComponent<firing>().ableToShoot = false; });
        }
        else 
        {
            shooterIndex = new System.Random().Next(2);
            players[shooterIndex].player.grapplingGun.GetComponent<firing>().ableToShoot = true; 
            print("making player able to shoot"); 
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
        CalibrateTwoPlayerShooting();

    }
}
