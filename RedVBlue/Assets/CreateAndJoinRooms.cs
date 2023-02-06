using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    public TextMeshProUGUI createInput;
    public TextMeshProUGUI joinInput;


    public void CreateRoom()
    { PhotonNetwork.CreateRoom(createInput.GetParsedText()); ; }
    public void JoinRoom()
    { PhotonNetwork.JoinRoom(joinInput.GetParsedText());  }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Map1");
    }
}
