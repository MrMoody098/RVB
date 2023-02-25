using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

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
    }

    void Update()
    {
        if (!view.IsMine) { return; }
        if (Input.GetMouseButtonDown(0))
        { view.RPC("Shoot",RpcTarget.All);}
    }
    [PunRPC]
    public void Shoot() //to whom ever improves this, put in coroutine and have it calculate bullet dip and
             //yeird return new wait for seconds (calculated time)  enjoy
    {
        RaycastHit hit;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        GameObject Nb = Instantiate(bullet.gameObject, transform.Find("tip").position, Quaternion.identity);
        Nb.GetComponent<Bullet>().direction = transform.Find("tip").forward * 400 * Time.deltaTime;
        if (Physics.Raycast(ray, out hit))
        {
            //if the object has an attributes script
            if (hit.collider.GetComponent<Player>() != null)  
            {
                //change its attributes
                Player enemy = hit.collider.GetComponent<Player>();
                enemy.DownHealth(1,this);
                enemy.GetComponent<Rigidbody>().AddForceAtPosition(hit.normal*200*-1, hit.point);

                if(player.view.IsMine)
                {hitMarker.SetActive(true);
                    hitMarker.GetComponent<HitMarker>().Mark(camera, hit.point, 1);}

                Debug.DrawLine(camera.transform.position,hit.point, Color.red,1);
                Debug.DrawLine(transform.Find("tip").position, hit.point, Color.green, 1);
            }
        }
    }




}

