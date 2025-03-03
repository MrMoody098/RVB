using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
using UnityEngine.iOS;

[System.Serializable]
public class Controls 
{ public GloblalInputs inputs; 
    public bool quickRotate = false; }

[System.Serializable]
public class Video { public int fontSize; public GameObject hitmarker; public int fieldOfView; public bool enableDLSS = false; }
[System.Serializable]
public class Audio {
    [Range(0, 100)]
    public int volume = 50;
}
public class Settings : MonoBehaviour
{
    public List<Transform> options;
    private RoomLobby lobby;
    [Header("Controls")]
    public Controls controls;
    [Header("Display Settings")]
    public Video display;
    [Header("Audio settings")]
    public Audio audio;
    public static GameObject menu;
    public GameObject deathScreen;
    public Animation deathScreenAnimation;
    public static PhotonView ClientView;


    public List<RectTransform> panels;
    public GameObject underliner;
    int currentPanelIndex = 0;

    Button selectedMenu;
    public Button lobbyBtn, videoBtn, audioBtn, controlsBtn;
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

    public void leaveRoom()
    {
        if (!ClientView.IsMine) { return; }
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("loading");
    }

    private void Update()
    {
        currentPanelIndex = Mathf.Clamp(currentPanelIndex, 0, panels.Count-1);
        Vector2 ul = underliner.GetComponent<RectTransform>().anchoredPosition;
        float newX = panels[currentPanelIndex].anchoredPosition.x;
        underliner.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(ul, new Vector2(newX, ul.y), (10 * Time.deltaTime) * MathF.Abs(newX - ul.x));
        panels[currentPanelIndex].GetComponent<Button>().Select();

        if (Input.GetKeyDown(KeyCode.LeftArrow)) { currentPanelIndex--;  }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { currentPanelIndex++;}
    }

    public void OpenLobby() { selectedMenu = lobbyBtn; ChangeSettingsPanel(); }
    public void OpenControls() { selectedMenu = controlsBtn; ChangeSettingsPanel(); }
    public void OpenAudio() { selectedMenu = audioBtn; ChangeSettingsPanel(); }
    public void OpenVideo() { selectedMenu = videoBtn; ChangeSettingsPanel(); }
    public void ChangeSettingsPanel()
    {for(int i = 0; i < panels.Count; i++)
        {if (panels[i].gameObject == selectedMenu.gameObject)
            { currentPanelIndex = i;} }}
}
