using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obsticle : MonoBehaviour
{
    GameObject lava;
    // Start is called before the first frame update
    void Start()
    {
  lava = GameObject.Find("fallOffPoint");

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y + 50 <= lava.transform.position.y)
        { Destroy(gameObject); }
    }
}
