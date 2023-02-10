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
    
    private void Awake()
    {
        
    }
    public void CreateRoom()
    { PhotonNetwork.CreateRoom(createInput.text); joinInput.GetParsedText(); }
    public void JoinRoom()
    {print(joinInput.GetParsedText()); PhotonNetwork.JoinRoom(joinInput.GetParsedText());  }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lava Tower");
    }
}
