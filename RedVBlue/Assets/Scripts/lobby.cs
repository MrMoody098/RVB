using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobby : MonoBehaviour
{
    public GameObject openScreen, hostPanel, joinPanel, back;

    private void Start()
    {
        back.SetActive(false);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
        openScreen.SetActive(true);
    }
    public void exit(){Application.Quit();}

    public void host()
    {
        openScreen.SetActive(false);
        hostPanel.SetActive(true);
        back.SetActive(true);
    }
    public void join()
    {
        openScreen.SetActive(false);
        back.SetActive(true);
        joinPanel.SetActive(true);
    }

    public void goBack()
    {
        back.SetActive(false);
        openScreen.SetActive(true);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
    }
}
