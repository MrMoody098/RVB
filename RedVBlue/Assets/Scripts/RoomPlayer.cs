using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPlayer : MonoBehaviour
{
    PhotonView player;
    public Photon.Realtime.Player roomPlayer;
    TextMeshProUGUI name, ping, score;
    private void Awake()
    {
        name = transform.Find("playerTxt").GetComponent<TextMeshProUGUI>();
        ping = transform.Find("pingTxt").GetComponent<TextMeshProUGUI>();
        score = transform.Find("pointsTxt").GetComponent<TextMeshProUGUI>();
    }

    public void SetPlayerInfo(Photon.Realtime.Player player)
    {
        roomPlayer = player;
        
    }

    public void LinkPlayerInformation(PhotonView player)
    {
        this.player = player;
        player.gameObject.name = RoomUI.userName;
        roomPlayer.NickName = player.gameObject.name;
        name.SetText(roomPlayer.NickName);
    }

}
