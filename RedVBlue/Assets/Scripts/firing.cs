using UnityEngine;

public class firing : MonoBehaviour
{
    private Vector3 currentGrapplePosition;
    private LineRenderer lr;
    private Vector3 hitPoint;
    public Transform camera, player;

    public float Gundamage;
    public int ammo;

    private void Awake()
    {
        camera = GetComponentInParent<Camera>().transform.parent;
       // player = GetComponentInParent<PlayerMovement>().transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { Shoot(); }
    }


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void Shoot()
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


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopShoot(){ print("done"); }
    void addDamage(){print("damage");} 



}

