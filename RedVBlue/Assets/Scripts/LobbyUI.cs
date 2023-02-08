using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class LobbyUI : MonoBehaviour
{
    public GameObject openScreen, hostPanel, joinPanel, back;
    public List<Button> buttons = new List<Button>(); 
    int index = 1; float floatingIndex = 1;
    private void Start() 
    { goBack(); EventSystem.current.SetSelectedGameObject(null); }

    private void Update()
    { TraverseActivePanelElements(); }

    public void TraverseActivePanelElements()
    {
        //all inputs are zero by default until pressed
        floatingIndex -= Input.GetAxis("Vertical") * Time.deltaTime * 20;
        floatingIndex -= Input.GetAxis("Mouse ScrollWheel") * 1500 * Time.deltaTime;
        floatingIndex += Convert.ToInt32(Gamepad.current.dpad.down.isPressed) * Time.deltaTime * 10; 
        floatingIndex -= Convert.ToInt32(Gamepad.current.dpad.up.isPressed) * Time.deltaTime * 10;
        floatingIndex += Convert.ToInt32(Gamepad.current.dpad.right.isPressed) * Time.deltaTime * 10;
        floatingIndex -= Convert.ToInt32(Gamepad.current.dpad.left.isPressed) * Time.deltaTime * 10;
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
        openScreen.SetActive(false);
        back.SetActive(true);
        joinPanel.SetActive(true);

        buttons.Clear();
        buttons.Add(back.GetComponent<Button>());
        foreach (Button b in joinPanel.GetComponentsInChildren<Button>())
        { buttons.Add(b); print(b.name); }
    }

    public void goBack()
    {
        back.SetActive(false);
        openScreen.SetActive(true);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);

        buttons.Clear();
        foreach (Button b in openScreen.GetComponentsInChildren<Button>())
        { buttons.Add(b); }
    }
}
