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
   // public static int randomIndex;
    public static int currentPlayerActorNumber;
    private void Start()
    { 
        foreach (KeyValuePair<int, Photon.Realtime.Player> p in PhotonNetwork.CurrentRoom.Players )
        { IEnumerator c = AddPlayerListItem(p.Value, 2); StartCoroutine(c); }
        print("finding existing players");
        spawnPoint = GameObject.Find("Spawn").transform.position;
    }
    
    //delay event for two seconds to wait for player view instanciation
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) 
    {IEnumerator c =  AddPlayerListItem(newPlayer, 3); StartCoroutine(c); }
    private IEnumerator AddPlayerListItem(Photon.Realtime.Player player, float delay)
    {
        yield return new WaitForSeconds(delay);
        print("adding lobby player");
        RoomPlayer roomPlayer = Instantiate(roomPlayerPrefab, contentArea).GetComponent<RoomPlayer>();
        roomPlayer.SetPlayerInfo(player);
        players.Add(roomPlayer);
        roomPlayer.index = players.Count - 1;
        foreach (Player p in FindObjectsOfType<Player>())
        {
            if(p.ACNUM > 0 && player.ActorNumber == p.ACNUM)
            {roomPlayer.LinkPlayerInformation(p);  }
        }

        if (PhotonNetwork.IsMasterClient)//if im the host
        { SetActiveShooter(new System.Random().Next(2)); }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        SetActiveShooter(0);
    }
    public void DisablePlayerShooting()
    {
        print("removing players ability to shoot");
        players.ForEach(e =>
        { e.player.gun.ableToShoot = false; });
    }
    public void SetActiveShooter(int index) 
    {
        //can change this whatever way but im just making it work for two player atm
        if (players.Count != 2) { DisablePlayerShooting(); }
        else 
        {
            DisablePlayerShooting();
            players[index].player.gun.ableToShoot = true; 
            print("making" + players[index].info.NickName + " able to shoot"); 
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
        SetActiveShooter(0);

    }
}
