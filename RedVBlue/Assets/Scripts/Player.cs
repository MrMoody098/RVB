using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public string username;
    public GameObject uiPrefab;
    
    [HideInInspector]
    public Settings ui;

    public float points = 0;
    public float health = 3;
    public float maxHealth = 3;

    [HideInInspector]
    public bool alive = true;
    [HideInInspector]
    public PhotonView view;
    [HideInInspector]
    public Camera camera;
    [HideInInspector]
    public GrapplingGun grapplingGun;
    [HideInInspector]
    public RoomPlayer lobbyPlayer;
    public int ACNUM;
    [HideInInspector]
    public PlayerMovement movement;
    void Awake() //runs after lobby player is created
    {
        camera = GetComponentInChildren<Camera>();
        health = maxHealth;
        movement = GetComponent<PlayerMovement>();
        view = GetComponent<PhotonView>();
       
        if (view != null)
        {   
            ACNUM = view.OwnerActorNr;
            print("player view actor number set to " + ACNUM);
            if (view.IsMine) 
            {
                
                Settings.ClientView = view;
                InitializePlayerUI();
            }
            else { camera.targetDisplay = 2; }
            grapplingGun = camera.GetComponentInChildren<GrapplingGun>();
            grapplingGun.GetComponent<firing>().player = this;
        }
    }
    public void TransmitAndDisplayUserName()
    {
        if (view.IsMine) //set my view data on eveyone elses version of me
        { view.RPC("SetNickname", RpcTarget.All, FindObjectOfType<userData>().Username); }
        
        //set text of this object in the list entery
        lobbyPlayer.name.SetText(lobbyPlayer.info.NickName);
        view.Owner.NickName = lobbyPlayer.name.text;
        gameObject.name = lobbyPlayer.name.text; ;
    }
    [PunRPC] 
    void SetNickname(string name) { lobbyPlayer.info.NickName = name; }
    public void AddPoints(int amount)
    { points += amount; }
    public void UpHealth(float amount) 
    { health += amount;
        print(health);}
    public void DownHealth(float amount, firing shooter)
    {
        if (health < 1) 
        { alive = false; 
          Dead(); shooter.player.AddPoints(1); }
        else { health -= amount; }
    }
    public void DownHealth(float amount)
    {
        if (health < 1) 
        { alive = false; Dead();} 
        else { health -= amount; }
    }
    public void Dead() 
    { 
        print(gameObject.name + " is dead");
        //points--;
        lobbyPlayer.score.SetText(points+"");
        transform.position = GameObject.Find("Spawn").transform.position;
        health = maxHealth;
        if (!view.IsMine){return;}
        ui.deathScreen.SetActive(true);
        ui.deathScreenAnimation.Play();
    }

    void InitializePlayerUI()
    {
        try
        {
            ui = Instantiate(uiPrefab).GetComponentInChildren<Settings>();
          
            firing f = GetComponentInChildren<firing>();
            f.player = this;
            f.hitMarker = ui.display.hitmarker;
            f.hitMarker.SetActive(false);
        }
        catch { Debug.LogWarning("Non player characters dismissing UI initialization");}
    }
        
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //other
    {
        if (stream.IsWriting)
        {stream.SendNext(health);}
        if (stream.IsReading)
        {health = (float)stream.ReceiveNext(); }

        try{ print("player" + ACNUM + "{ health: " + health + ", points: " + points);
                lobbyPlayer.score.SetText(health + ""); }
        catch{ Debug.LogWarning("waiting for player data to link before pulling stats"); }

    }
}
