using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 direction;
    // Update is called once per frame
    void Update()
    {
        transform.position += direction;
        Destroy(gameObject, 3);
    }
}
