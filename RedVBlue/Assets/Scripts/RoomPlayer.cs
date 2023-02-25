using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPlayer : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    public Photon.Realtime.Player info;

    public TextMeshProUGUI name, ping, score;
    private void Awake()
    {
        //name = transform.Find("playerTxt").GetComponent<TextMeshProUGUI>();
        //ping = transform.Find("pingTxt").GetComponent<TextMeshProUGUI>();
        //score = transform.Find("pointsTxt").GetComponent<TextMeshProUGUI>();
        
    }

    public void SetPlayerInfo(Photon.Realtime.Player player)
    {info = player;}

    public void LinkPlayerInformation(Player player)
    {
        print("linking lobby player" + player.ACNUM + " with world player" + info.ActorNumber);
        this.player = player;
        this.player.lobbyPlayer = this;
        player.gameObject.name = RoomUI.userName;
        info.NickName = player.gameObject.name;
        name.SetText(info.NickName);
    }

    public void UpdatePlayerUIPoints(int points)
    {score.SetText(points+"");}

}
