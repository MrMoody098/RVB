using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ROTATOR : MonoBehaviour

{
    public float rotateSpeed=3;
    private Transform transform;
    // Start is called before the first frame update
    void Start()
    {
       transform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, rotateSpeed, 0);
    }
}
