using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour, ISelectHandler
{
    public Text RoomNameText;
    public Text RoomPlayersText;

    public LobbyUI lobbyUI;

    [HideInInspector]
    public RoomInfo info;

    int playerCount = 0;

    [HideInInspector]
    public int roomId;

    private void Awake()
    {
        lobbyUI = GameObject.FindObjectOfType<LobbyUI>();
    }
    public void SetRoomInfo(RoomInfo info)
    {
        this.info = info;
        roomId = info.masterClientId;
        playerCount = info.PlayerCount;
        RoomNameText.text = info.Name;
        RoomPlayersText.text = playerCount + " / " + 128;//info.MaxPlayers;
    }
    public void Join()
    {
        print("joining " + info.Name);
        PhotonNetwork.JoinRoom(info.Name);
    }
    public void OnSelect(BaseEventData eventData)
    {
        lobbyUI.selectedRoom = this;
        lobbyUI.joinBtn.interactable = true;
        print("room selected");
    }
}