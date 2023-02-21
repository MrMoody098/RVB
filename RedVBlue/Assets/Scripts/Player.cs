using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public string username;
    public GameObject uiPrefab;
    
    [HideInInspector]
    public RoomUI ui;

    public int points = 0;
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

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();

        health = maxHealth;

        view = GetComponent<PhotonView>();
        if (view)
        { 
            if (view.IsMine) 
            { RoomUI.player = view; InitializePlayerUI(); }
            else { camera.targetDisplay = 2; }
            grapplingGun = camera.GetComponentInChildren<GrapplingGun>();
            grapplingGun.GetComponent<firing>().player = this;
        }
    }
    public void UpHealth(float amount) 
    { health += amount;
        print(health);}
    public void DownHealth(float amount, firing shooter)
    {
        if (health < 1) 
        { alive = false; 
          Dead(); shooter.player.points++; }
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
        points--;
        transform.position = GameObject.Find("Spawn").transform.position;
        ui.deathScreen.SetActive(true);
        ui.deathScreenAnimation.Play();
        health = maxHealth;
    }

    void InitializePlayerUI()
    {
       // try
        {
            ui = Instantiate(uiPrefab).GetComponentInChildren<RoomUI>();
            RoomUI.userName = username;
          //  ui.GetComponent<Canvas>().worldCamera = this.camera;
            firing f = GetComponentInChildren<firing>();
            f.player = this;
            f.hitMarker = ui.display.hitmarker;
            f.hitMarker.SetActive(false);
        }
       // catch { Debug.LogWarning("Non player characters dismissing UI initialization");}
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
