using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GloblalInputs : MonoBehaviour
{
    Settings ui;
    private void Awake() {ui = GetComponent<Settings>(); }
    // Update is called once per frame
    void Update()
    {
        if (Settings.ClientView == null) { return; }
       if(Settings.ClientView.IsMine) 
       {
            if(Input.GetButtonDown("Cancel"))
            {
                TogglePauseMenuVisible();
            }

       }
    }

    public static void TogglePauseMenuVisible() 
    {
       Settings.ClientView.GetComponent<PlayerMovement>().isAbleToMove 
            = !Settings.ClientView.GetComponent<PlayerMovement>().isAbleToMove;
        Cursor.visible = !Settings.ClientView.GetComponent<PlayerMovement>().isAbleToMove;
        Settings.menu.SetActive(Cursor.visible);
        Settings.ClientView.GetComponent<PlayerMovement>().isAbleToMove = !Cursor.visible;
        if (Cursor.visible) { Cursor.lockState = CursorLockMode.None; }
        else { Cursor.lockState = CursorLockMode.Locked; }
    }

}
