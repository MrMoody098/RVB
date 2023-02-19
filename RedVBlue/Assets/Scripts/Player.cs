using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject uiPrefab; //load from resources in future
    
    [HideInInspector]
    public RoomUI ui;

    public int points = 0;
    public float health = 3;
    public float maxHealth = 3;

    [HideInInspector]
    public bool alive = true;

    [HideInInspector]
    public PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
       
        health = maxHealth;
        view = GetComponent<PhotonView>(); 
        if (view.IsMine) { RoomUI.player = view;  initPlayerUI(); }
      

        
    }

    public void UpHealth(float amount) {
        health += amount;
        print(health);
    }
    public void DownHealth(float amount, firing shooter)
    {
        if (health < 1) { alive = false; Dead(); shooter.player.points++; }
        else { health -= amount; }
    }
    public void DownHealth(float amount)
    {
        if (health < 1) { alive = false; Dead();} 
        else { health -= amount; }}
    public void Dead() 
    {

        print(gameObject.name + "is dead");
       // if(!view.IsMine)
        {
            points--;
            transform.position = GameObject.Find("Spawn").transform.position;
            ui.deathScreen.SetActive(true);
            ui.deathScreenAnimation.Play();
            health = maxHealth;
        }
    }

    void initPlayerUI()
    {

        try
        {
            ui = Instantiate(uiPrefab).GetComponentInChildren<RoomUI>();
            firing f = GetComponentInChildren<firing>();
            f.player = this;
            f.hitMarker = ui.display.hitmarker.gameObject;
            f.hitMarker.gameObject.SetActive(false);
            //ui.GetComponent<Canvas>().worldCamera = GetComponentInChildren<Camera>();
        }
        catch { Debug.LogWarning("Non player characters dismissing UI initialization"); /*enemy doesnt need ui*/}
    }
        
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //other
    {
        if (stream.IsWriting)
        {stream.SendNext(health);}
        if (stream.IsReading)
        {health = (float)stream.ReceiveNext(); }
        print(health);
    }
}
