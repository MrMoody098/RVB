using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Vector3 direction;
    // Update is called once per frame
    void Update()
    {
        transform.position += direction;
        Destroy(gameObject, 3);
    }
}
