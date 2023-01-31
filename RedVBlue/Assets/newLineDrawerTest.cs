using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newLineDrawerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(GetComponentInParent<Camera>().transform.position, GetComponentInParent<Camera>().transform.forward);
        if(Physics.Raycast(ray,out RaycastHit hit, LayerMask.NameToLayer("grapplable")))
        {
            if(Input.GetButtonDown("Fire1"))
            {
              LineRenderer lr =  gameObject.AddComponent<LineRenderer>();
                lr.useWorldSpace = false;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, transform.InverseTransformPoint(hit.point));
            }
        }
    }
}
