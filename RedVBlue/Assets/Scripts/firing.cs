using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using System;

public class firing : MonoBehaviour
{
    public GameObject hitMarker;

    private LineRenderer lr;

    public Camera camera;

    [HideInInspector]
    public Player player;

    public float Gundamage;
    public int ammo;

    public GameObject bullet;

    [HideInInspector]
    public PhotonView view;
    private void Start()
    {
        view = GetComponentInParent<PhotonView>(); 
        camera = GetComponentInParent<Camera>();
        player = GetComponentInParent<Player>();

        onChangeShootPermission += UpdateNetworkShooter;//this is how we asign an event to a method in base c# i guess
        //instead of onchange.addlistener(updatenetworkshooter) which would be way more intuative
        
    }

    private bool _ableToShoot;
    public delegate void IsAbleToShoot(bool boolean);
    public event IsAbleToShoot onChangeShootPermission;
    public bool ableToShoot
    {
        get { return _ableToShoot; }
        set
        {
            if (_ableToShoot != value)//if something different
            {
                _ableToShoot = value;

                if (onChangeShootPermission != null)
                {
                    UpdateNetworkShooter(_ableToShoot);
                }
            }
        }
    }
    void UpdateNetworkShooter(bool boolean) 
    { if (!ableToShoot) { return; }
        view.RPC("TransmitNetworkShooter", RpcTarget.All); }

    [PunRPC]
    public void TransmitNetworkShooter()
    {
        if (!view.IsMine) { return; }
        ableToShoot = true;
        print(player.lobbyPlayer.info.NickName + " is armed and dangerious!");
    }
    
    void Update()
    {
        if (!view.IsMine) { return; }

        if (Input.GetMouseButtonDown(0) && _ableToShoot)
        { view.RPC("Shoot",RpcTarget.All);}
    }

    [PunRPC]
    public void Shoot()
    {

        RaycastHit hit;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        GameObject Nb = Instantiate(bullet.gameObject, transform.Find("tip").position, Quaternion.identity);
        Nb.GetComponent<CustomBullet>().direction = transform.Find("tip").forward * 400 * Time.deltaTime;

        if (Physics.Raycast(ray, out hit))
        {
            //if the object has an attributes script
            if (hit.collider.GetComponent<Player>() != null)  
            {
                Player enemy = hit.collider.GetComponent<Player>();
               // enemy.DownHealth(1,this);
                enemy.GetComponent<Rigidbody>().AddForceAtPosition(hit.normal*200*-1, hit.point);

                if(player.view.IsMine)
                {hitMarker.SetActive(true);
                    hitMarker.GetComponent<HitMarker>().Mark(camera, hit.point, 1);}

                Debug.DrawLine(camera.transform.position,hit.point, Color.red,1);
                Debug.DrawLine(transform.Find("tip").position, hit.point, Color.green, 1);
            }
        }
        if (player.lobbyPlayer.index == 1)
        { FindObjectOfType<RoomLobby>().SetActiveShooter(0); }
        else { FindObjectOfType<RoomLobby>().SetActiveShooter(1); }
        _ableToShoot = false;
    }




}

