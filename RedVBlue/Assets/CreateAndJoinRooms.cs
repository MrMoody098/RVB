using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    public TMPro.TMP_InputField createInput;
    public TMPro.TMP_InputField joinInput;


    public void CreateRoom() 
    { PhotonNetwork.CreateRoom(createInput.text); }
    public void JoinRoom()
    { PhotonNetwork.JoinRoom(joinInput.text); }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Map1");
    }
}
