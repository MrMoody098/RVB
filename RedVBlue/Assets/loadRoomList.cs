using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class loadRoomList : MonoBehaviourPunCallbacks
{
   
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomsContainer;
    List<LobbyRoom> rooms = new List<LobbyRoom>();

    private void Update()
    {
    }
    private void Awake()
    { PhotonNetwork.AutomaticallySyncScene = true; }

    //event fires when rooms have updated
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //full list on awake, one element on runtime.
    //really dumb but they making this tight nit for security reasons.
    {
        roomList.ForEach(r => 
        {
            try {
                rooms.ForEach(lr =>
                {
                    if (lr.info.Name == r.Name)
                    {
                        if (r.RemovedFromList || lr.roomId != r.masterClientId) 
                        { Destroy(lr.gameObject); rooms.Remove(lr); }
                        else { lr.SetRoomInfo(r); }
                        return;
                    }
                });
            }
            catch { print("room removed from under our feet"); return; }

            print("roomInstanceFound");
            
            
            GameObject room = Instantiate(roomPrefab, roomsContainer);
            room.GetComponent<LobbyRoom>().SetRoomInfo(r);
            rooms.Add(room.GetComponent<LobbyRoom>());

        });
    }

    public void Clear()
    {
        rooms.ForEach(r => { Destroy(r.gameObject);});
        rooms.Clear();
    }
}
