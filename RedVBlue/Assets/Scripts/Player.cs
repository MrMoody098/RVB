using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public AudioSource playerHurt;
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
    public Gun gun;
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
            else { camera.targetDisplay = 2; Destroy(camera.GetComponent<AudioListener>()); }
            gun = camera.GetComponentInChildren<Gun>();
            gun.GetComponent<Gun>().player = this;
        }
    }
    public void TransmitAndDisplayUserName()
    {
        if (view.IsMine) //set my view data on eveyone elses version of me
        { view.RPC("SetNickname", RpcTarget.All, FindObjectOfType<userData>().Username); }
        
        //set text of this object in the list entery
        lobbyPlayer.name.SetText(lobbyPlayer.info.NickName);
        view.Owner.NickName = lobbyPlayer.name.text;
        gameObject.name = lobbyPlayer.name.text;
    }
    [PunRPC] 
    void SetNickname(string name) { lobbyPlayer.info.NickName = name; }
    public void AddPoints(int amount)
    { points += amount; }
    public void UpHealth(float amount) 
    { health += amount;}
    public void DownHealth(float amount, Gun shooter)
    {
        if (health < 1) 
        { alive = false; 
          Dead(); shooter.player.AddPoints(1); }
        else { health -= amount; }
    }
    public void DownHealth(float amount)
    {
        playerHurt.Play();
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
          
            Gun g = GetComponentInChildren<Gun>();
            g.player = this;
            g.hitMarker = ui.display.hitmarker;
            g.hitMarker.SetActive(false);
        }
        catch { Debug.LogWarning("Non player characters dismissing UI initialization");}
    }
        
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //other
    {
        if (stream.IsWriting)
        {stream.SendNext(health);}
        if (stream.IsReading)
        {health = (float)stream.ReceiveNext(); }

        try{ //print("player" + ACNUM + "{ health: " + health + ", points: " + points);
                lobbyPlayer.score.SetText(health + ""); }
        catch{ Debug.LogWarning("waiting for player data to link before pulling stats"); }

    }
}
