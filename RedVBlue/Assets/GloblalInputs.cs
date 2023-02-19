using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GloblalInputs : MonoBehaviour
{
    RoomUI ui;
    private void Awake() {ui = GetComponent<RoomUI>(); }
    // Update is called once per frame
    void Update()
    {
        if (RoomUI.player == null) { return; }
       if(RoomUI.player.IsMine) 
       {
            if(Input.GetButtonDown("Cancel"))
            {
                TogglePauseMenuVisible();
            }

       }
    }

    public static void TogglePauseMenuVisible() 
    {
        RoomUI.player.GetComponent<PlayerMovement>().isAbleToMove 
            = !RoomUI.player.GetComponent<PlayerMovement>().isAbleToMove;
        Cursor.visible = !RoomUI.player.GetComponent<PlayerMovement>().isAbleToMove;
        RoomUI.menu.SetActive(Cursor.visible);
        RoomUI.player.GetComponent<PlayerMovement>().isAbleToMove = !Cursor.visible;
        if (Cursor.visible) { Cursor.lockState = CursorLockMode.None; }
        else { Cursor.lockState = CursorLockMode.Locked; }
    }

}
