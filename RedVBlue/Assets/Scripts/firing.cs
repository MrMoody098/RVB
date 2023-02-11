using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class firing : MonoBehaviour
{
    private Vector3 currentGrapplePosition;
    private LineRenderer lr;
    private Vector3 hitPoint;
    public Transform camera, player;

    public float Gundamage;
    public int ammo;

    public GameObject bullet;

    [HideInInspector]
    public PhotonView view;
    private void Awake()
    {
        view = GetComponentInParent<PhotonView>(); 
        camera = GetComponentInParent<Camera>().transform;
    }

    void Update()
    {
        if (!view.IsMine) { return; }
        if (Input.GetMouseButtonDown(0))
        { view.RPC("Shoot",RpcTarget.All);
        }
    }
    [PunRPC]
    public void Shoot() //to whom ever improves this, put in coroutine and have it calculate bullet dip and
                        //yeird return new wait for seconds (calculated time)  enjoy
    {
        RaycastHit hit;
        Ray ray = new Ray(camera.position, camera.forward);
        GameObject Nb = Instantiate(bullet.gameObject, transform.Find("tip").position, Quaternion.identity);
        Nb.GetComponent<Bullet>().direction = transform.Find("tip").forward * 400 * Time.deltaTime;
        if (Physics.Raycast(ray, out hit))
        {

            //if the object has an attributes script
            if (hit.collider.GetComponent<CharacterAttributes>() != null)  
            {
                //change its attributes
               CharacterAttributes enemy = hit.collider.GetComponent<CharacterAttributes>();
  
                enemy.DownHealth(1);
                enemy.GetComponent<Rigidbody>().AddForceAtPosition(hit.normal*200*-1, hit.point);

                Debug.DrawLine(camera.transform.position,hit.point, Color.red,1);
                Debug.DrawLine(transform.Find("tip").position, hit.point, Color.green, 1);
            }
        }
    }




}

