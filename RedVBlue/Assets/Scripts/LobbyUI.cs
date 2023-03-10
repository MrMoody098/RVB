using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    public string username = "player";
    public GameObject openScreen, hostPanel, joinPanel, back;
    public List<Button> buttons = new List<Button>(); 
    int index = 1; float floatingIndex = 1;
    
    public GameObject currentSelection;
    
    public TextMeshProUGUI createInput;
    public TextMeshProUGUI joinInput;

    public Button joinBtn;

    public string map = null;

    public LobbyRoom selectedRoom;

    private void Start() 
    {
        if (!PhotonNetwork.IsConnected) { SceneManager.LoadScene("loading"); }
        goBack(); 
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void CreateRoom()
    { PhotonNetwork.CreateRoom(createInput.GetParsedText()); }
    public void JoinRoom()
    { selectedRoom.Join(); }

    //runs after create room or join room is called
    public override void OnJoinedRoom()
    { PhotonNetwork.LoadLevel(map); }

    private void LateUpdate()
    { TraverseActivePanelElements(); 
        currentSelection = EventSystem.current.currentSelectedGameObject; }
    public void TraverseActivePanelElements()
    {
        //all inputs are zero by default until pressed
        floatingIndex -= Input.GetAxis("Vertical") * Time.deltaTime * 20;
        floatingIndex -= Input.GetAxis("Mouse ScrollWheel") * 1500 * Time.deltaTime;
        try
        {floatingIndex += Convert.ToInt32(Gamepad.current.dpad.down.isPressed) * Time.deltaTime * 10;
            floatingIndex -= Convert.ToInt32(Gamepad.current.dpad.up.isPressed) * Time.deltaTime * 10;
            floatingIndex += Convert.ToInt32(Gamepad.current.dpad.right.isPressed) * Time.deltaTime * 10;
                floatingIndex -= Convert.ToInt32(Gamepad.current.dpad.left.isPressed) * Time.deltaTime * 10;}
        catch { }//no gamepadConnected

        floatingIndex = Input.GetAxis("Horizontal") > 0 ? 0 : floatingIndex;

        //stay in bounds -- when converting to int it will round down always
        floatingIndex = Mathf.Clamp(floatingIndex, 0, (buttons.Count - 1) + .4f);
        
        //on floating index change
        if ((int)floatingIndex != index) 
        {index = (int)floatingIndex; 
            buttons[index].Select();}

        if (Input.GetButtonDown("Cancel")) { goBack(); }
 
    }
    public void exit(){Application.Quit();}
    public void host()
    {
        openScreen.SetActive(false);
        hostPanel.SetActive(true);
        back.SetActive(true);

        buttons.Clear();
        buttons.Add(back.GetComponent<Button>());
        foreach (Button b in hostPanel.GetComponentsInChildren<Button>())
        { buttons.Add(b); }
    }
    public void join()
    {
        if (!PhotonNetwork.InLobby)
        { PhotonNetwork.JoinLobby(); }
        openScreen.SetActive(false);
        back.SetActive(true);
        joinPanel.SetActive(true);

        buttons.Clear();
        buttons.Add(back.GetComponent<Button>());
        foreach (Button b in joinPanel.GetComponentsInChildren<Button>())
        { buttons.Add(b);}

        if (!PhotonNetwork.InLobby)
        {
            //don't worry about the error. the client is connected to the lobby type default
            PhotonNetwork.JoinLobby();
        }
    }
    public void goBack()
    {
        //removes duplicates
        if (joinPanel.activeSelf)
        {  FindObjectOfType<loadRoomList>().Clear(); }
      

        if (PhotonNetwork.InLobby)
        {PhotonNetwork.LeaveLobby();}

        back.SetActive(false);
        openScreen.SetActive(true);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);

        buttons.Clear();
        foreach (Button b in openScreen.GetComponentsInChildren<Button>())
        { buttons.Add(b); }
    }
}
