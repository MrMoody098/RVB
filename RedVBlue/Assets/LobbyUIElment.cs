using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyUIElment : MonoBehaviour, ISelectHandler
{
    LobbyUI lobbyUI;
    private void Awake()
    {lobbyUI = FindObjectOfType<LobbyUI>();}
    public void OnSelect(BaseEventData eventData)
    {
        lobbyUI.joinBtn.interactable = false;
       
    }

}
