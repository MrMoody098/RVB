using UnityEngine;
using Photon.Pun;
public class firing : MonoBehaviour
{
    private Vector3 currentGrapplePosition;
    private LineRenderer lr;
    private Vector3 hitPoint;
    public Transform camera, player;

    public float Gundamage;
    public int ammo;

    [HideInInspector]
    public PhotonView view;
    private void Awake()
    {
        view = GetComponentInParent<PhotonView>(); 
        camera = GetComponentInParent<Camera>().transform.parent;
    }

    void Update()
    {
        if (!view.IsMine) { return; }
        if (Input.GetMouseButtonDown(0))
        { view.RPC("Shoot",RpcTarget.All); }
    }
    [PunRPC]
    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit))
        {
            hitPoint = hit.point;
            //if the ovject has an attributes script
            if (hit.collider.GetComponent<CharacterAttributes>() != null)  
            {
                //change its attributes
               CharacterAttributes enemy = hit.collider.GetComponent<CharacterAttributes>();
  
                enemy.DownHealth(1);
                print(enemy.health);

                Debug.DrawLine(camera.transform.position,hit.point, Color.red,1);
                Debug.DrawLine(transform.Find("tip").position, hit.point, Color.green, 1);
            }
        }
    }




}

