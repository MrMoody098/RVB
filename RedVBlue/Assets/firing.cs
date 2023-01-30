using UnityEngine;

public class firing : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 hitPoint;
    public Transform camera, player;
    public float Gundamage;
    public int ammo;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartShoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopShoot();
        }
    }


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartShoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit))
        {
            hitPoint = hit.point;
            if (hit.collider.GetComponent<CharcterAtributes>()   != null) 
            {
               CharcterAtributes enemy = hit.collider.GetComponent<CharcterAtributes>()  ;

                enemy.DownHealth(1);
                print(enemy.health);

            }




        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopShoot()
    {
        print("done");
    }

    private Vector3 currentGrapplePosition;

    void addDamage()
    {
        print("damage");
    } }

