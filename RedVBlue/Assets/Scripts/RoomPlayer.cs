using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPlayer : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public Player player;
    public Photon.Realtime.Player info;
    public int index;
    public TextMeshProUGUI name, ping, score;

    public void SetPlayerInfo(Photon.Realtime.Player player)
    {info = player;}

    public void LinkPlayerInformation(Player player)
    {
        print("linking lobby player" + player.ACNUM + " with world player" + info.ActorNumber);
        this.player = player;
        this.player.lobbyPlayer = GetComponent<RoomPlayer>(); 
        this.player.TransmitAndDisplayUserName();
    }
    public void UpdatePlayerUIPoints(int points)
    {score.SetText(points+"");}

}
