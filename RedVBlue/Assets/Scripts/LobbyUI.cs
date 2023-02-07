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
    public List<Button> buttons = new List<Button>(); int index = -1; float floatingIndex = 0;
    private void Awake()
    { goBack();}

    private void Update()
    { TraverseActivePanelElements(); }

    //<summary>Allows the user to go through all the buttons/inputs using controller or keyboard</summary>
    public void TraverseActivePanelElements()
    {
        floatingIndex -=
            Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) ?
            (Input.GetAxis("Vertical") * Time.deltaTime * 5) : (Input.GetAxis("Mouse ScrollWheel") * 1000 * Time.deltaTime);
        floatingIndex += Convert.ToInt32(Gamepad.current.dpad.down.isPressed)*Time.deltaTime*10;
        floatingIndex -= Convert.ToInt32(Gamepad.current.dpad.up.isPressed)*Time.deltaTime*10;


        floatingIndex = Mathf.Clamp(floatingIndex, 0, (buttons.Count - 1) + .4f);
        print(floatingIndex);
        if ((int)floatingIndex != index) //on floating index change
        { index = (int)floatingIndex; buttons[index].Select(); }

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
