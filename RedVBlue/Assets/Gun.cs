using Photon.Pun;
using UnityEngine;

public class Gun : MonoBehaviour
{
    
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    private Vector3 currentGrapplePosition;
    public Transform gunTip;
    [HideInInspector]
    public Player player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public GameObject hitMarker;
    public float Gundamage;
    public int ammo;
    public GameObject bullet;
    PhotonView view;
    void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);

        view = GetComponentInParent<PhotonView>();


        onChangeShootPermission += UpdateNetworkShooter;//this is how we asign an event to a method in base c# i guess
                                                        //instead of onchange.addlistener(updatenetworkshooter) which would be way more intuative
    }
    void LateUpdate() { DrawRope(); }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)){  StartGrapple();} 
        else if (Input.GetMouseButtonUp(1)) { StopGrapple(); }

        if (!view.IsMine) { return; }
       
        if (Input.GetMouseButtonDown(0) && _ableToShoot)
        { view.RPC("Shoot", RpcTarget.All); }
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
    {
        if (!ableToShoot) { return; }
        view.RPC("TransmitNetworkShooter", RpcTarget.All);
    }

    [PunRPC]
    public void TransmitNetworkShooter()
    {
        if (!view.IsMine) { return; }
        ableToShoot = true;
        //this makes the noise
        GetComponent<AudioSource>().Play();
        print(player.lobbyPlayer.info.NickName + " is armed and dangerious!");
    }

    [PunRPC]
    public void Shoot()
    {
        //this makes the noise
        
        RaycastHit hit;
        Ray ray = new Ray(player.camera.transform.position, player.camera.transform.forward);
        GameObject Nb = Instantiate(bullet.gameObject, transform.Find("tip").position, Quaternion.identity);
        Nb.GetComponent<Bullet>().direction = transform.Find("tip").forward * 400 * Time.deltaTime;
        if (Physics.Raycast(ray, out hit))
        {
            //if the object has an attributes script
            if (hit.collider.GetComponent<Player>() != null)
            {
                Player enemy = hit.collider.GetComponent<Player>();
                enemy.DownHealth(1, this);
                enemy.GetComponent<Rigidbody>().AddForceAtPosition(hit.normal * 200 * -1, hit.point);

                if (player.view.IsMine)
                { hitMarker.SetActive(true);
                    hitMarker.GetComponent<HitMarker>().Mark(player.camera, hit.point, 1);}

                Debug.DrawLine(player.camera.transform.position, hit.point, Color.red, 1);
                Debug.DrawLine(transform.Find("tip").position, hit.point, Color.green, 1);
            }
        }
        if (player.lobbyPlayer.index == 1)
        { FindObjectOfType<RoomLobby>().SetActiveShooter(0); }
        else { FindObjectOfType<RoomLobby>().SetActiveShooter(1); }
        _ableToShoot = false;
    }

    void StartGrapple() //this needs to be rewritten to acomidate moving closer and moving back to where you were
    {
        RaycastHit hit;
        if (Physics.Raycast(player.camera.transform.position, player.camera.transform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            currentGrapplePosition = gunTip.localPosition;
        }
    }
    void StopGrapple()
    { lineRenderer.positionCount = 0;
        Destroy(joint);}
    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;
        if (Input.GetButton("Fire3"))
        { joint.maxDistance -= Time.deltaTime * 200;
            joint.minDistance -= Time.deltaTime * 200; }

        //currentGrapplePosition = Vector3.Lerp(currentGrapplePosition,transform.InverseTransformPoint( grapplePoint), Time.deltaTime * 8f);

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, transform.InverseTransformPoint(grapplePoint)); //currentGrapplePosition);
    }
    public bool IsGrappling()
    {return joint != null;}
    public Vector3 GetGrapplePoint()
    { return grapplePoint; }
}
