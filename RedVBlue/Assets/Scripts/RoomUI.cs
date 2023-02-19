using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
[System.Serializable]
public class Lobby
{
   // public List<RoomPlayer> players = new List<RoomPlayer>();
}
[System.Serializable]
public class Controls 
{ public GloblalInputs inputs; 
    public bool quickRotate = false; }

[System.Serializable]
public class Video { public int fontSize; public GameObject hitmarker; }
[System.Serializable]
public class Audio {
    [Range(0, 100)]
    public int volume = 50;
}
public class RoomUI : MonoBehaviour
{
    [Header ("Room Lobby")]
    
    public Lobby lobby;
    [Header("Controls")]
    public Controls controls;
    [Header("Display Settings")]
    public Video display;
    [Header("Audio settings")]
    public Audio audio;

    public static GameObject menu;

    public GameObject deathScreen;
    public Animation deathScreenAnimation;
    private void Awake()
    {
        menu = transform.Find("PauseMenu").gameObject;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu.SetActive(false);
        //deathScreenAnimation = GetComponentInChildren<Animation>();
        //deathScreen = deathScreenAnimation.transform.parent.gameObject;
    }
    // Start is called before the first frame update
    public static PhotonView player;
    public void leaveRoom()
    {
        if (!player.IsMine) { return; }
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("loading");
    }
    
}
